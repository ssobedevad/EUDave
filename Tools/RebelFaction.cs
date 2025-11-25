using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class RebelFaction
{
    public List<Vector3Int> provinces = new List<Vector3Int>();
    public int size;
    public float uprisingProgress;
    public float bonusTactics;
    public float totalUnrest;
    public int type;
    public int demandID;

    public RebelFaction(Vector3Int start)
    {
        provinces.Add(start);
        uprisingProgress = 0f;
    }
    public RebelFaction(List<Vector3Int> provinces, int size, float uprisingProgress, float bonusTactics)
    {
        this.provinces = provinces;
        this.size = size;
        this.uprisingProgress = uprisingProgress;
        this.bonusTactics = bonusTactics;
    }

    public void Update()
    {
        List<TileData> tiles = provinces.ConvertAll(i => Map.main.GetTile(i));
        if (tiles.Count == 0) { return; }
        TileData central = tiles[0];
        size = 0;
        totalUnrest = 0;
        type = 0;
        foreach (var tile in tiles.ToList())
        {
            if(tile.unrest < 0) { tiles.Remove(tile);continue; }
            if (tile.totalDev > central.totalDev)
            {
                central = tile;
            }
            size += (int)Mathf.Min(tile.population - tile.avaliablePopulation,tile.totalDev * 100);
            totalUnrest += tile.unrest;
        }
        if (central.cores[0] != central.civID && central.seperatism > 0)
        {
            type = 1;
            demandID = central.cores[0];
        }
        else if(central.religion != central.civ.religion)
        {
            type = 2;
            demandID = central.religion;
        }
        if (totalUnrest <= 0)
        {
            provinces.Clear();
            uprisingProgress = 0;
        }
        else if (size >= 1000)
        {
            uprisingProgress += Mathf.Min(totalUnrest/provinces.Count,10f);
            if(uprisingProgress >= 100)
            {
                Revolt();
            }
            else if(uprisingProgress >= 50 && central.civID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.rebelFaction;
                notification.description = "" + size + " rebels will take arms against you";
                notification.province = central;
                NotificationsUI.AddNotification(notification);
            }
        }
    }
    public void Revolt()
    {
        List<TileData> tiles = provinces.ConvertAll(i => Map.main.GetTile(i));
        if(tiles.Count == 0 ) { return; }
        TileData central = tiles[0];
        RebelArmyStats stats = new RebelArmyStats(central.civ.militaryTactics.value + bonusTactics, central.civ.moraleMax.value, central.civ.discipline.value,(int)central.civ.combatWidth.value,central.civ.units.ToArray(),rebelType:type);
        stats.rebelDemandsID = demandID;
        stats.rebelType = type;
        size = 0;
        foreach(var tile in tiles.ToList())
        {
            if (tile.unrest < 0) { tiles.Remove(tile); continue; }
            if (tile.totalDev > central.totalDev)
            {
                central = tile;
            }
            tile.localUnrest.AddModifier(new Modifier(-100f, ModifierType.Flat, "Recent Uprising", 25920));
            size += (int)Mathf.Min(tile.population - tile.avaliablePopulation, tile.totalDev * 100);
            tile.population -= (int)Mathf.Min(tile.population - tile.avaliablePopulation, tile.totalDev * 100);
        }
        if(size > 1000)
        {
            central.CreateRebelArmy(size,stats);
        }
        uprisingProgress = 0;
    }
}
