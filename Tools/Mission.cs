using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Mission
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public Condition[] conditions;

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
    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";

        if (conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                bool met = condition.isMet(civ);
                text += condition.ToString(civ) + " (" + met + ")\n";
            }
        }
        text += "\nRewards:\n\n";
        for (int i = 0; i < effects.Length; i++)
        {
            text += effects[i].GetHoverText(civ) + DurationText(effects[i].duration) + "\n";
        }
        return text;
    }
    string DurationText(int duration)
    {
        if (duration > 0)
        {
            return "For " + Mathf.Round(duration * 10f / 60f) / 10f + " days";
        }
        else
        {
            return "Until the End of the Game";
        }
    }
}
