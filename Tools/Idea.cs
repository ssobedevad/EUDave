using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Idea
{
    public string name;
    public Sprite icon;
    public string[] effect;
    public int[] type;
    public float[] effectStrength;

    public string GetHoverText(Civilisation civ)
    {
        string text = name + ":\n";
        for(int i = 0; i < effect.Length; i++)
        {
            text += effect[i] +" "+ Modifier.ToString(effectStrength[i], civ.GetStat(effect[i])) + "\n";
        }        
        return text;
    }
}
