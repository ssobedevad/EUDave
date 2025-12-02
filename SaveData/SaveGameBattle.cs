using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameBattle
{
    public SaveGameVector3Int p;
    public int ai;
    public int di;
    public General ag;
    public General dg;

    public List<SaveGameArmy> aA;
    public List<SaveGameArmy> dA;

    public List<Regiment> aR;
    public List<Regiment> aT;
    public BattleLine aF;
    public BattleLine aB;
    public int ac;
    public int ad;


    public List<Regiment> dR;
    public List<Regiment> dT;
    public BattleLine dF;
    public BattleLine dB;
    public int dc;
    public int dd;

    public int ar;
    public int dr;
    public int wi;
    public bool a;
    public int bl;
    public int ap;


    public SaveGameBattle()
    {

    }

    public SaveGameBattle(Battle battle)
    {
        p = new SaveGameVector3Int(battle.pos);
        ai = battle.attackerCiv.CivID;
        di = battle.defenderCiv.CivID;
        ag = battle.attackerGeneral;
        dg = battle.defenderGeneral;

        aA = battle.attackingArmies.ConvertAll(i => new SaveGameArmy(i));
        dA = battle.defendingArmies.ConvertAll(i => new SaveGameArmy(i));

        aR = battle.attackingReserves;
        aT = battle.attackingRetreated;
        aF = battle.attackingFrontLine;
        aB = battle.attackingBackLine;
        ac = battle.attackerCount;
        ad = battle.attackerCasualties;

        dR = battle.defendingReserves;
        dT = battle.defendingRetreated;
        dF = battle.defendingFrontLine;
        dB = battle.defendingBackLine;
        dc = battle.defenderCount;
        dd = battle.defenderCasualties;

        ar = battle.attackerDiceRoll;
        dr = battle.defenderDiceRoll;
        wi = battle.WarID;
        a = battle.active;
        bl = battle.battleLength;
        ap = battle.attackPhases;
    }

    public void LoadToBattle()
    {
        Battle battle = new Battle(this);
        battle.attackerCiv = Game.main.civs[ai];
        battle.defenderCiv = Game.main.civs[di];
        battle.attackerGeneral = ag;
        battle.defenderGeneral = dg;

        battle.attackingArmies = aA.ConvertAll(i => i.NewArmy());
        battle.defendingArmies = dA.ConvertAll(i => i.NewArmy());

        battle.attackingReserves = aR;
        battle.attackingRetreated = aT;
        battle.attackingFrontLine = aF;
        battle.attackingBackLine = aB;
        battle.attackerCount = ac;
        battle.attackerCasualties = ad;

        battle.defendingReserves = dR;
        battle.defendingRetreated = dT;
        battle.defendingFrontLine = dF;
        battle.defendingBackLine = dB;
        battle.defenderCount = dc;
        battle.defenderCasualties = dd;

        battle.attackerDiceRoll = ar;
        battle.defenderDiceRoll = dr;
        battle.WarID = wi;
        battle.active = a;
        battle.battleLength = bl;
        battle.attackPhases = ap;
    }
}