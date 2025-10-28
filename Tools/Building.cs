using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Building
{
    public string Name;
    public Effect effects;
    public float baseCost;
    public float baseTime;
    public int fortLevel;

    public float GetCost(TileData tileData,Civilisation civ)
    {
        return Mathf.Max(baseCost * (1f + tileData.localConstructionCost.value + civ.constructionCost.value),1f);
    }
    public float GetTime(TileData tileData, Civilisation civ)
    {
        return Mathf.Max(baseTime * (1f + tileData.localConstructionTime.value + civ.constructionTime.value),1f);
    }
}
