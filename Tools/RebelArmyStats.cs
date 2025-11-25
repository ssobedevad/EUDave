using System;
using System.Collections.Generic;

using UnityEngine;

public class RebelArmyStats
{
    public float tactics;
    public float morale;
    public float discipline;
    public int flankingRange = 1;
    public int combatWidth;
    public UnitType[] units;
    public int rebelType;
    public int rebelDemandsID;
    public RebelArmyStats(float tactics, float morale, float discipline,int combatWidth, UnitType[] units, int rebelType)
    {
        this.tactics = tactics;
        this.morale = morale;
        this.discipline = discipline;
        this.combatWidth = combatWidth;
        this.units = units;
        this.rebelType = rebelType;
    }
    public static RebelArmyStats GetRebelStats(Army rebels)
    {
        if (!Game.main.rebelFactions.Contains(rebels)) { return null; }
        return Game.main.rebelStats[Game.main.rebelFactions.IndexOf(rebels)];       
    }
}
