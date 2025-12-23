using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
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
        if(civ.militaryAccess.Count > 0)
        {
            foreach(var civid in civ.militaryAccess.ToList())
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Game.main.multiplayerManager.CivRequestAccessRpc(civ.CivID, civid, true);
                    }
                }
                else
                {
                    civ.RemoveAccess(civid);
                }

            }
        }
        foreach(var army in civ.armies)
        {
            if (army.exiled && army.path.Count == 0)
            {
                army.SetPath(army.RetreatProvince());
            }
            else if (AIMoveArmiesWar.ShouldMergeArmy(civ, army))
            {
                if (!army.isMerging)
                {
                    armiesToMerge.Add(army);
                }
            }
            else
            {
                freeArmies.Add(army);
            }
        }
        if (armiesToMerge.Count > 1)
        {
            int total = 0;
            Army central = null;
            for (int i = 0; i < armiesToMerge.Count; i++)
            {
                Army army = armiesToMerge[i];
                if (central == null) { central = army; total = central.regiments.Count; continue; }
                if (total + army.regiments.Count < central.tile.supplyLimit)
                {
                    if (central.pos == army.pos)
                    {
                        army.CombineInto(central);
                        continue;
                    }
                    central.isMerging = true;
                    army.mergeTargets.Add(central);
                    central.mergeTargets.Add(army);
                    army.isMerging = true;
                    army.SetPath(central.pos);
                    total += army.regiments.Count;
                }
                else
                {
                    if (!central.isMerging)
                    {
                        freeArmies.Add(central);
                    }
                    central = army;
                    total = central.regiments.Count;
                    continue;
                }
            }
        }
        else
        {
            freeArmies.AddRange(armiesToMerge);
            armiesToMerge.Clear();
        }
        if(freeArmies.Count > 0)
        {
            if (civ.generals.Exists(i => i.equipped == false))
            {
                foreach (var army in civ.armies)
                {
                    if (civ.generals.Exists(i => i.equipped == false))
                    {
                        if (army.general == null || !army.general.active || !army.general.equipped)
                        {
                            int generalIndex = civ.generals.FindIndex(i => i.equipped == false);


                            if (Game.main.isMultiplayer)
                            {
                                if (NetworkManager.Singleton.IsServer)
                                {
                                    army.GetComponent<NetworkArmy>().EquipGeneralRpc(generalIndex);
                                }
                            }
                            else
                            {

                                army.AssignGeneral(civ.generals[generalIndex]);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            AddHomeProvinces(civ);
            MoveArmiesToProvinces(freeArmies);
        }
        if(civ.TotalMaxArmySize()/1000f < (civ.forceLimit.v - 1) && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.5f && Game.main.Started)
        {
            TileData safeProv = Map.main.GetTile(civ.SafeProvince());
            if (safeProv.unitQueue.Count == 0)
            {
                int desiredUnitType = AIMoveArmiesWar.GetDesiredUnitType(civ);
                AIMoveArmiesWar.RecruitUnit(safeProv, desiredUnitType);
            }
        }
    }
    public static void AddHomeProvinces(Civilisation civ)
    {
        foreach (var province in civ.GetAllCivTiles())
        {
            TileData data = Map.main.GetTile(province);
            allyProvinces.Add(province);
        }
    }
    public static float ProvinceScore(TileData tile,Civilisation civ,Army army)
    {
        float score = tile.supplyLimit;
        score -= TileData.evenr_distance(tile.pos, army.pos) * 2;
        score -= tile.armiesOnTile.Count > 1 ? tile.armiesOnTile.Count * 10f : 0f;
        return score;
    }
    public static void MoveArmiesToProvinces(List<Army> armies)
    {
        List<Vector3Int> provs = new List<Vector3Int>();
        provs.AddRange(allyProvinces);
        for (int i = 0; i < armies.Count; i++)
        {
            Army army = armies[i];
            provs.Sort((x, y) => ProvinceScore(Map.main.GetTile(y), Game.main.civs[army.civID], army).CompareTo(ProvinceScore(Map.main.GetTile(x), Game.main.civs[army.civID], army)));
            int loops = 0;
            while (provs.Count > 0 && loops < 100)
            {               
                if (army.pos != provs[0] && !army.SetPath(provs[0]))
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
