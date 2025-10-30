using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Tech
{
    public Age expectedDate;
    public string Name;
    public string Description;
    public string[] unlock;
    public string[] effect;
    public int[] effectType;
    public float[] effectStrength;
    public Effect[] effects;
    public int type;

    public void TakeTech(int civID)
    {
        Civilisation civilisation = Game.main.civs[civID];        
        for (int i = 0; i < effect.Length; i++)
        {
            civilisation.ApplyCivModifier(effect[i], effectStrength[i], Name, effectType[i]);
        }
        foreach (var unlocked in unlock)
        {
            if (Map.main.Buildings.ToList().Exists(i => i.Name == unlocked))
            {
                int index = Map.main.Buildings.ToList().FindIndex(i => i.Name == unlocked);
                if (!civilisation.unlockedBuildings.Contains(index))
                {
                    civilisation.unlockedBuildings.Add(index);
                }
            }
            else if (unlocked.Contains("Ideas"))
            {
                civilisation.unlockedIdeaGroupSlots++;
            }
            civilisation.techUnlocks.Add(unlocked);
        }
        int ahead = TechnologyUI.GetAheadTime(civilisation, type);
        if (ahead > 0)
        {
            string aeffect = type == 0 ? "Tax Efficiency" : type == 1 ? "Production Value" : "Population Growth";
            civilisation.ApplyCivModifier(aeffect, 0.2f, "Ahead of Time",1, ahead);
        }
    }
}
