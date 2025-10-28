using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GreatProject
{
    public string Name;
    public TileData tile;
    public Sprite icon;
    public Condition[] conditions;
    public Effect[] T1, T2, T3;
    public float baseCost;
    public float baseTime;
    public int tier;
    public int buildTimer;
    public bool isBuilding;
    public GreatProject() 
    {
        
    }
    public void RemoveProject(Civilisation civ)
    {
        Effect[] effect = tier == 1 ? T1 : tier == 2 ? T2 : tier == 3 ? T3 : new Effect[0];        
        for (int i = 0; i < effect.Length; i++)
        {
            if (!effect[i].isProvince)
            {
                civ.RemoveCivModifier(effect[i].name, Name);
            }
            else
            {
                tile.RemoveTileLocalModifier(effect[i].name, Name);
            }
        }
    }
    public bool CanUse(Civilisation civ)
    {
        if (conditions.Length > 0)
        {
            foreach (var condition in conditions)
            {
                bool met = condition.isMet(civ);
                if (!met) { return false; }               
            }
        }
        return true;
    }
    public void AddProject(Civilisation civ)
    {
        Effect[] effect = tier == 1 ? T1 : tier == 2 ? T2 : tier == 3 ? T3 : new Effect[0];
        for (int i = 0; i < effect.Length; i++)
        {          
            if (!effect[i].isProvince)
            {
                civ.ApplyCivModifier(effect[i].name, effect[i].amount, Name, effect[i].type);
            }
            else
            {
                tile.ApplyTileLocalModifier(effect[i].name, effect[i].amount, effect[i].type, Name);
            }
        }
    }
    public void Upgrade(Civilisation civ)
    {
        RemoveProject(civ);
        tier++;
        AddProject(civ);
    }
    public float GetCost(TileData tileData,Civilisation civ)
    {
        return Mathf.Max(baseCost * (tier + 1f) * (1f + tileData.localConstructionCost.value + civ.constructionCost.value),1f);
    }
    public float GetTime(TileData tileData, Civilisation civ)
    {
        return Mathf.Max(baseTime * (tier + 1f) * (1f + tileData.localConstructionTime.value + civ.constructionTime.value),1f);
    }
    public string GetHoverTextT1(Civilisation civ)
    {
        string text = Name + " T1:\n";
        for (int i = 0; i < T1.Length; i++)
        {
            if (T1[i].type == 4)
            {
                text += (T1[i].isProvince ? "Province: " : "") + T1[i].name + "\n";
            }
            else
            {
                text += (T1[i].isProvince ? "Province: " : "" )+ T1[i].name + " " + Modifier.ToString(T1[i].amount, civ.GetStat(T1[i].name)) + "\n";
            }
        }
        return text;
    }
    public string GetHoverTextT2(Civilisation civ)
    {
        string text = Name + " T2:\n";
        for (int i = 0; i < T2.Length; i++)
        {
            if (T2[i].type == 4)
            {
                text += (T2[i].isProvince ? "Province: " : "") + T2[i].name + "\n";
            }
            else
            {
                text += (T2[i].isProvince ? "Province: " : "") + T2[i].name + " " + Modifier.ToString(T2[i].amount, civ.GetStat(T2[i].name)) + "\n";
            }
        }
        return text;
    }
    public string GetHoverTextT3(Civilisation civ)
    {
        string text = Name + " T3:\n";
        for (int i = 0; i < T3.Length; i++)
        {
            if (T3[i].type == 4)
            {
                text += (T3[i].isProvince ? "Province: " : "") + T3[i].name + "\n";
            }
            else
            {
                text += (T3[i].isProvince ? "Province: " : "") + T3[i].name + " " + Modifier.ToString(T3[i].amount, civ.GetStat(T3[i].name)) + "\n";
            }
        }
        return text;
    }
}
