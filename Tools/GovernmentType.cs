using System;
using UnityEngine;

[Serializable]
public class GovernmentType
{
    
    public string name;
    public Color c;
    public Sprite sprite;
    public Effect[] effects;
    public GovernmentReformTier[] BaseReforms;

    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for (int i = 0; i < effects.Length; i++)
        {
            text += effects[i].GetHoverText(civ);
        }
        return text;
    }
}
