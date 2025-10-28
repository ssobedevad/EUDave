using System;
using UnityEngine;

[Serializable]
public class Religion
{    
    public string name;
    public Color c;
    public Sprite sprite;
    public Effect[] effects;
    public Effect[] religiousMechanicEffects;

    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for (int i = 0; i < effects.Length; i++)
        {
            text += effects[i].name + " " + Modifier.ToString(effects[i].amount, civ.GetStat(effects[i].name)) + "\n";
        }
        return text;
    }
}
