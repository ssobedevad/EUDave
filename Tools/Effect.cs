using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Effect
{
    public string name;
    public int type;
    public float amount;
    public int duration = -1;
    public bool isProvince = false;

    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        if (type == 4)
        {
            text += name + "\n";
        }
        else
        {
            text += name + " " + Modifier.ToString(amount, civ.GetStat(name),type == 2 || type == 0, type == 3) + "\n";
        }        
        return text;
    }
}
