using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeaceDeal
{
    public List<Vector3Int> provinces = new List<Vector3Int>();
    public List<int> civTo = new List<int>();
    public List<Vector3Int> possible = new List<Vector3Int>();
    public int numLoans;
    public float warScore;
    public float overextension;
    public float aggressiveExpansion;
    public War war;
    public Civilisation target;
    public Civilisation taker;
    public bool fullAnnexation;
    public bool subjugation;

    public PeaceDeal(PeaceDeal clone)
    {
        war = clone.war;
        target = clone.target;
        taker = clone.taker;
        warScore = clone.warScore;
        subjugation = clone.subjugation;
        provinces = clone.provinces.ToList();
        civTo = clone.civTo.ToList(); 
        possible = clone.possible.ToList();
        numLoans = clone.numLoans;
        fullAnnexation = clone.fullAnnexation;
    }
    public PeaceDeal(War War,Civilisation Target,Civilisation Taker)
    {
        war = War;
        target = Target;
        taker = Taker;
        warScore = 0f;
        subjugation = false;
        SetPossible();
    }
    public void AddLoan()
    {
        if(numLoans < 5)
        {
            numLoans++;
        }
        RecalculateWarScore();
    }
    public void RemoveLoan()
    {
        if (numLoans > 0)
        {
            numLoans--;
        }
        RecalculateWarScore();
    }
    public float WarScoreForSubjugation()
    {
        float score = 0f;
        foreach (var item in provinces)
        {
            TileData prov = Map.main.GetTile(item);
            score += prov.GetWarScore(taker.CivID);
        }
        score = target.GetTotalWarScore(taker.CivID) - score;
        
        return score;
    }
    public void RequestSubjugation()
    {
        if (!war.casusBelli.canTakeProvinces) { return; }
        if (target.GetTotalWarScore(taker.CivID) <= 100)
        {
            subjugation = true;
        }
        RecalculateWarScore();
    }
    public void RemoveSubjugation()
    {
        subjugation = false;
        RecalculateWarScore();
    }
    void RecalculateWarScore()
    {
        warScore = 0f;
        overextension = 0f;
        aggressiveExpansion = 0f;
        bool isPrimary = (target == war.attackerCiv || target == war.defenderCiv);
        for (int i = 0; i < provinces.Count;i++)
        {
            TileData prov = Map.main.GetTile(provinces[i]);
            int to = civTo[i];
            warScore += prov.GetWarScore(to);
            overextension += prov.totalDev * 0.8f;
            aggressiveExpansion += taker.GetBaseAE(prov, target, 0.6f, isPrimary ? 1f : 1.5f);
        }
        if (subjugation) 
        {
            warScore += WarScoreForSubjugation();
        }
        warScore += numLoans * 5f;
        SetPossible();
        UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>().refresh = true;
    }
    public void AddProvince(Vector3Int province)
    {
        if (!war.casusBelli.canTakeProvinces) { return; }
        TileData tileData = Map.main.GetTile(province);
        if (tileData.occupied && (!taker.atWarTogether.Contains(tileData.occupiedByID) && tileData.occupiedByID != taker.CivID)) { return; }
        provinces.Add(province);
        civTo.Add(tileData.occupied ? tileData.occupiedByID : taker.CivID);
        if(provinces.FindAll(i=>target.GetAllCivTiles().Contains(i)).Count >= target.GetAllCivTiles().Count)
        {
            fullAnnexation = true;
            subjugation = false;
        }
        RecalculateWarScore();
    }
    public void SetPossible()
    {
        if (!war.casusBelli.canTakeProvinces) { return; }
        List<Vector3Int> provs = target.GetAllCivTiles();
        if (war.Between(taker.CivID, target.CivID))
        {
            bool isAttacker = taker == war.attackerCiv;
            if (isAttacker)
            {
                foreach(var defender in war.defenderAllies)
                {
                    provs.AddRange(defender.GetAllCivTiles());
                }
            }
            else
            {
                foreach (var attacker in war.attackerAllies)
                {
                    provs.AddRange(attacker.GetAllCivTiles());
                }
            }
        }
        possible.Clear();
        foreach (var province in provs)
        {           
            TileData tileData = Map.main.GetTile(province);
            List<Vector3Int> neighbors = tileData.GetNeighbors();
            if(tileData.occupied && tileData.occupiedByID == -1) { continue; }
            Civilisation takeCiv = tileData.occupied ? Game.main.civs[tileData.occupiedByID] : taker;
            if ((taker == takeCiv || taker.atWarTogether.Contains(tileData.occupiedByID) )&& target.atWarWith.Contains(tileData.occupiedByID))
            {
                if (!provinces.Contains(province))
                {
                    if (takeCiv.CanCoreTile(tileData))
                    {
                        possible.Add(province);
                    }
                    else
                    {
                        if (tileData.occupied && (!taker.atWarTogether.Contains(tileData.occupiedByID) && tileData.occupiedByID != taker.CivID)) { continue; }
                        foreach (var neighbor in neighbors)
                        {
                            TileData n = Map.main.GetTile(neighbor);
                            if ((provinces.Contains(neighbor) && civTo[provinces.IndexOf(neighbor)] == takeCiv.CivID))
                            {
                                possible.Add(province);
                                break;
                            }
                        }
                    }
                }
            }     
        }
    }
    public void CheckProvinces()
    {
        foreach (var prov in provinces.ToList())
        {
            TileData tileData = Map.main.GetTile(prov);
            if (tileData.occupied && tileData.occupiedByID != civTo[provinces.IndexOf(prov)])
            {
                int index = provinces.IndexOf(prov);
                provinces.Remove(prov);
                civTo.RemoveAt(index);
                break;
            }                                                        
        }
    }
    public void RemoveProvince(Vector3Int province)
    {
        int index = provinces.IndexOf(province);
        int civToID = civTo[index];
        Civilisation takeCiv = Game.main.civs[civToID];
        provinces.Remove(province);
        civTo.RemoveAt(index);
        CheckProvinces();
        fullAnnexation = false;
        TileData tileData = Map.main.GetTile(province);
        List<Vector3Int> neighbors = tileData.GetNeighbors();
        List<Vector3Int> TilesToChain = new List<Vector3Int>();
        foreach(var neighbour in neighbors) 
        {
            if (provinces.Contains(neighbour))
            {
                index = provinces.IndexOf(neighbour);
                if (civToID == civTo[index])
                {
                    
                    TileData nextTileData = Map.main.GetTile(neighbour);
                    List<Vector3Int> nextNeighbors = nextTileData.GetNeighbors();
                    bool valid = false;
                    foreach (var nextNeighbor in nextNeighbors)
                    {
                        TileData check = Map.main.GetTile(nextNeighbor);
                        if (takeCiv.CanCoreTile(check))
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
                            index = provinces.IndexOf(nb);
                            if (civToID == civTo[index])
                            {
                                chainCheck.Enqueue(nb);
                                chain.Add(nb);
                            }
                        }
                        TileData nbTile = Map.main.GetTile(nb);
                        if(takeCiv.CanCoreTile(nbTile))
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
                        index = provinces.IndexOf(chainPos);
                        provinces.Remove(chainPos);
                        civTo.RemoveAt(index);
                    }
                }
            }
        }
        RecalculateWarScore();
    }
}
