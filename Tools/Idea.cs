using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Idea
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for(int i = 0; i < effects.Length; i++)
        {
            text += effects[i].GetHoverText(civ);
        }        
        return text;
    }
}
