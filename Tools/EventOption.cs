using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EventOption
{
    public Effect[] effects;
    public bool provinceModifier;
    public int devA,devB,devC;
    public int manaA,manaB,manaC;
    public int stability, prestige,govReformProgress;
    public int population;
    public float coins;
    public float coinsYIncomePercent;

}
