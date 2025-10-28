using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Siege
{
    public bool inProgress;
    public float progress;
    public int fortLevel;
    public TileData target;
    public int leaderCivID;
    public int tickTime = 12;
    public int tickTimer = 0;
    public int progressRoll;
    public List<Army> armiesSieging = new List<Army>();
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
    void Tick()
    {
        progressRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
        progress += (float)progressRoll / (100f * (2 * fortLevel + 0.5f));
        if(Mathf.Round(progress * 100f)/100f >= 1f)
        {
            Complete();
        }
        float siegeAbility = 0f;
        if(leaderCivID > -1)
        {
            siegeAbility = Game.main.civs[leaderCivID].siegeAbility.value;
        }
        tickTime = (int)(24 * (1f + target.localDefensiveness.value + target.civ.fortDefence.value - siegeAbility));
    }
    void Complete()
    {
        List<TileData> neighbors = target.GetNeighborTiles();
        if (leaderCivID == target.civID || target.civ.atWarTogether.Contains(leaderCivID)||(target.civ.overlordID == leaderCivID && leaderCivID > -1))
        {
            target.occupied = false;
            target.occupiedByID = target.civID;
            foreach (var n in neighbors)
            {
                if(n.civID == target.civID && n.occupied && !n.HasNeighboringActiveOccupiedFort(target.civID) && n.fortLevel == 0 && n.armiesOnTile.Count == 0)
                {
                    n.occupied = false;
                    n.occupiedByID = target.civID;
                }
            }
        }
        else if (leaderCivID != target.civID && (target.civ.atWarWith.Contains(leaderCivID)|| leaderCivID == -1) && (!target.occupied || target.occupiedByID == -1 || (leaderCivID == -1 && target.occupied && target.occupiedByID > -1)) )
        {
            if(leaderCivID == -1 && (!target.HasNeighboringActiveFort(-1)))
            {
                target.control = Mathf.Clamp(target.control - 10f,0f,target.maxControl);
                target.heldByID = RebelArmyStats.GetRebelStats(armiesSieging[0]).rebelDemandsID;
                target.heldByType = RebelArmyStats.GetRebelStats(armiesSieging[0]).rebelType;
            }
            target.occupied = true;
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
            target.occupiedByID = occupyId;
            foreach (var n in neighbors)
            {
                if (n.civID == target.civID && !n.occupied && !n.HasNeighboringActiveFort(leaderCivID) && n.fortLevel == 0 && n.armiesOnTile.Count ==0)
                {
                    n.occupied = true;
                    n.occupiedByID = occupyId;
                    if (leaderCivID == -1)
                    {
                        n.heldByID = RebelArmyStats.GetRebelStats(armiesSieging[0]).rebelDemandsID;
                        n.heldByType = RebelArmyStats.GetRebelStats(armiesSieging[0]).rebelType;
                    }
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
    }

}
