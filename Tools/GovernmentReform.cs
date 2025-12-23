using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class GovernmentReform
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public bool isLocked;
    public Condition[] conditions;
    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for(int i = 0; i < effects.Length; i++)
        {
            text += effects[i].GetHoverText(civ);
        }        
        return text;
    }
    public bool CanTake(Civilisation civ)
    {
        foreach (var condition in conditions)
        {
            if (!condition.isMet(civ))
            {
                return false;
            }
        }
        return true;
    }
}
