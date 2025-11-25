using System;
using System.Collections.Generic;

using UnityEngine;
 
public class UnitType
{
    public string name;
    public Stat meleeDamage = new Stat(0f, "Melee Damage",true);
    public Stat flankingDamage = new Stat(0f, "Flanking Damage",true);
    public Stat rangedDamage = new Stat(0f, "Ranged Damage", true);
    public Stat combatAbility = new Stat(0f, "Combat Ability");
    public int flankingRange;
    public float baseCost;

    public UnitType(string name, float baseMelee,float baseFlanking, float baseRanged, int FlankingRange, float baseCost)
    {
        this.name = name;
        meleeDamage.ChangeBaseStat(baseMelee);
        flankingDamage.ChangeBaseStat(baseFlanking);
        rangedDamage.ChangeBaseStat(baseRanged);
        flankingRange = FlankingRange;
        this.baseCost = baseCost;
    }
}
