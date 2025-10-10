using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RebelArmyStats
{
    public float tactics;
    public float morale;
    public float discipline;
    public float meleeDamage;
    public int flankingRange = 1;
    public int combatWidth;
    public float ICA, FCA, SCA;
    public int rebelType;
    public RebelArmyStats(float tactics, float morale, float discipline, float meleeDamage, int combatWidth,float ICA, float FCA, float SCA, int rebelType)
    {
        this.tactics = tactics;
        this.morale = morale;
        this.discipline = discipline;
        this.meleeDamage = meleeDamage;
        this.combatWidth = combatWidth;
        this.ICA = ICA;
        this.FCA = FCA;
        this.SCA = SCA;
        this.rebelType = rebelType;
    }
}
