using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIMoveFleetsPeace 
{
    public static List<TileData> allyCoastalProvinces = new List<TileData>();
    public static void MoveAtPeace(int civID)
    {
        List<Fleet> fleetsToMerge = new List<Fleet>();
        List<Fleet> freeFleets = new List<Fleet>();
        allyCoastalProvinces.Clear();
        Civilisation civ = Game.main.civs[civID];
        if(civ.TotalMaxArmySize()/1000f >= civ.forceLimit.value - 1 && civ.fleets.Count < 2)
        {
            if(civ.civCoastalTiles.Count > 0)
            {
                TileData boatTile = civ.civCoastalTiles[0];
                float cost = boatTile.GetRecruitCost(0);
                if (civ.coins >= cost)
                {
                    boatTile.StartRecruitingBoat(0);
                }              
            }
        }
        foreach(var fleet in civ.fleets)
        {
            if (AIMoveFleetsWar.ShouldMergeFleet(civ, fleet))
            {
                if (!fleet.isMerging)
                {
                    fleetsToMerge.Add(fleet);
                }
            }
            else
            {
                if (fleet.exiled && fleet.path.Count == 0)
                {
                    fleet.SetPath(fleet.RetreatProvince());
                }
                else if(!fleet.exiled && fleet.path.Count == 0 && !fleet.tile.underSiege)
                {
                    freeFleets.Add(fleet);
                }
            }
        }
        if (fleetsToMerge.Count > 1)
        {
            Fleet central = fleetsToMerge[0];
            int total = central.CombatWidth();
            if (total < 31)
            {
                central.isMerging = true;
                for (int i = 1; i < fleetsToMerge.Count; i++)
                {
                    Fleet fleet = fleetsToMerge[i];
                    if (total > 31)
                    {
                        break;
                    }
                    fleet.mergeTargets.Add(central);
                    central.mergeTargets.Add(fleet);
                    fleet.isMerging = true;
                    fleet.SetPath(central.pos);
                    total += fleet.CombatWidth();
                }
            }
        }
        else
        {
            freeFleets.AddRange(fleetsToMerge);
            fleetsToMerge.Clear();
        }
        if(freeFleets.Count > 0)
        {
            AddHomeProvinces(civ);
            allyCoastalProvinces.Sort((x, y) => y.totalDev.CompareTo(x.totalDev));
            if (allyCoastalProvinces.Count > 0)
            {
                freeFleets.ForEach(i => i.SetPath(allyCoastalProvinces[0].pos));
            }
        }
        //if(civ.TotalMaxArmySize()/1000f < (civ.forceLimit.value -1) && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.5f && Game.main.Started)
        //{
        //    TileData capitalTile = Map.main.GetTile(civ.SafeProvince());
        //    if (capitalTile.recruitQueue.Count == 0)
        //    {
        //        capitalTile.StartRecruiting(0);
        //    }
        //}
    }
    public static void AddHomeProvinces(Civilisation civ)
    {
        foreach (var province in civ.civCoastalTiles.ToList())
        {
            if (!province.occupied || !province.underSiege)
            {
                allyCoastalProvinces.Add(province);
            }
        }
    }    
}
