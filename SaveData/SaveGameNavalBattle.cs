using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameNavalBattle
{
    public SaveGameVector3Int pos;
    public int attackerCivID;
    public int defenderCivID;
    public General attackerGeneral;
    public General defenderGeneral;

    public List<SaveGameFleet> attackingFleets;
    public List<SaveGameFleet> defendingFleets;

    public List<Boat> attackingReserves;
    public List<Boat> attackingRetreated;
    public FleetBattleLine attackingFrontLine;
    public int attackerCount;
    public int attackerCasualties;


    public List<Boat> defendingReserves;
    public List<Boat> defendingRetreated;
    public FleetBattleLine defendingFrontLine;
    public int defenderCount;
    public int defenderCasualties;

    public int attackerDiceRoll;
    public int defenderDiceRoll;
    public int WarID;
    public bool active;
    public int battleLength;
    public int attackPhases;


    public SaveGameNavalBattle()
    {

    }

    public SaveGameNavalBattle(NavalBattle battle)
    {
        pos = new SaveGameVector3Int(battle.pos);
        attackerCivID = battle.attackerCiv.CivID;
        defenderCivID = battle.defenderCiv.CivID;
        attackerGeneral = battle.attackerGeneral;
        defenderGeneral = battle.defenderGeneral;

        attackingFleets = battle.attackingArmies.ConvertAll(i => new SaveGameFleet(i));
        defendingFleets = battle.defendingArmies.ConvertAll(i => new SaveGameFleet(i));

        attackingReserves = battle.attackingReserves;
        attackingRetreated = battle.attackingRetreated;
        attackingFrontLine = battle.attackingFrontLine;
        attackerCount = battle.attackerCount;
        attackerCasualties = battle.attackerCasualties;

        defendingReserves = battle.defendingReserves;
        defendingRetreated = battle.defendingRetreated;
        defendingFrontLine = battle.defendingFrontLine;
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
        NavalBattle battle = new NavalBattle(this);
        battle.attackerCiv = Game.main.civs[attackerCivID];
        battle.defenderCiv = Game.main.civs[defenderCivID];
        battle.attackerGeneral = attackerGeneral;
        battle.defenderGeneral = defenderGeneral;

        battle.attackingArmies = attackingFleets.ConvertAll(i => i.NewFleet());
        battle.defendingArmies = defendingFleets.ConvertAll(i => i.NewFleet());

        battle.attackingReserves = attackingReserves;
        battle.attackingRetreated = attackingRetreated;
        battle.attackingFrontLine = attackingFrontLine;
        battle.attackerCount = attackerCount;
        battle.attackerCasualties = attackerCasualties;

        battle.defendingReserves = defendingReserves;
        battle.defendingRetreated = defendingRetreated;
        battle.defendingFrontLine = defendingFrontLine;
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