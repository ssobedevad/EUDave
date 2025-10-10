using System;
using UnityEngine;

[Serializable]
public class Modifier
{
    public float value;
    public int type;
    public int duration;
    public string name;
    public bool isPercentage;
    public Modifier(float Value,int Type, string Name, int Duration = -1, bool isPercentage = false)
    {
        value = Value;
        type = Type;
        name = Name;
        duration = Duration;
        this.isPercentage = isPercentage;
    }
    public static string ToString(float effect,Stat stat,bool forcePercentage = false)
    {
        string val = "";
        string effectString = (stat != null && stat.isFlat && !forcePercentage) ? effect + "" : Mathf.Round(effect * 100f) + "%";
        if (effect > 0)
        {     
            val += " +" + effectString;
        }
        else
        {
            val += " " + effectString;
        }
        return val;
    }
}
public class ModifierType
{
    public static int Additive = 0;
    public static int Flat = 1;
    public static int Multiplicative = 2;
    public static int Base = 3;
}