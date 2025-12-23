using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class NavalBattle
{
    public Vector3Int pos;
    public TileData tile => Map.main.GetTile(pos);
    public Civilisation attackerCiv;    
    public Civilisation defenderCiv;
    public General attackerGeneral;
    public General defenderGeneral;
    public List<Fleet> attackingFleets = new List<Fleet>();
    public List<Fleet> defendingFleets = new List<Fleet>();
    public List<Boat> attackingReserves = new List<Boat>();
    public List<Boat> attackingRetreated = new List<Boat>();
    public FleetBattleLine attackingFrontLine = new FleetBattleLine();
    public int attackerCount;
    public int attackerCasualties;
    public List<Boat> defendingReserves = new List<Boat>();
    public List<Boat> defendingRetreated = new List<Boat>();
    public FleetBattleLine defendingFrontLine = new FleetBattleLine();
    public int defenderCount;
    public int defenderCasualties;    
    public int attackerDiceRoll,defenderDiceRoll;
    public List<WeightedChoice> diceRolls = new List<WeightedChoice>();
    public int WarID;
    public bool active = false;
    public int battleLength;
    public int attackPhases;
    public NetworkNavalBattle networkBattle;
    BattleUI bui;

    public static int GetEstimatedBattleLength(float attackerStrength,float defenderStrength)
    {
        int time = (int)(50 * Mathf.Min(attackerStrength / defenderStrength, defenderStrength / attackerStrength));
        return time;
    }
    public List<Fleet> GetInvolvedFleets()
    {
        List<Fleet> involved = new List<Fleet>(); 
        involved.AddRange(attackingFleets); 
        involved.AddRange(defendingFleets);
        return involved;
    }
    public int DefenderDiceRollBonus()
    {
        int diceRoll = 0;
        diceRoll += (int)defenderCiv.defenderDiceRoll.v;
        return diceRoll;
    }
    public int AttackerDiceRollBonus()
    {
        int diceRoll = 0;
        diceRoll += (int)attackerCiv.attackerDiceRoll.v;
        return diceRoll +tile.terrain.attackerDiceRoll + (int)tile.localAttackerDiceRoll.v;
    }
    public bool Involving(int civID)
    {
        if(attackerCiv.CivID == civID || defenderCiv.CivID == civID) { return true; }
        return false;
    }
    public NavalBattle(SaveGameNavalBattle battle)
    {
        pos = battle.pos.GetVector3Int();
        active = battle.active;
        Game.main.ongoingNavalBattles.Add(this);
        Game.main.hourTick.AddListener(HourTick);
        bui = GameObject.Instantiate(UIManager.main.BattleUIPrefab, Map.main.tileMapManager.tilemap.CellToWorld(battle.pos.GetVector3Int()), Quaternion.identity, UIManager.main.worldCanvas).GetComponent<BattleUI>();
        bui.navalBattle = this;
        UIManager.main.WorldSpaceUI.Add(bui.gameObject);
    }
    public NavalBattle(Vector3Int Pos, Fleet attacker, Fleet defender, int warID = -1)
    {
        active = true;
        Game.main.ongoingNavalBattles.Add(this);
        Game.main.hourTick.AddListener(HourTick);
        bui = GameObject.Instantiate(UIManager.main.BattleUIPrefab,Map.main.tileMapManager.tilemap.CellToWorld(Pos),Quaternion.identity,UIManager.main.worldCanvas).GetComponent<BattleUI>();
        bui.navalBattle = this;
        UIManager.main.WorldSpaceUI.Add(bui.gameObject);
        pos = Pos;
        attackingReserves.AddRange(attacker.boats);
        if (attacker.civID > -1)
        {
            attackerCiv = Game.main.civs[attacker.civID];
        }
        attacker.EnterBattle();
        defendingReserves.AddRange(defender.boats);
        if (defender.civID > -1)
        {
            defenderCiv = Game.main.civs[defender.civID];
        }
        defender.EnterBattle();
        Map.main.GetTile(pos)._navalBattle = this;
        WarID = warID;
        for(int i = 1; i < 11; i++)
        {
            diceRolls.Add(new WeightedChoice(i, (int)Mathf.Pow(6 - Mathf.Abs(i - 5),2)));
        }
        attackingFrontLine = new FleetBattleLine(31);
        defendingFrontLine = new FleetBattleLine(31);
        PhaseTick();
        attackingFleets.Add(attacker);
        attackerCount += attacker.boats.Count;
        defendingFleets.Add(defender);
        defenderCount += defender.boats.Count;
        battleLength = 0;
        attackPhases = 0;
        CheckGeneral(true, attacker);
        CheckGeneral(false, defender);

    }
    void CheckGeneral(bool attacker, Fleet fleet)
    {
        General current = attacker ? attackerGeneral : defenderGeneral;
        if (fleet.general != null && fleet.general.active)
        {
            if (current != null && current.active)
            {
                if (current.siegeSkill < fleet.general.siegeSkill)
                {
                    current = fleet.general;
                }
            }
            else
            {
                current = fleet.general;
            }
        }
        if (attacker)
        {
            attackerGeneral = current;
        }
        else
        {
            defenderGeneral = current;
        }
    }
    public void AddToBattle(Fleet fleet, bool isAttacker)
    {
        if (isAttacker)
        {
            attackingFleets.Add(fleet);
            attackerCount += fleet.boats.Count;
            attackingReserves.AddRange(fleet.boats);
            fleet.EnterBattle();
            CheckGeneral(true, fleet);
        }
        else
        {
            defendingFleets.Add(fleet);
            defenderCount += fleet.boats.Count;
            defendingReserves.AddRange(fleet.boats);
            fleet.EnterBattle();
            CheckGeneral(false, fleet);
        }
        if (Game.main.isMultiplayer && networkBattle != null)
        {
            networkBattle.SendBattleDataRpc();
        }
    }
    public void DoBattleEnd(bool attackerWin = true, bool wipe = false, int winnerCasualties = -1, int loserCasualties = -1)
    {
        UIManager.main.WorldSpaceUI.Remove(bui.gameObject);
        Civilisation winnerCiv = attackerWin ? attackerCiv : defenderCiv;
        Civilisation loserCiv = attackerWin ? defenderCiv : attackerCiv;    
        List<Fleet> winners = attackerWin ? attackingFleets : defendingFleets;
        List<Fleet> losers = attackerWin ? defendingFleets : attackingFleets;
        if (!Game.main.isMultiplayer || NetworkManager.Singleton.IsServer)
        {
            foreach (var winner in winners)
            {
                if (winner != null)
                {
                    winner.ExitBattle();
                }
            }
            if (!wipe)
            {
                foreach (var loser in losers)
                {
                    if (loser != null)
                    {
                        loser.SetRetreat(loser.RetreatProvince());
                        loser.ExitBattle();
                    }
                }
            }
            else
            {
                foreach (var loser in losers)
                {
                    if (loser != null)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            loser.GetComponent<NetworkFleet>().ExitEnterRpc(false, loser.pos);
                        }
                        else
                        {
                            loser.OnExitTile(loser.tile);
                        }
                        loser.ExitBattle();
                        GameObject.Destroy(loser.gameObject);
                    }
                }
            }
        }
        winnerCiv.AddPrestige(Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f), 2f) *(1f + winnerCiv.battlePrestige.v));
        winnerCiv.AddArmyTradition(Mathf.Min((winnerCasualties + 1f) / (winnerCiv.forceLimit.v * 1000f + 1f) * 12f , 5f) * (1f + winnerCiv.battleTraditon.v));
        loserCiv.AddPrestige(-Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f), 2f) / 2f);
        loserCiv.AddArmyTradition(Mathf.Min((loserCasualties + 1f) / (loserCiv.forceLimit.v * 1000f + 1f) * 12f, 2f) * (1f + loserCiv.battleTraditon.v));
        
        if (WarID > -1)
        {
            float warScore = Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f) * 6f,10f);   
            if(winnerCiv == Game.main.ongoingWars[WarID].defenderCiv) { warScore *= -1f; }
            UpdateWar(warScore);
        }
        Game.main.ongoingNavalBattles.Remove(this);
        Game.main.hourTick.RemoveListener(HourTick);
        if (Involving(Player.myPlayer.myCivID))
        {
            BattleResultUI brui = GameObject.Instantiate(UIManager.main.battleResultPrefab, UIManager.main.transform).GetComponent<BattleResultUI>();
            UIManager.main.UI.Add(brui.gameObject);
            string attackerName = attackerCiv.civName;
            string defenderName = defenderCiv.civName;
            bool win = attackerWin ? (Player.myPlayer.myCivID == winnerCiv.CivID) : (Player.myPlayer.myCivID == winnerCiv.CivID);
            brui.SetUpText(win? "Victory at " + tile.Name : "Defeat at " + tile.Name ,attackerName, defenderName, attackerCount, defenderCount, attackerCasualties, defenderCasualties, pos);
        }
        if (Map.main.GetTile(pos)._navalBattle == this)
        {
            Map.main.GetTile(pos)._navalBattle = null;
        }
        active = false;
    }
    void BattleEnd(bool attackerWin = true, bool wipe = false)
    {
        int winnerCasualties = attackerWin ? attackerCasualties : defenderCasualties;
        int loserCasualties = attackerWin ? defenderCasualties : attackerCasualties;
        if (Game.main.isMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer && networkBattle != null)
            {
               networkBattle.EndBattleRpc(attackerWin, wipe, winnerCasualties, loserCasualties);
            }
        }
        else
        {
            DoBattleEnd(attackerWin, wipe, winnerCasualties, loserCasualties);
        }
    }
    void UpdateWar(float warScoreForAttacker)
    {
        Game.main.ongoingWars[WarID].AddBattle(warScoreForAttacker);
    }
    bool CheckInstakill(bool attacker)
    {
        float morale = TotalSailors(attacker);
        int sideA = GetSideSize(attacker);
        int sideB = GetSideSize(!attacker);
        if (morale <= 0.5 || sideB >= sideA * 10)
        {
            List<Fleet> losers = attacker ? attackingFleets : defendingFleets;
            foreach (var loser in losers)
            {
                if (loser != null)
                {
                    if (attacker)
                    {
                        attackerCasualties += loser.boats.Count;
                    }
                    else
                    {
                        defenderCasualties += loser.boats.Count;
                    }
                }
            }
            BattleEnd(!attacker, true);
            return true;
        }
        return false;
    }
    bool CheckRetreat(bool attacker)
    {
        float morale = TotalSailors(attacker);
        if (morale <= 0)
        {
            BattleEnd(!attacker, false);
            return true;
        }
        return false;
    }
    bool CheckWipe(bool attacker)
    {
        float morale = TotalSailors(attacker);
        int sideA = GetSideSize(attacker);
        int sideB = GetSideSize(!attacker);
        if(morale <= 0 && sideB >= sideA * 2)
        {
            List<Fleet> losers = attacker ? attackingFleets : defendingFleets;
            foreach (var loser in losers)
            {
                if (attacker)
                {
                    attackerCasualties += loser.boats.Count;
                }
                else
                {
                    defenderCasualties += loser.boats.Count;
                }                                  
            }
            BattleEnd(!attacker, true);
            return true;
        }
        return false;
    }
    void FillBattleLine(List<Boat> reserves, List<Boat> retreated, FleetBattleLine line,int targetWidth = -1,bool back = false)
    {
        List<Boat> used = new List<Boat>();        
        used = line.RefillboatsFront(reserves, retreated, targetWidth);      
        used.ForEach(i => reserves.Remove(i));
    }
    public void HourTick()
    {
        if (attackingFrontLine == null) { return; }
        battleLength++;
        if(battleLength % 3 == 1)
        {
            PhaseTick();
            return;
        }
        if (attackingReserves.Count > defendingReserves.Count)
        {
            FillBattleLine(defendingReserves, defendingRetreated, defendingFrontLine);
            int targetWidth = defendingFrontLine.GetUsedWidth();
            FillBattleLine(attackingReserves, attackingRetreated, attackingFrontLine, targetWidth);
        }
        else
        {
            FillBattleLine(attackingReserves, attackingRetreated, attackingFrontLine);
            int targetWidth = attackingFrontLine.GetUsedWidth();
            FillBattleLine(defendingReserves, defendingRetreated, defendingFrontLine, targetWidth);
        }
        if (battleLength == 1)
        {
            if (CheckInstakill(false)) { return; }
            if (CheckInstakill(true)) { return; }
        }
        FleetBattleLine defendingLineClone = new FleetBattleLine(defendingFrontLine);
        AttackerHullDamage();
        DefenderHullDamage(defendingLineClone);
        int dCasualties = 0;
        int aCasualties = 0;
        AttackerBoard(ref aCasualties, ref dCasualties);
        attackerCasualties += aCasualties;
        defenderCasualties += dCasualties;
        UIManager.main.NewCombatText("-" + aCasualties, tile.worldPos() + new Vector3(-0.5f,-0.5f), 2f, true);
        UIManager.main.NewCombatText("-" + dCasualties, tile.worldPos() + new Vector3(0.5f, -0.5f), 2f, true);
        if (Player.myPlayer.selectedNavalBattle == this)
        {
            Debug.Log("Casualties: " + aCasualties + " " + dCasualties + " Battle Length: " + battleLength + " battlePhase: " + attackPhases);
        }
        if (attackPhases < 4)
        {
            if (CheckWipe(false)) { return; }
            if (CheckWipe(true)) { return; }
        }
        else
        {
            if (CheckRetreat(false)) { return; }
            if (CheckRetreat(true)) { return; }
        }
        float armyQ = 0;
        for (int i = 0; i < attackingFrontLine.width; i++)
        {
            if (attackingFrontLine.boats[i] != null && attackingFrontLine.boats[i].segmentID == 0)
            {
                armyQ += attackingFrontLine.boats[i].boat.sailors;
            }
        }
        if (armyQ == 0) { BattleEnd(false);return; }
        armyQ = 0;
        for (int i = 0; i < defendingFrontLine.width; i++)
        {
            if (defendingFrontLine.boats[i] != null && defendingFrontLine.boats[i].segmentID == 0)
            {
                armyQ += defendingFrontLine.boats[i].boat.sailors;
            }
        }
        if (armyQ == 0) { BattleEnd(true); return; }
    }
    public void PhaseTick()
    {
        if (Game.main.isMultiplayer)
        {
            if (NetworkManager.Singleton.IsServer && networkBattle != null)
            {
                int attackerRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
                int defendingRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
                networkBattle.BattleDiceRollRpc(attackerRoll, defendingRoll);
            }
        }
        else
        {
            attackerDiceRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
            defenderDiceRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
        }
        attackPhases++;
    }
    void AttackerBoard(ref int attackerCasualties, ref int defenderCasualties)
    {
        List<Boat> attacked = new List<Boat>();
        for (int i = 0; i < attackingFrontLine.width; i++)
        {
            if (attackingFrontLine.boats[i] != null && attackingFrontLine.boats[i].boat != null && attackingFrontLine.boats[i].boat.sailors > 0 && attackingFrontLine.boats[i].hullStrength > 0 && attackingFrontLine.boats[i].boat.type > -1)
            {
                Boat attacker = attackingFrontLine.boats[i].boat;
                if (attacked.Contains(attacker)) { continue; }
                int flankingRange = attackerCiv.boats[attacker.type].flankingRange;
                int target = FindTarget(defendingFrontLine, i, flankingRange);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (attackerGeneral != null && attackerGeneral.active) { generalBonus =  attackerGeneral.combatSkill; }
                    int defenderGeneralBonus = 0;
                    if (defenderGeneral != null && defenderGeneral.active) { defenderGeneralBonus =defenderGeneral.combatSkill; }
                    Boat defender = defendingFrontLine.boats[target].boat;
                    if (defender.sailors > 0 && defender.hullStrength > 0 && defender.type > -1)
                    {
                        float attackerMods = Modifiers((float)(Mathf.Max(attacker.sailors - 20, 1)) /(float)defender.sailors, battleLength, disciplineDiff: attackerCiv.discipline.v - defenderCiv.discipline.v, combatAbilityDiff: attackerCiv.boats[attacker.type].combatAbility.v - defenderCiv.boats[defender.type].combatAbility.v);
                        float attackerDamage = BaseCasualties(attackerDiceRoll + generalBonus, AttackerDiceRollBonus()) * attackerMods;
                        float defenderMods = Modifiers((float)defender.sailors / (float)(Mathf.Max(attacker.sailors-20,1)), battleLength, disciplineDiff: defenderCiv.discipline.v - attackerCiv.discipline.v, combatAbilityDiff: defenderCiv.boats[attacker.type].combatAbility.v - attackerCiv.boats[defender.type].combatAbility.v);
                        float defenderDamage = BaseCasualties(defenderDiceRoll + defenderGeneralBonus, DefenderDiceRollBonus()) * defenderMods;

                        defender.TakeSailorDamage((int)attackerDamage);
                        attacker.TakeSailorDamage((int)defenderDamage);

                        attackerCasualties += (int)defenderDamage;
                        defenderCasualties += (int)attackerDamage;

                        attacked.Add(attacker);
                    }
                }
            }
        }
    }
    void AttackerHullDamage()
    {
        for(int i = 0; i < attackingFrontLine.width;i++)
        {
            if (attackingFrontLine.boats[i] != null && attackingFrontLine.boats[i].boat != null && attackingFrontLine.boats[i].boat.sailors > 0 && attackingFrontLine.boats[i].hullStrength > 0 && attackingFrontLine.boats[i].boat.type > -1)
            {
                Boat attacker = attackingFrontLine.boats[i].boat;
                int target = FindTarget(defendingFrontLine, i, 0);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (attackerGeneral != null &&attackerGeneral.active) { generalBonus = attackerGeneral.combatSkill; }
                    Boat defender = defendingFrontLine.boats[target].boat;
                    if (defender.sailors > 0 && defender.hullStrength > 0 && defender.type > -1)
                    {
                        defender.TakeHullDamage(HullDamage(attackingFrontLine.boats[i].cannons, attackerDiceRoll + generalBonus, AttackerDiceRollBonus()));
                        defendingFrontLine.boats[target].hullStrength -= HullDamage(attackingFrontLine.boats[i].cannons, attackerDiceRoll + generalBonus, AttackerDiceRollBonus());
                    }
                }
            }
        }        
    }
    int FindTarget(FleetBattleLine targetLine,int ownPos,int flankingRange,bool preferFlank = false)
    {
        Boat defender = new Boat(-1,-1);
        int centre = ownPos;
        for (int j = 0; j < flankingRange + 1; j++)
        {
            int index = centre + j;
            if (index >= 0 && index < targetLine.width)
            {
                if (preferFlank && index == ownPos)
                {
                    continue;
                }
                if (targetLine.boats[index] != null && targetLine.boats[index].boat.sailors > 0 && targetLine.boats[index].hullStrength > 0 && targetLine.boats[index].boat.type > -1)
                {
                    return index;
                }
            }
            if (j > 0)
            {
                index = centre - j;
                if (index >= 0 && index < targetLine.width)
                {
                    if (targetLine.boats[index] != null && targetLine.boats[index].hullStrength > 0 && targetLine.boats[index].boat.sailors > 0 && targetLine.boats[index].boat.type > -1)
                    {
                        return index;
                    }
                }
            }
        }
        if (targetLine.boats[ownPos] != null && targetLine.boats[ownPos].boat.sailors > 0 && targetLine.boats[ownPos].hullStrength > 0 && targetLine.boats[ownPos].boat.type > -1)
        {
            return ownPos;
        }
        return -1;
    }
    void DefenderHullDamage(FleetBattleLine DefendingLineClone)
    {
        for (int i = 0; i < DefendingLineClone.width; i++)
        {
            if (DefendingLineClone.boats[i] != null && DefendingLineClone.boats[i].boat != null && DefendingLineClone.boats[i].boat.sailors > 0 && DefendingLineClone.boats[i].hullStrength > 0 && DefendingLineClone.boats[i].boat.type > -1)
            {
                Boat attacker = DefendingLineClone.boats[i].boat;             
                int target = FindTarget(attackingFrontLine, i, 0);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (defenderGeneral != null && defenderGeneral.active) { generalBonus = defenderGeneral.combatSkill; }
                    Boat defender = attackingFrontLine.boats[target].boat;
                    if (defender.sailors > 0 && defender.hullStrength > 0 && defender.type > -1)
                    {
                        defender.TakeHullDamage(HullDamage(DefendingLineClone.boats[i].cannons, defenderDiceRoll + generalBonus, DefenderDiceRollBonus()));
                        attackingFrontLine.boats[target].hullStrength -= HullDamage(DefendingLineClone.boats[i].cannons, defenderDiceRoll + generalBonus, DefenderDiceRollBonus());
                    }
                }
            }
        }       
    }
    public float HullDamage(int cannons,int diceRoll, int terrain = 0, int diceRollBonus = 0)
    {
        return (0.01f + cannons * 0.5f) * (diceRoll + diceRollBonus + terrain);
    }
    public float BaseCasualties(int diceRoll,int terrain = 0, int diceRollBonus = 0)
    {
        return 2 + 0.5f * (diceRoll + diceRollBonus + terrain);
    }
    public float Modifiers(float strengthRatio, int BattleLength, float combatAbilityDiff = 0, float disciplineDiff = 0f)
    {
        return strengthRatio * (1f + combatAbilityDiff) * (1f + disciplineDiff) * (1f + (float)BattleLength / 100f);
    }
    public float TotalMaxSailors(bool attacker)
    {
        List<Boat> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        float moraleTotal = 0f;
        float count = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1)
            {
                moraleTotal += regiment.maxSailors;
                count++;
            }
        }
        return moraleTotal / count;
    }
    public float TotalSailors(bool attacker)
    {
        List<Boat> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        float moraleTotal = 0f;
        float count = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1)
            {
                moraleTotal += regiment.sailors;
                count++;
            }
        }
        return moraleTotal / count;
    }
    public int GetSideSize(bool attacker)
    {
        List<Boat> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        int sideSize = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1 && regiment.hullStrength > 0 && regiment.sailors > 0)
            {
                sideSize += regiment.sailors;
            }
        }
        return sideSize;
    }
    public List<Boat> GetSide(bool attacker)
    {
        List<Boat> side = new List<Boat>();
        if (attacker)
        {
            side.AddRange(attackingFrontLine.GetBoats());
            side.AddRange(attackingReserves);
            side.AddRange(attackingRetreated);
        }
        else
        {
            side.AddRange(defendingFrontLine.GetBoats());
            side.AddRange(defendingReserves);
            side.AddRange(defendingRetreated);
        }
        return side;
    }
}
