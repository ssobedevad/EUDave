using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AIMoveArmiesPeace 
{
    public static List<Vector3Int> allyProvinces = new List<Vector3Int>();
    public static void MoveAtPeace(int civID)
    {
        List<Army> armiesToMerge = new List<Army>();
        List<Army> freeArmies = new List<Army>();
        allyProvinces.Clear();
        Civilisation civ = Game.main.civs[civID];
        foreach(var army in civ.armies)
        {
            if (AIMoveArmiesWar.ShouldMergeArmy(civ, army))
            {
                if (!army.isMerging)
                {
                    armiesToMerge.Add(army);
                }
            }
            else
            {
                if (army.exiled && army.path.Count == 0)
                {
                    army.SetPath(army.RetreatProvince());
                }
                else if(!army.exiled && army.path.Count == 0 && !army.tile.underSiege)
                {
                    freeArmies.Add(army);
                }
            }
        }
        if (armiesToMerge.Count > 1)
        {
            Army central = armiesToMerge[0];
            central.isMerging = true;
            for (int i = 1; i < armiesToMerge.Count; i++)
            {
                Army merge = armiesToMerge[i];
                merge.mergeTargets.Add(central);
                central.mergeTargets.Add(merge);
                merge.isMerging = true;
                merge.SetPath(central.pos);
            }
        }
        if(freeArmies.Count > 0)
        {
            AddHomeProvinces(civ);
            MoveArmiesToProvinces(freeArmies);
        }
        if(civ.TotalMaxArmySize()/1000f < civ.forceLimit.value && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.5f && Game.main.Started)
        {
            TileData capitalTile = Map.main.GetTile(civ.SafeProvince());
            if (capitalTile.recruitQueue.Count == 0)
            {
                capitalTile.StartRecruiting(0);
            }
        }
    }
    public static void AddHomeProvinces(Civilisation civ)
    {
        foreach (var province in civ.GetAllCivTiles())
        {
            TileData data = Map.main.GetTile(province);
            if (data.occupied || data.underSiege)
            {
                allyProvinces.Add(province);
            }
        }
    }
    public static void MoveArmiesToProvinces(List<Army> armies)
    {
        List<Vector3Int> provs = new List<Vector3Int>();
        provs.AddRange(allyProvinces);
        for (int i = 0; i < armies.Count; i++)
        {
            Army army = armies[i];
            provs.Sort((x, y) => AIMoveArmiesWar.ProvinceScore(y, army.pos).CompareTo(AIMoveArmiesWar.ProvinceScore(x, army.pos)));
            int loops = 0;
            while (provs.Count > 0 && loops < 100)
            {
                if (!army.SetPath(provs[0]))
                {
                    provs.RemoveAt(0);
                }
                else
                {
                    provs.RemoveAt(0);
                    break;
                }
                loops++;
            }
        }
    }
}
