using MessagePack;
using System;
using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class Modifier
{
    public float v;
    public int t;
    public int d;
    public string n;
    public bool p;
    public bool dc;
    public Modifier()
    {

    }
    public Modifier(float Value,int Type, string Name, int Duration = -1, bool isPercentage = false,bool decay = false)
    {
        v = Value;
        t = Type;
        n = Name;
        d = Duration;
        this.p = isPercentage;
        this.dc = decay;
    }
    public static string ToString(float effect,Stat stat,bool forcePercentage = false,bool forceFlat = false)
    {
        string val = "";
        string effectString = ((forceFlat && !forcePercentage) || (forceFlat == false && forcePercentage == false && stat != null && stat.f)) ? Mathf.Round(effect * 100f)/100f + "" : Mathf.Round(effect * 100f) + "%";
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