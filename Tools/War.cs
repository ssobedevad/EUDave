using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class War
{
    public Civilisation attackerCiv;
    public Civilisation defenderCiv;
    public float warScore;
    public List<float> battleResults = new List<float>();
    public List<float> siegeResults = new List<float>();
    public int WarID;
    public bool attackerSurrender;
    public bool defenderSurrender;
    public int lengthOfWar;
    public bool Involving(int civID)
    {
        if (attackerCiv.CivID == civID || defenderCiv.CivID == civID) { return true; }
        return false;
    }
    public bool Between(int civID1,int civID2)
    {
        if ((attackerCiv.CivID == civID1 && defenderCiv.CivID == civID2)|| (defenderCiv.CivID == civID1 && attackerCiv.CivID == civID2)) { return true; }
        return false;
    }
    public Civilisation GetOpposingLeader(int civID)
    {
        if(attackerCiv.CivID == civID) { return defenderCiv; }
        else { return attackerCiv; }
    }
    public War(Civilisation attacker , Civilisation defender)
    {
        attackerCiv = attacker;
        defenderCiv = defender;
        warScore = 0;
        Game.main.ongoingWars.Add(this);
        Game.main.hourTick.AddListener(HourTick);
        WarID = 0;
    }
    void HourTick()
    {
        lengthOfWar++;
    }

    public void EndWar()
    {
        attackerCiv.atWarWith.Remove(defenderCiv.CivID);
        defenderCiv.atWarWith.Remove(attackerCiv.CivID);
        Game.main.ongoingWars.Remove(this);
        Game.main.hourTick.RemoveListener(HourTick);
        foreach (var province in attackerCiv.GetAllCivTiles().ToList())
        {
            TileData tile = Map.main.GetTile(province);            
            if (tile.occupiedByID == defenderCiv.CivID)
            {
                tile.occupied = false;
                tile.siege = null;
            }
        }
        foreach (var province in defenderCiv.GetAllCivTiles().ToList())
        {
            TileData tile = Map.main.GetTile(province);
            if (tile.occupiedByID == attackerCiv.CivID)
            {
                tile.occupied = false;
                tile.siege = null;
            }
        }
    }
    public void AddBattle(float battleScore)
    {
        battleResults.Add(battleScore);
        UpdateWarScore();
    }
    public void CheckOccupations()
    {
        var aTiles = attackerCiv.GetAllCivTiles();
        int defenderOccupations = 0;
        int attackerOccupations = 0;
        bool fullyOccupiedAttacker = true;
        bool fullyOccupiedDefender = true;
        siegeResults.Clear();
        foreach (var tile in aTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if(prov.occupied && prov.occupiedByID == defenderCiv.CivID)
            {
                siegeResults.Add(-75f * prov.GetWarScore(defenderCiv.CivID)/attackerCiv.GetTotalWarScore(defenderCiv.CivID));
                defenderOccupations++;
            }
            else
            {
                fullyOccupiedAttacker = false;
            }
        }

        var dTiles = defenderCiv.GetAllCivTiles();
        foreach (var tile in dTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if (prov.occupied && prov.occupiedByID == attackerCiv.CivID)
            {
                siegeResults.Add(75f  * prov.GetWarScore(attackerCiv.CivID) / defenderCiv.GetTotalWarScore(attackerCiv.CivID));
                attackerOccupations++;
            }
            else
            {
                fullyOccupiedDefender = false;
            }
        }
        if (fullyOccupiedAttacker && attackerOccupations == 0) { attackerSurrender = true; }
        if (fullyOccupiedDefender && defenderOccupations == 0) { defenderSurrender = true; }
        UpdateWarScore();
    }

    void UpdateWarScore()
    {
        warScore = 0f;
        battleResults.ForEach(i => warScore += i);
        warScore = Mathf.Clamp(warScore, -40f, 40f);
        siegeResults.ForEach(i => warScore += i);
        warScore = Mathf.Clamp(warScore, -99f, 99f);
        if (attackerSurrender) { warScore = -100f; }
        else if (defenderSurrender) { warScore = 100f; }
        //Debug.Log(warScore);
    }
}
