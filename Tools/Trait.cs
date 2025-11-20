using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Trait
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public int traitType;
    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for(int i = 0; i < effects.Length; i++)
        {
            if (effects[i].type == 4)
            {
                text += effects[i].name + "\n";
            }
            else
            {
                text += effects[i].name + " " + Modifier.ToString(effects[i].amount, civ.GetStat(effects[i].name), effects[i].type == 2 || effects[i].type == 0, effects[i].type == 3) + "\n";
            }
        }        
        return text;
    }
}
