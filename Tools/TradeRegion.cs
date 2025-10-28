using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TradeRegion
{
    public string name;
    public List<TileData> tiles = new List<TileData>();
    public Dictionary<string,float> resources = new Dictionary<string,float>();
    public float ValuePerDay = 0f;
    public List<int> civs = new List<int>();

    public void AddTile(TileData tile)
    {
        tiles.Add(tile);
    }
    public void Refresh()
    {
        ValuePerDay = 0;
        civs.Clear();
        resources.Clear();
        foreach(var tile in tiles)
        {
            float quantity = tile.GetDailyProductionAmount() * (1f + tile.marketLevel * 0.5f);
            if (resources.ContainsKey(tile.tileResource.name))
            {
                resources[tile.tileResource.name] += quantity;
            }
            else
            {
                resources.Add(tile.tileResource.name, quantity);
            }
            ValuePerDay += quantity * tile.tileResource.Value;
            if (!civs.Contains(tile.civID))
            {
                civs.Add(tile.civID);
            }
        }
        ValuePerDay *= (1f + civs.Count * 0.05f);
    }
    public float GetMarketValue(string resource, Civilisation civ , ref float marketPercent)
    {
        if (!resources.ContainsKey(resource)) { return 0f; }
        float total = resources[resource];
        if(total == 0f) { return 0f; }
        float civInput = 0f;
        foreach(var tile in tiles)
        {
            if(tile.civID == civ.CivID && tile.tileResource.name == resource)
            {
                civInput += tile.GetDailyProductionAmount() * (1f + tile.marketLevel * 0.5f);
            }
        }
        marketPercent = (civInput / total);
        return civInput * marketPercent * Map.main.resourceDict[resource].Value * (1f + civs.Count * civ.tradeValPerCiv.value);
    }
    public float GetTradeIncome(Civilisation civ)
    {
        float income = 0f;
        foreach (var resource in resources.Keys)
        {
            float percent = 0f;
            income += GetMarketValue(resource, civ,ref percent);
        }
        return income;
    }
}
