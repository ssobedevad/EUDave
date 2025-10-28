using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class MercenaryGroup
{
    public string Name;
    public int baseRegiments;
    public float cavalryPercent;
    public int regimentsPerYearExtra;
    public float costPerRegiment;
    public int requiredReligion;
    public int requiredGovernmentType;
}
