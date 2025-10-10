using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PeaceDeal
{
    public List<Vector3Int> provinces = new List<Vector3Int>();
    public List<Vector3Int> possible = new List<Vector3Int>();
    public float warScore;
    public War war;
    public bool attacker;
    public bool fullAnnexation;

    public PeaceDeal(War War,bool Attacker)
    {
        war = War;
        attacker = Attacker;
        warScore = 0f;
        SetPossible();
    }
    void RecalculateWarScore()
    {
        warScore = 0f;
        foreach (var item in provinces)
        {
            TileData prov = Map.main.GetTile(item);
            warScore += prov.GetWarScore(attacker? war.attackerCiv.CivID : war.defenderCiv.CivID);
        }
        SetPossible();
        UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>().refresh = true;
    }
    public void AddProvince(Vector3Int province)
    {
        provinces.Add(province);
        if(provinces.Count >= (attacker ? war.defenderCiv.GetAllCivTiles().Count : war.attackerCiv.GetAllCivTiles().Count))
        {
            fullAnnexation = true;
        }
        RecalculateWarScore();
    }
    public void SetPossible()
    {
        Civilisation target = attacker ? war.defenderCiv : war.attackerCiv;
        Civilisation taker = attacker ? war.attackerCiv : war.defenderCiv;
        List<Vector3Int> provs = target.GetAllCivTiles();
        possible.Clear();
        foreach (var province in provs)
        {
            if (provinces.Contains(province)) { continue; }
            TileData tileData = Map.main.GetTile(province);
            List<Vector3Int> neighbors = tileData.GetNeighbors();
            foreach (var neighbor in neighbors)
            {
                TileData n = Map.main.GetTile(neighbor);
                if (n.occupied && n.occupiedByID != taker.CivID) { continue; }
                if (n.civID == taker.CivID || provinces.Contains(neighbor))
                {
                    possible.Add(province);
                    break;
                }
            }
        }
    }
    public void CheckProvinces()
    {
        Civilisation target = attacker ? war.defenderCiv : war.attackerCiv;
        Civilisation taker = attacker ? war.attackerCiv : war.defenderCiv;
        foreach (var prov in provinces.ToList())
        {
            TileData tileData = Map.main.GetTile(prov);
            if (tileData.occupied && tileData.occupiedByID != taker.CivID)
            {
                provinces.Remove(prov);
                break;
            }                                                        
        }
    }
    public void RemoveProvince(Vector3Int province)
    {
        Civilisation target = attacker ? war.defenderCiv : war.attackerCiv;
        Civilisation taker = attacker ? war.attackerCiv : war.defenderCiv;
        provinces.Remove(province);
        CheckProvinces();
        fullAnnexation = false;
        TileData tileData = Map.main.GetTile(province);
        List<Vector3Int> neighbors = tileData.GetNeighbors();
        List<Vector3Int> TilesToChain = new List<Vector3Int>();
        foreach(var neighbour in neighbors) 
        {
            if (provinces.Contains(neighbour))
            {
                TileData nextTileData = Map.main.GetTile(neighbour);
                List<Vector3Int> nextNeighbors = nextTileData.GetNeighbors();
                bool valid = false;
                foreach(var nextNeighbor in nextNeighbors)
                {
                    TileData check = Map.main.GetTile(nextNeighbor);
                    if (check.civID == taker.CivID)
                    {
                        valid = true;
                        break;
                    }
                }
                if (!valid)
                {
                    TilesToChain.Add(neighbour);
                }
            }
        }
        if(TilesToChain.Count > 0)
        {
            foreach (var tile in TilesToChain) 
            {
                Queue<Vector3Int> chainCheck = new Queue<Vector3Int>();
                List<Vector3Int> chain = new List<Vector3Int>();
                chainCheck.Enqueue(tile);
                chain.Add(tile);
                bool validChain = false;
                int loops = 0;
                while (chainCheck.Count > 0 && loops < 100)
                {
                    Vector3Int checkSpot = chainCheck.Dequeue();
                    TileData checkTile = Map.main.GetTile(checkSpot);
                    List<Vector3Int> nbs = checkTile.GetNeighbors();
                    foreach(var nb in nbs)
                    {
                        if (provinces.Contains(nb) && !chain.Contains(nb))
                        {
                            chainCheck.Enqueue(nb);
                            chain.Add(nb);
                        }
                        TileData nbTile = Map.main.GetTile(nb);
                        if(nbTile.civID == taker.CivID)
                        {
                            validChain = true; 
                            break;
                        }
                    }
                    if(validChain) { break; }
                }
                if (!validChain)
                {
                    foreach(var chainPos in chain)
                    {
                        provinces.Remove(chainPos);
                    }
                }
            }
        }
        RecalculateWarScore();
    }
}
