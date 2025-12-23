using System;
using System.Collections.Generic;

using UnityEngine;
 
public class UnitType
{
    public string name;
    public Stat baseDamage = new Stat(0f, "Base Damage",true);
    public int flankingRange;
    public Stat baseCost = new Stat(0f, "Base Cost",true);

    public UnitType(string name, float baseDamage, int FlankingRange, float baseCost,Civilisation civ)
    {
        this.name = name;
        this.baseDamage.ChangeBaseStat(baseDamage);
        flankingRange = FlankingRange;
        this.baseCost.ChangeBaseStat(baseCost);
        if (!civ.stats.ContainsKey(name + " Damage"))
        {
            civ.stats.Add(name + " Damage", this.baseDamage);
        }
        else
        {
            this.baseDamage = civ.stats[name + " Damage"];
        }
        if (!civ.stats.ContainsKey(name + " Cost"))
        {
            civ.stats.Add(name + " Cost", this.baseCost);
        }
        else
        {
            this.baseCost = civ.stats[name + " Cost"];
        }
    }
}
