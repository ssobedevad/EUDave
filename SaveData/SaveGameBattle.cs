using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public class SaveGameBattle
{
    public Vector3Int pos;
    public int attackerCivID;
    public int defenderCivID;
    public General attackerGeneral;
    public General defenderGeneral;

    public List<SaveGameArmy> attackingArmies;
    public List<SaveGameArmy> defendingArmies;

    public List<Regiment> attackingReserves;
    public List<Regiment> attackingRetreated;
    public BattleLine attackingFrontLine;
    public BattleLine attackingBackLine;
    public int attackerCount;
    public int attackerCasualties;


    public List<Regiment> defendingReserves;
    public List<Regiment> defendingRetreated;
    public BattleLine defendingFrontLine;
    public BattleLine defendingBackLine;
    public int defenderCount;
    public int defenderCasualties;

    public int attackerDiceRoll;
    public int defenderDiceRoll;
    public int WarID;
    public bool active;
    public int battleLength;
    public int attackPhases;


    public SaveGameBattle()
    {

    }

    public SaveGameBattle(Battle battle)
    {
        pos = battle.pos;
        attackerCivID = battle.attackerCiv.CivID;
        defenderCivID = battle.defenderCiv.CivID;
        attackerGeneral = battle.attackerGeneral;
        defenderGeneral = battle.defenderGeneral;

        attackingArmies = battle.attackingArmies.ConvertAll(i => new SaveGameArmy(i));
        defendingArmies = battle.defendingArmies.ConvertAll(i => new SaveGameArmy(i));

        attackingReserves = battle.attackingReserves;
        attackingRetreated = battle.attackingRetreated;
        attackingFrontLine = battle.attackingFrontLine;
        attackingBackLine = battle.attackingBackLine;
        attackerCount = battle.attackerCount;
        attackerCasualties = battle.attackerCasualties;

        defendingReserves = battle.defendingReserves;
        defendingRetreated = battle.defendingRetreated;
        defendingFrontLine = battle.defendingFrontLine;
        defendingBackLine = battle.defendingBackLine;
        defenderCount = battle.defenderCount;
        defenderCasualties = battle.defenderCasualties;

        attackerDiceRoll = battle.attackerDiceRoll;
        defenderDiceRoll = battle.defenderDiceRoll;
        WarID = battle.WarID;
        active = battle.active;
        battleLength = battle.battleLength;
        attackPhases = battle.attackPhases;
    }

    public void LoadToBattle()
    {
        Battle battle = new Battle(this);
        battle.attackerCiv = Game.main.civs[attackerCivID];
        battle.defenderCiv = Game.main.civs[defenderCivID];
        battle.attackerGeneral = attackerGeneral;
        battle.defenderGeneral = defenderGeneral;

        battle.attackingArmies = attackingArmies.ConvertAll(i => i.NewArmy());
        battle.defendingArmies = defendingArmies.ConvertAll(i => i.NewArmy());

        battle.attackingReserves = attackingReserves;
        battle.attackingRetreated = attackingRetreated;
        battle.attackingFrontLine = attackingFrontLine;
        battle.attackingBackLine = attackingBackLine;
        battle.attackerCount = attackerCount;
        battle.attackerCasualties = attackerCasualties;

        battle.defendingReserves = defendingReserves;
        battle.defendingRetreated = defendingRetreated;
        battle.defendingFrontLine = defendingFrontLine;
        battle.defendingBackLine = defendingBackLine;
        battle.defenderCount = defenderCount;
        battle.defenderCasualties = defenderCasualties;

        battle.attackerDiceRoll = attackerDiceRoll;
        battle.defenderDiceRoll = defenderDiceRoll;
        battle.WarID = WarID;
        battle.active = active;
        battle.battleLength = battleLength;
        battle.attackPhases = attackPhases;
    }
}