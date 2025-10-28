﻿using System;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            text += name + " " + Modifier.ToString(amount, civ.GetStat(name)) + "\n";
        }        
        return text;
    }
}
