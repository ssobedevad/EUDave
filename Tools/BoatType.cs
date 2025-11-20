using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class BoatType
{
    public string name;
    public Stat cannons = new Stat(0f, "Cannons",true);
    public Stat hullStrength = new Stat(0f, "Hull Strength",true);
    public Stat maxSailors = new Stat(0f, "Max Sailors", true);
    public Stat supply = new Stat(0f, "Supply", true);
    public Stat combatAbility = new Stat(0f, "Combat Ability");
    public int flankingRange;
    public int width;
    public float baseCost;

    public BoatType(string name, float baseCannons,float baseHullStrength, float baseSupply, int FlankingRange, float baseCost, float MaxSailors, int width)
    {
        this.name = name;
        cannons.ChangeBaseStat(baseCannons);
        hullStrength.ChangeBaseStat(baseHullStrength);
        supply.ChangeBaseStat(baseSupply);
        flankingRange = FlankingRange;
        this.baseCost = baseCost;
        maxSailors.ChangeBaseStat(MaxSailors);
        this.width = width;
    }
}
