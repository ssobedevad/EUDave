using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using UnityEngine;

public class Siege
{
    public bool inProgress;
    public float progress;
    public int fortLevel;
    public TileData target;
    public int leaderCivID;
    public General siegeGeneral;
    public int tickTime = 12;
    public int tickTimer = 0;
    public int progressRoll;
    public List<Army> armiesSieging = new List<Army>();
    public int artillery = 0;
    public int unitsSieging()
    {
        int count = 0;       
        armiesSieging.ForEach(i => count += (int)i.ArmySize());
        return count;
    }
    public List<WeightedChoice> diceRolls = new List<WeightedChoice>();
    public Siege(TileData Target, int LeaderCivID)
    {
        target = Target;
        leaderCivID = LeaderCivID;
        fortLevel = target.fortLevel;
        inProgress = true;
        Game.main.tenMinTick.AddListener(Timer);
        for (int i = 1; i < 11; i++)
        {
            diceRolls.Add(new WeightedChoice(i, 10));
        }
    }

    void Timer()
    {
        armiesSieging.RemoveAll(i => i == null);
        armiesSieging.RemoveAll(i => i.exiled);
        armiesSieging.RemoveAll(i => i.path.Count > 0);
        armiesSieging.RemoveAll(i => i.pos != target.pos);
        if (leaderCivID == -1)
        {
            if(target.occupied && target.occupiedByID == -1)
            {
                target.siege = null;
                Game.main.tenMinTick.RemoveListener(Timer);
                inProgress = false;
                progress = 0;
                tickTimer = 0;
            }
        }
        else
        {
            bool canrelease = target.occupied && (target.civID == leaderCivID || target.civ.atWarTogether.Contains(leaderCivID) || target.civ.subjects.Contains(leaderCivID) || target.civ.overlordID == leaderCivID);
            bool canSiege = leaderCivID != target.civID &&
            (
                (target.civ.atWarWith.Contains(leaderCivID) && !target.occupied) ||
                (target.occupied && target.occupiedByID == -1)
                );
            if (!canrelease && !canSiege)                
            {
                target.siege = null;
                Game.main.tenMinTick.RemoveListener(Timer);
                inProgress = false;
                progress = 0;
                tickTimer = 0;
            }
        }
        if (armiesSieging.FindAll(i => i.civID == leaderCivID).Count > 0)
        {           
            if (unitsSieging() >= Mathf.Max(fortLevel * 3000,1000) && !armiesSieging.Exists(i=>i.inBattle))
            {
                tickTimer++;
                if (tickTimer > tickTime)
                {
                    Tick();
                    tickTimer = 0;
                }
            }
        }
        else
        {
            target.siege = null;
            Game.main.tenMinTick.RemoveListener(Timer);
            inProgress = false;
            progress = 0;
            tickTimer = 0;
        }
    }
    public void AddProgress(int roll)
    {
        progressRoll = roll;
        progress += (float)progressRoll / (100f * (2 * fortLevel + 0.5f));
        if (Mathf.Round(progress * 100f) / 100f >= 1f)
        {
            Complete();
        }
        tickTimer = 0;
    }
    void Tick()
    {
        artillery = 0;
        if (armiesSieging.Count > 0)
        {
            for (int i = 0; i < armiesSieging.Count; i++)
            {
                Army army = armiesSieging[i];
                for (int j = 0; j < army.regiments.Count; j++)
                {
                    Regiment regiment = army.regiments[j];
                    if (regiment.type == 2)
                    {
                        artillery++;
                    }
                }
                if(army.general != null && army.general.active)
                {
                    if(siegeGeneral != null && siegeGeneral.active)
                    {
                        if(siegeGeneral.siegeSkill < army.general.siegeSkill)
                        {
                            siegeGeneral = army.general;
                        }
                    }
                    else
                    {
                        siegeGeneral = army.general;
                    }
                }
            }
        }
        int generalSiege = (siegeGeneral != null && siegeGeneral.active) ? siegeGeneral.siegeSkill : 0;        
        if (Game.main.isMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                int roll = WeightedChoiceManager.getChoice(diceRolls).choiceID + artillery / (1 + fortLevel) + generalSiege;
                Game.main.multiplayerManager.SiegeDiceRollRpc(target.pos, roll);
            }
        }
        else
        {
            int roll = WeightedChoiceManager.getChoice(diceRolls).choiceID + artillery / (1 + fortLevel) + generalSiege;
            AddProgress(roll);
        }
        float siegeAbility = 0f;
        if(leaderCivID > -1)
        {
            siegeAbility = Game.main.civs[leaderCivID].siegeAbility.v;
        }
        tickTime = (int)(24 * (1f + target.localDefensiveness.v + target.civ.fortDefence.v - siegeAbility));
    }
    void Occupy(TileData tile, int occupierID)
    {
        if (Game.main.isMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                Game.main.multiplayerManager.CivOccupyRpc(occupierID, tile.pos);
            }
        }
        else
        {
            Civilisation tileCiv = tile.civ;
            if (tileCiv.atWarWith.Contains(occupierID))
            {
                tile.occupied = true;
                tile.occupiedByID = occupierID;
            }
            else if (tileCiv.atWarTogether.Contains(occupierID))
            {
                tile.occupied = false;
                tile.occupiedByID = occupierID;
            }
            else
            {
                tile.occupied = false;
                tile.occupiedByID = tile.civID;
            }
        }
    }
    void Complete()
    {
        List<TileData> neighbors = target.GetNeighborTiles();
        if (leaderCivID == target.civID || target.civ.atWarTogether.Contains(leaderCivID)||(target.civ.overlordID == leaderCivID && leaderCivID > -1))
        {
            Occupy(target, target.civID);
            foreach (var n in neighbors)
            {
                if(n.civID == target.civID && n.occupied && !n.HasNeighboringActiveOccupiedFort(target.civID) && n.fortLevel == 0 && n.armiesOnTile.Count == 0)
                {
                    Occupy(n,target.civID);
                }
            }
        }
        else if (leaderCivID != target.civID && (target.civ.atWarWith.Contains(leaderCivID)|| leaderCivID == -1) && (!target.occupied || target.occupiedByID == -1 || (leaderCivID == -1 && target.occupied && target.occupiedByID > -1)) )
        {           
            int occupyId = leaderCivID;
            if (leaderCivID > -1)
            {
                Civilisation leaderCiv = Game.main.civs[leaderCivID];
                if (!leaderCiv.CanCoreTile(target))
                {
                    War war = target.civ.GetWars().Find(i => i.Involving(leaderCivID));
                    if (war != null && war.attackerCiv.CivID != leaderCivID && war.defenderCiv.CivID != leaderCivID)
                    {
                        if (war.attackerAllies.Exists(i => i.CivID == leaderCivID))
                        {
                            occupyId = war.attackerCiv.CivID;
                        }
                        else if (war.defenderAllies.Exists(i => i.CivID == leaderCivID))
                        {
                            occupyId = war.defenderCiv.CivID;
                        }
                    }
                }
            }
            Occupy(target, occupyId);
            foreach (var n in neighbors)
            {
                if (n.civID == target.civID && !n.occupied && !n.HasNeighboringActiveFort(leaderCivID) && n.fortLevel == 0 && n.armiesOnTile.Count ==0)
                {
                    Occupy(n, occupyId);
                }
            }
        }
        if (leaderCivID > -1)
        {
            Game.main.civs[leaderCivID].AddArmyTradition(fortLevel);
            target.civ.GetWars().ForEach(i => i.CheckOccupations());
            if (target.civ.atWarWith.Contains(Player.myPlayer.myCivID))
            {
                PeaceDeal peaceDeal = UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>().peaceDeal;
                if (peaceDeal != null)
                {
                    peaceDeal.CheckProvinces();
                }
            }
        }
        inProgress = false;
        Game.main.tenMinTick.RemoveListener(Timer);
        foreach(var fleet in target.fleetsOnTile)
        {
            fleet.SetPath(target.portTile);
        }
    }

}
