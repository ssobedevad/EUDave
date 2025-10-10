using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EventOption
{
    public string[] Effect;
    public int[] EffectType;
    public int[] EffectDuration;
    public float[] EffectStrength;
    public bool provinceModifier;
    public int devA,devB,devC;
    public int manaA,manaB,manaC;
    public int stability, prestige;
    public int population;
    public float coins;
    public float coinsYIncomePercent;

}
