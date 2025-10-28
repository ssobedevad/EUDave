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
    public bool decay;
    public Modifier(float Value,int Type, string Name, int Duration = -1, bool isPercentage = false,bool decay = false)
    {
        value = Value;
        type = Type;
        name = Name;
        duration = Duration;
        this.isPercentage = isPercentage;
        this.decay = decay;
    }
    public static string ToString(float effect,Stat stat,bool forcePercentage = false)
    {
        string val = "";
        string effectString = (stat != null && stat.isFlat && !forcePercentage) ? Mathf.Round(effect * 100f)/100f + "" : Mathf.Round(effect * 100f) + "%";
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