using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Trait
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public int traitType;

    public Trait()
    {

    }
    public Trait(Trait clone)
    {
        name = clone.name;
        icon = clone.icon;
        effects = clone.effects;
        traitType = clone.traitType;
    }
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
