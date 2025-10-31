using System.Collections.Generic;
using UnityEngine;

public class Battle
{
    public Vector3Int pos;
    public TileData tile => Map.main.GetTile(pos);
    public Civilisation attackerCiv;
    public bool AttackerRebels;
    public RebelArmyStats attackerRebelStats;
    public bool DefenderRebels;
    public RebelArmyStats defenderRebelStats;
    public Civilisation defenderCiv;
    public General attackerGeneral;
    public General defenderGeneral;
    public List<Army> attackingArmies = new List<Army>();
    public List<Army> defendingArmies = new List<Army>();
    public List<Regiment> attackingReserves = new List<Regiment>();
    public List<Regiment> attackingRetreated = new List<Regiment>();
    public BattleLine attackingFrontLine = new BattleLine();
    public BattleLine attackingBackLine = new BattleLine();
    public int attackerCount;
    public int attackerCasualties;
    public List<Regiment> defendingReserves = new List<Regiment>();
    public List<Regiment> defendingRetreated = new List<Regiment>();
    public BattleLine defendingFrontLine = new BattleLine();
    public BattleLine defendingBackLine = new BattleLine();
    public int defenderCount;
    public int defenderCasualties;    
    public int attackerDiceRoll,defenderDiceRoll;
    public List<WeightedChoice> diceRolls = new List<WeightedChoice>();
    public int WarID;
    public bool active = false;
    public int battleLength;
    public int attackPhases;
    BattleUI bui;

    public static int GetEstimatedBattleLength(float attackerStrength,float defenderStrength)
    {
        int time = (int)(50 * Mathf.Min(attackerStrength / defenderStrength, defenderStrength / attackerStrength));
        return time;
    }
    public List<Army> GetInvolvedArmies()
    {
        List<Army> involved = new List<Army>(); 
        involved.AddRange(attackingArmies); 
        involved.AddRange(defendingArmies);
        return involved;
    }
    public int DefenderDiceRollBonus()
    {
        int diceRoll = 0;
        if (!DefenderRebels)
        {
            diceRoll += (int)defenderCiv.defenderDiceRoll.value;
        }
        return diceRoll;
    }
    public int AttackerDiceRollBonus()
    {
        int diceRoll = 0;
        if (!AttackerRebels)
        {
            diceRoll += (int)attackerCiv.attackerDiceRoll.value;
        }
        return diceRoll +tile.terrain.attackerDiceRoll + (int)tile.localAttackerDiceRoll.value;
    }
    public bool Involving(int civID)
    {
        if((!AttackerRebels && attackerCiv.CivID == civID) || (!DefenderRebels && defenderCiv.CivID == civID)) { return true; }
        return false;
    }
    public Battle(Vector3Int Pos, Army attacker, Army defender, int warID = -1)
    {
        active = true;
        Game.main.ongoingBattles.Add(this);
        Game.main.hourTick.AddListener(HourTick);
        bui = GameObject.Instantiate(UIManager.main.BattleUIPrefab,Map.main.tileMapManager.tilemap.CellToWorld(Pos),Quaternion.identity,UIManager.main.worldCanvas).GetComponent<BattleUI>();
        bui.battle = this;
        UIManager.main.WorldSpaceUI.Add(bui.gameObject);
        pos = Pos;
        attackingReserves.AddRange(attacker.regiments);
        if (attacker.civID > -1)
        {
            attackerCiv = Game.main.civs[attacker.civID];
        }
        else
        {
            AttackerRebels = true;
            attackerRebelStats = RebelArmyStats.GetRebelStats(attacker);
        }
        attacker.EnterBattle();
        defendingReserves.AddRange(defender.regiments);
        if (defender.civID > -1)
        {
            defenderCiv = Game.main.civs[defender.civID];
        }
        else
        {
            DefenderRebels = true;
            defenderRebelStats = RebelArmyStats.GetRebelStats(defender);
        }
        defender.EnterBattle();
        Map.main.GetTile(pos)._battle = this;
        WarID = warID;
        for(int i = 1; i < 11; i++)
        {
            diceRolls.Add(new WeightedChoice(i, (int)Mathf.Pow(6 - Mathf.Abs(i - 5),2)));
        }
        attackingFrontLine = new BattleLine(AttackerRebels ? attackerRebelStats.combatWidth :(int)attackerCiv.combatWidth.value);
        defendingFrontLine = new BattleLine(DefenderRebels ? defenderRebelStats.combatWidth:(int)defenderCiv.combatWidth.value );
        PhaseTick();
        attackingArmies.Add(attacker);
        attackerCount += (int)attacker.ArmySize();
        defendingArmies.Add(defender);
        defenderCount += (int)defender.ArmySize();
        battleLength = 0;
        attackPhases = 0;
        CheckGeneral(true, attacker);
        CheckGeneral(false, defender);

    }
    void CheckGeneral(bool attacker, Army army)
    {
        General current = attacker ? attackerGeneral : defenderGeneral;
        if (army.general != null && army.general.active)
        {
            if (current != null && current.active)
            {
                if (current.siegeSkill < army.general.siegeSkill)
                {
                    current = army.general;
                }
            }
            else
            {
                current = army.general;
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
    public void AddToBattle(Army army, bool isAttacker)
    {
        if (isAttacker)
        {
            attackingArmies.Add(army);
            attackerCount += (int)army.ArmySize();
            attackingReserves.AddRange(army.regiments);
            army.EnterBattle();
            CheckGeneral(true, army);
        }
        else
        {
            defendingArmies.Add(army);
            defenderCount += (int)army.ArmySize();
            defendingReserves.AddRange(army.regiments);
            army.EnterBattle();
            CheckGeneral(false, army);
        }
    }
    void BattleEnd(bool attackerWin = true, bool wipe = false)
    {
        UIManager.main.WorldSpaceUI.Remove(bui.gameObject);
        Civilisation winnerCiv = attackerWin ? attackerCiv : defenderCiv;
        Civilisation loserCiv = attackerWin ? defenderCiv : attackerCiv;
        bool winnerRebels = attackerWin ? AttackerRebels : DefenderRebels;
        bool loserRebels = attackerWin ? DefenderRebels : AttackerRebels;
        int winnerCasualties = attackerWin ? attackerCasualties : defenderCasualties;
        int loserCasualties = attackerWin ? defenderCasualties : attackerCasualties;        
        List<Army> winners = attackerWin ? attackingArmies : defendingArmies;
        List<Army> losers = attackerWin ? defendingArmies : attackingArmies;
        foreach (var winner in winners)
        {
            winner.ExitBattle();
            winner.WinBattleMorale();
        }
        if (!wipe && !loserRebels)
        {
            foreach (var loser in losers)
            {
                loser.ExitBattle();
                if (loser != null)
                {
                    loser.SetRetreat(loser.RetreatProvince());
                }
            }
        }
        if (!winnerRebels)
        {
            winnerCiv.AddPrestige(Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f), 2f) *(1f + winnerCiv.battlePrestige.value));
            winnerCiv.AddArmyTradition(Mathf.Min((winnerCasualties + 1f) / (winnerCiv.forceLimit.value * 1000f + 1f) * 12f , 5f) * (1f + winnerCiv.battleTraditon.value));
        }
        if (!loserRebels)
        {
            loserCiv.AddPrestige(-Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f), 2f) / 2f);
            loserCiv.AddArmyTradition(Mathf.Min((loserCasualties + 1f) / (loserCiv.forceLimit.value * 1000f + 1f) * 12f, 2f) * (1f + loserCiv.battleTraditon.value));
        }
        if (WarID > -1)
        {
            float warScore = Mathf.Min((loserCasualties + 1f) / (winnerCasualties + 1f) * 6f,10f);   
            if(winnerCiv == Game.main.ongoingWars[WarID].defenderCiv) { warScore *= -1f; }
            UpdateWar(warScore);
        }
        Game.main.ongoingBattles.Remove(this);
        Game.main.hourTick.RemoveListener(HourTick);
        if (Involving(Player.myPlayer.myCivID))
        {
            BattleResultUI brui = GameObject.Instantiate(UIManager.main.battleResultPrefab, UIManager.main.transform).GetComponent<BattleResultUI>();
            UIManager.main.UI.Add(brui.gameObject);
            string attackerName = AttackerRebels ? "Rebels" : attackerCiv.civName;
            string defenderName = DefenderRebels ? "Rebels" : defenderCiv.civName;
            bool win = attackerWin ? (!AttackerRebels && Player.myPlayer.myCivID == winnerCiv.CivID) : (!DefenderRebels && Player.myPlayer.myCivID == winnerCiv.CivID);
            brui.SetUpText(win? "Victory at " + tile.Name : "Defeat at " + tile.Name ,attackerName, defenderName, attackerCount, defenderCount, attackerCasualties, defenderCasualties, pos);
        }
        if (Map.main.GetTile(pos)._battle == this)
        {
            Map.main.GetTile(pos)._battle = null;
        }
        active = false;
    }
    void UpdateWar(float warScoreForAttacker)
    {
        Game.main.ongoingWars[WarID].AddBattle(warScoreForAttacker);
    }
    bool CheckInstakill(bool attacker)
    {
        float morale = AverageMorale(attacker);
        int sideA = GetSideSize(attacker);
        int sideB = GetSideSize(!attacker);
        if (morale <= 0.5 || sideB >= sideA * 10)
        {
            List<Army> losers = attacker ? attackingArmies : defendingArmies;
            foreach (var loser in losers)
            {
                if (loser != null)
                {
                    loser.OnExitTile();
                    if (attacker)
                    {
                        attackerCasualties += (int)loser.ArmySize();
                    }
                    else
                    {
                        defenderCasualties += (int)loser.ArmySize();
                    }
                    GameObject.Destroy(loser.gameObject);
                }
            }
            BattleEnd(!attacker, true);
            return true;
        }
        return false;
    }
    bool TryKillRebels(bool attacker)
    {
        if (attacker ? AttackerRebels : DefenderRebels)
        {
            List<Army> losers = attacker ? attackingArmies : defendingArmies;
            foreach (var loser in losers)
            {
                loser.OnExitTile();
                if (attacker)
                {
                    attackerCasualties += (int)loser.ArmySize();
                }
                else
                {
                    defenderCasualties += (int)loser.ArmySize();
                }
                GameObject.Destroy(loser.gameObject);
            }
            BattleEnd(!attacker, true);
            return true;
        }
        return false;
    }
    bool CheckRetreat(bool attacker)
    {
        float morale = AverageMorale(attacker);
        if (morale <= 0)
        {
            if (!TryKillRebels(attacker))
            {
                BattleEnd(!attacker, false);
            }
            return true;
        }
        return false;
    }
    bool CheckWipe(bool attacker)
    {
        float morale = AverageMorale(attacker);
        int sideA = GetSideSize(attacker);
        int sideB = GetSideSize(!attacker);
        if(morale <= 0 && sideB >= sideA * 2)
        {
            List<Army> losers = attacker ? attackingArmies : defendingArmies;
            foreach (var loser in losers)
            {
                loser.OnExitTile();
                if (attacker)
                {
                    attackerCasualties += (int)loser.ArmySize();
                }
                else
                {
                    defenderCasualties += (int)loser.ArmySize();
                }
                GameObject.Destroy(loser.gameObject);    
                
            }
            BattleEnd(!attacker, true);
            return true;
        }
        return false;
    }
    void FillBattleLine(List<Regiment> reserves, List<Regiment> retreated, BattleLine line,int targetWidth = -1,bool back = false)
    {
        List<Regiment> used = new List<Regiment>();
        if (back)
        {
            used = line.RefillRegimentsBack(reserves, retreated, targetWidth);
        }
        else
        {
            used = line.RefillRegimentsFront(reserves, retreated, targetWidth);
        }
        used.ForEach(i => reserves.Remove(i));
    }
    public void HourTick()
    {
        if (attackingFrontLine == null) { return; }
        battleLength++;
        if(battleLength % 3 == 0)
        {
            PhaseTick();
        }
        if (attackingReserves.Count > defendingReserves.Count)
        {
            FillBattleLine(defendingReserves, defendingRetreated, defendingFrontLine);
            int targetWidth = defendingFrontLine.GetUsedWidth();
            FillBattleLine(defendingReserves, defendingRetreated, defendingBackLine, targetWidth,true);
            FillBattleLine(attackingReserves, attackingRetreated, attackingFrontLine, targetWidth);
            FillBattleLine(attackingReserves, attackingRetreated, attackingBackLine, targetWidth, true);
        }
        else
        {
            FillBattleLine(attackingReserves, attackingRetreated, attackingFrontLine);
            int targetWidth = attackingFrontLine.GetUsedWidth();
            FillBattleLine(attackingReserves, attackingRetreated, attackingBackLine, targetWidth, true);
            FillBattleLine(defendingReserves, defendingRetreated, defendingFrontLine, targetWidth);
            FillBattleLine(defendingReserves, defendingRetreated, defendingBackLine, targetWidth, true);
        }
        if (battleLength == 1)
        {
            if (CheckInstakill(false)) { return; }
            if (CheckInstakill(true)) { return; }
        }
        BattleLine defendingLineClone = new BattleLine(defendingFrontLine);
        int dCasualties = AttackerAttack();
        int aCasualties = DefenderAttack(defendingLineClone);
        attackerCasualties += aCasualties;
        defenderCasualties += dCasualties;
        UIManager.main.NewCombatText("-" + aCasualties, tile.worldPos() + new Vector3(-0.5f,-0.5f), 2f, true);
        UIManager.main.NewCombatText("-" + dCasualties, tile.worldPos() + new Vector3(0.5f, -0.5f), 2f, true);
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
        float averageAttackerMorale = AverageMaxMorale(true);
        float averageDefenderMorale = AverageMaxMorale(false);
        foreach (var reserve in attackingReserves)
        {
            reserve.TakeReserveMoraleDamage(averageDefenderMorale);
        }
        foreach (var reserve in defendingReserves)
        {
            reserve.TakeReserveMoraleDamage(averageAttackerMorale);
        }
        foreach (var frontLine in attackingFrontLine.regiments)
        {
            if (frontLine != null)
            {
                frontLine.TakeFrontlineMoraleDamage(averageDefenderMorale);
            }
        }
        foreach (var frontLine in defendingFrontLine.regiments)
        {
            if (frontLine != null)
            {
                frontLine.TakeFrontlineMoraleDamage(averageAttackerMorale);
            }
        }
        float armyQ = 0;
        for (int i = 0; i < attackingFrontLine.width; i++)
        {
            if (attackingFrontLine.regiments[i] != null)
            {
                armyQ += attackingFrontLine.regiments[i].size;
            }
        }
        if(armyQ == 0) { if (!TryKillRebels(false)) { BattleEnd(false); } return; }
        armyQ = 0;
        for (int i = 0; i < defendingFrontLine.width; i++)
        {
            if (defendingFrontLine.regiments[i] != null)
            {
                armyQ += defendingFrontLine.regiments[i].size;
            }
        }
        if (armyQ == 0) { if (!TryKillRebels(true)) { BattleEnd(true); } return; }
    }
    public void PhaseTick()
    {
        attackerDiceRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
        defenderDiceRoll = WeightedChoiceManager.getChoice(diceRolls).choiceID;
        attackPhases++;
    }
    int AttackerAttack()
    {
        int casualties = 0;
        for(int i = 0; i < attackingFrontLine.width;i++)
        {
            if (attackingFrontLine.regiments[i] != null && attackingFrontLine.regiments[i].size > 0)
            {
                Regiment attacker = attackingFrontLine.regiments[i];
                float tactics = DefenderRebels ? defenderRebelStats.tactics * defenderRebelStats.discipline: defenderCiv.militaryTactics.value * defenderCiv.discipline.value;
                float discipline = AttackerRebels ? attackerRebelStats.discipline : attackerCiv.discipline.value;
                float combatAbility = AttackerRebels ? attackerRebelStats.units[attacker.type].combatAbility.value : attackerCiv.units[attacker.type].combatAbility.value;
                float baseDamage = AttackerRebels ? attackerRebelStats.units[attacker.type].meleeDamage.value : attackerCiv.units[attacker.type].meleeDamage.value;
                float flankDamage = AttackerRebels ? attackerRebelStats.units[attacker.type].flankingDamage.value : attackerCiv.units[attacker.type].flankingDamage.value;
                int flankingRange = AttackerRebels ? attackerRebelStats.flankingRange : attackerCiv.units[attacker.type].flankingRange; 
                int target = FindTarget(defendingFrontLine, i, flankingRange, attacker.type == 1);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (attackerGeneral != null &&attackerGeneral.active) { generalBonus = (target != i) ? attackerGeneral.flankingSkill : attackerGeneral.meleeSkill; }
                    if (target != i) { baseDamage = flankDamage; }
                    Regiment defender = defendingFrontLine.regiments[target];
                    if (defender.size > 0 && defender.type > -1)
                    {
                        float modifiers = Modifiers(attacker.size, battleLength, tactics, baseDamage, discipline: discipline, combatAbility: combatAbility);
                        float damage = BaseCasualties(attackerDiceRoll + generalBonus, AttackerDiceRollBonus()) * modifiers;
                        defender.TakeCasualties((int)damage, AverageMaxMorale(true));
                        casualties += (int)damage;
                    }
                }
                else if (attacker.civID == Player.myPlayer.myCivID)
                {
                    //Debug.Log("Cannot hit from index " + i + " fr: " + flankingRange);
                }
            }
        }
        for (int i = 0; i < attackingBackLine.width; i++)
        {
            if (attackingBackLine.regiments[i] != null && attackingBackLine.regiments[i].size > 0)
            {
                Regiment attacker = attackingBackLine.regiments[i];
                float tactics = DefenderRebels ? defenderRebelStats.tactics * defenderRebelStats.discipline : defenderCiv.militaryTactics.value * defenderCiv.discipline.value;
                float discipline = AttackerRebels ? attackerRebelStats.discipline : attackerCiv.discipline.value;
                float combatAbility = AttackerRebels ? attackerRebelStats.units[attacker.type].combatAbility.value : attackerCiv.units[attacker.type].combatAbility.value;
                float baseDamage = AttackerRebels ? attackerRebelStats.units[attacker.type].rangedDamage.value : attackerCiv.units[attacker.type].rangedDamage.value;
                float flankDamage = AttackerRebels ? attackerRebelStats.units[attacker.type].flankingDamage.value : attackerCiv.units[attacker.type].flankingDamage.value;
                int flankingRange = AttackerRebels ? attackerRebelStats.flankingRange : attackerCiv.units[attacker.type].flankingRange;
                int target = FindTarget(defendingFrontLine, i, flankingRange,false);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (attackerGeneral != null && attackerGeneral.active) { generalBonus = (target != i)? attackerGeneral.flankingSkill: attackerGeneral.rangedSkill; }
                    if (target != i) { baseDamage = flankDamage; }
                    Regiment defender = defendingFrontLine.regiments[target];
                    if (defender.size > 0 && defender.type > -1)
                    {
                        float modifiers = Modifiers(attacker.size, battleLength, tactics, baseDamage, discipline: discipline, combatAbility: combatAbility);
                        float damage = BaseCasualties(attackerDiceRoll+ generalBonus, AttackerDiceRollBonus()) * modifiers;
                        defender.TakeCasualties((int)damage, AverageMaxMorale(true));
                        casualties += (int)damage;
                    }
                }
                else if (attacker.civID == Player.myPlayer.myCivID)
                {
                    //Debug.Log("Cannot hit from index " + i + " fr: " + flankingRange);
                }
            }
        }
        return casualties;
    }
    int FindTarget(BattleLine targetLine,int ownPos,int flankingRange,bool preferFlank = false)
    {
        Regiment defender = new Regiment(-1,-1, -1, -1);
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
                if (targetLine.regiments[index] != null && targetLine.regiments[index].size > 0)
                {
                    return index;
                }
            }
            if (j > 0)
            {
                index = centre - j;
                if (index >= 0 && index < targetLine.width)
                {
                    if (targetLine.regiments[index] != null && targetLine.regiments[index].size > 0)
                    {
                        return index;
                    }
                }
            }
        }
        if (targetLine.regiments[ownPos] != null && targetLine.regiments[ownPos].size > 0)
        {
            return ownPos;
        }
        return -1;
    }
    int DefenderAttack(BattleLine DefendingLineClone)
    {
        int casualties = 0;
        for (int i = 0; i < DefendingLineClone.width; i++)
        {
            if (DefendingLineClone.regiments[i] != null && DefendingLineClone.regiments[i].size > 0)
            {
                Regiment attacker = DefendingLineClone.regiments[i];
                float tactics = AttackerRebels ? attackerRebelStats.tactics * attackerRebelStats.discipline : attackerCiv.militaryTactics.value * attackerCiv.discipline.value;
                float discipline = DefenderRebels ?defenderRebelStats.discipline : defenderCiv.discipline.value;
                float combatAbility = DefenderRebels ? defenderRebelStats.units[attacker.type].combatAbility.value : defenderCiv.units[attacker.type].combatAbility.value;
                float baseDamage = DefenderRebels ? defenderRebelStats.units[attacker.type].meleeDamage.value : defenderCiv.units[attacker.type].meleeDamage.value;
                float flankDamage = DefenderRebels ? defenderRebelStats.units[attacker.type].flankingDamage.value : defenderCiv.units[attacker.type].flankingDamage.value;
                int flankingRange = DefenderRebels ? defenderRebelStats.flankingRange : defenderCiv.units[attacker.type].flankingRange;               
                int target = FindTarget(attackingFrontLine, i, flankingRange, attacker.type == 1);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (defenderGeneral != null && defenderGeneral.active) { generalBonus = (target != i) ? defenderGeneral.flankingSkill : defenderGeneral.meleeSkill; }
                    if (target != i) { baseDamage = flankDamage; }
                    Regiment defender = attackingFrontLine.regiments[target];
                    if (defender.size > 0 && defender.type > -1)
                    {
                        float modifiers = Modifiers(attacker.size, battleLength, tactics, baseDamage, discipline: discipline, combatAbility: combatAbility);
                        float damage = BaseCasualties(defenderDiceRoll + generalBonus, DefenderDiceRollBonus()) * modifiers;
                        defender.TakeCasualties((int)damage, AverageMaxMorale(false));
                        casualties += (int)damage;
                        //if (target != i) { Debug.Log("flanking damage "+ flankDamage+" modifiers "+ modifiers + " damage " + damage); }
                    }
                }
                else if (attacker.civID == Player.myPlayer.myCivID)
                {
                    //Debug.Log("Cannot hit from index " + i + " fr: " + flankingRange);
                }
            }
        }
        for (int i = 0; i < defendingBackLine.width; i++)
        {
            if (defendingBackLine.regiments[i] != null && defendingBackLine.regiments[i].size > 0)
            {
                Regiment attacker = defendingBackLine.regiments[i];
                float tactics = AttackerRebels ? attackerRebelStats.tactics * attackerRebelStats.discipline : attackerCiv.militaryTactics.value * attackerCiv.discipline.value;
                float discipline = DefenderRebels ? defenderRebelStats.discipline : defenderCiv.discipline.value;
                float combatAbility = DefenderRebels ? defenderRebelStats.units[attacker.type].combatAbility.value : defenderCiv.units[attacker.type].combatAbility.value;
                float baseDamage = DefenderRebels ? defenderRebelStats.units[attacker.type].rangedDamage.value : defenderCiv.units[attacker.type].rangedDamage.value;
                float flankDamage = DefenderRebels ? defenderRebelStats.units[attacker.type].flankingDamage.value : defenderCiv.units[attacker.type].flankingDamage.value;
                int flankingRange = DefenderRebels ? defenderRebelStats.flankingRange : defenderCiv.units[attacker.type].flankingRange;
                int target = FindTarget(attackingFrontLine, i, flankingRange, false);
                if (target > -1)
                {
                    int generalBonus = 0;
                    if (defenderGeneral != null && defenderGeneral.active) { generalBonus = (target != i) ? defenderGeneral.flankingSkill : defenderGeneral.rangedSkill; }
                    if (target != i) { baseDamage = flankDamage;}
                    Regiment defender = attackingFrontLine.regiments[target];
                    if (defender.size > 0 && defender.type > -1)
                    {
                        float modifiers = Modifiers(attacker.size, battleLength, tactics, baseDamage, discipline: discipline, combatAbility: combatAbility);
                        float damage = BaseCasualties(defenderDiceRoll + generalBonus, DefenderDiceRollBonus()) * modifiers;
                        defender.TakeCasualties((int)damage, AverageMaxMorale(false));
                        casualties += (int)damage;
                        //if (target != i) { Debug.Log("flanking damage "+ flankDamage+" modifiers "+ modifiers + " damage " + damage); }
                    }
                }
                else if (attacker.civID == Player.myPlayer.myCivID)
                {
                    //Debug.Log("Cannot hit from index " + i + " fr: " + flankingRange);
                }
            }
        }
        return casualties;
    }
    public float BaseCasualties(int diceRoll,int terrain = 0, int diceRollBonus = 0)
    {
        return 15 + 5 * (diceRoll + diceRollBonus + terrain);
    }
    public float Modifiers(int strength, int BattleLength,float milTactics, float baseDamage, float combatAbility = 0, float discipline = 1f)
    {
        return (float)strength / 1000f * baseDamage/milTactics * (1f + combatAbility) * (discipline) * (1f + (float)BattleLength / 100f);
    }
    public float AverageMaxMorale(bool attacker)
    {
        List<Regiment> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        float moraleTotal = 0f;
        float count = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1)
            {
                moraleTotal += regiment.maxMorale;
                count++;
            }
        }
        return moraleTotal / count;
    }
    public float AverageMorale(bool attacker)
    {
        List<Regiment> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        float moraleTotal = 0f;
        float count = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1)
            {
                moraleTotal += regiment.morale;
                count++;
            }
        }
        return moraleTotal / count;
    }
    public int GetSideSize(bool attacker)
    {
        List<Regiment> side = GetSide(attacker);
        if (side.Count == 0) { return 0; }
        int sideSize = 0;
        foreach (var regiment in side)
        {
            if (regiment != null && regiment.type > -1)
            {
                sideSize += regiment.size;
            }
        }
        return sideSize;
    }
    public List<Regiment> GetSide(bool attacker)
    {
        List<Regiment> side = new List<Regiment>();
        if (attacker)
        {
            side.AddRange(attackingFrontLine.regiments);
            side.AddRange(attackingReserves);
            side.AddRange(attackingRetreated);
        }
        else
        {
            side.AddRange(defendingFrontLine.regiments);
            side.AddRange(defendingReserves);
            side.AddRange(defendingRetreated);
        }
        return side;
    }
}
