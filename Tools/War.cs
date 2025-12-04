using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class War
{
    public Civilisation attackerCiv;
    public List<Civilisation> attackerAllies = new List<Civilisation>();
    public Civilisation defenderCiv;
    public List<Civilisation> defenderAllies = new List<Civilisation>();
    public float warScore;
    public List<float> battleResults = new List<float>();
    public List<float> siegeResults = new List<float>();
    public int WarID;
    public bool attackerSurrender;
    public bool defenderSurrender;
    public int lengthOfWar;
    public Vector3Int warGoal;
    public CasusBelli casusBelli;
    public bool attackerControlsWarGoal;
    public int warGoalTimer;
    public bool active;
    public bool Involving(int civID)
    {
        if (attackerCiv.CivID == civID || defenderCiv.CivID == civID|| attackerAllies.Exists(i=>i.CivID == civID) || defenderAllies.Exists(i => i.CivID == civID)) { return true; }
        return false;
    }
    public bool InvolvingAll(List<int> civIDs)
    {       
        foreach(var id in civIDs)
        {
            if (!Involving(id))
            {
                return false;
            }
        }       
        return true;
    }
    public string GetName()
    {
        if (casusBelli.Name == "")
        {
            return attackerCiv.civName + " War Of Aggression";
        }
        else
        {
            if (casusBelli.superiority)
            {
                return attackerCiv.civName + " " + casusBelli.Name + " of " + defenderCiv.civName;
            }
            else if (casusBelli.capital)
            {
                return attackerCiv.civName + " " + casusBelli.Name + " of " + defenderCiv.civName;
            }
            else if (casusBelli.province)
            {
                if (warGoal != Vector3Int.zero)
                {
                    return attackerCiv.civName + " "+ casusBelli.Name +" of " + Map.main.GetTile(warGoal).Name;
                }
                else
                {
                    return attackerCiv.civName + " " + casusBelli.Name + " of " + defenderCiv.civName;
                }
            }
        }
        return attackerCiv.civName + " War Of Aggression";
    }
    public void TakeOverLeadership(Civilisation newLeader,bool asAttacker)
    {
        if (newLeader.subjects.Count > 0)
        {
            foreach (var subject in newLeader.subjects)
            {
                JoinWar(Game.main.civs[subject], asAttacker);
            }
        }
        if (asAttacker)
        {
            if (attackerAllies.Contains(newLeader)) 
            { attackerAllies.Remove(newLeader); }
            else
            {
                foreach (var attackerAlly in attackerAllies)
                {
                    attackerAlly.atWarTogether.Add(newLeader.CivID);
                    newLeader.atWarTogether.Add(attackerAlly.CivID);
                }
            }
            if (!attackerAllies.Contains(attackerCiv)) { attackerAllies.Add(attackerCiv); }
            newLeader.atWarWith.Add(defenderCiv.CivID);
            defenderCiv.atWarWith.Add(newLeader.CivID);
            foreach (var defenderAlly in defenderAllies)
            {
                defenderAlly.atWarWith.Add(newLeader.CivID);
                newLeader.atWarWith.Add(defenderAlly.CivID);
                if (newLeader.allies.Contains(defenderAlly.CivID))
                {
                    newLeader.BreakAlliance(defenderAlly.CivID);
                }
            }
            attackerCiv = newLeader;            
        }
        else
        {
            if (defenderAllies.Contains(newLeader)) { defenderAllies.Remove(newLeader); }
            else
            {
                foreach (var defenderAlly in defenderAllies)
                {
                    defenderAlly.atWarTogether.Add(newLeader.CivID);
                    newLeader.atWarTogether.Add(defenderAlly.CivID);
                }
            }
            if (!defenderAllies.Contains(defenderCiv)) { defenderAllies.Add(defenderCiv); }
            newLeader.atWarWith.Add(attackerCiv.CivID);
            attackerCiv.atWarWith.Add(newLeader.CivID);
            foreach (var attackerAlly in attackerAllies)
            {
                attackerAlly.atWarWith.Add(newLeader.CivID);
                newLeader.atWarWith.Add(attackerAlly.CivID);
                if (newLeader.allies.Contains(attackerAlly.CivID))
                {
                    newLeader.BreakAlliance(attackerAlly.CivID);
                }
            }
            defenderCiv = newLeader;
        }
    }
    public void JoinWar(Civilisation civ , bool asAttackerAlly)
    {        
        if(civ.subjects.Count > 0)
        {
            foreach (var subject in civ.subjects)
            {
                JoinWar(Game.main.civs[subject], asAttackerAlly);
            }
        }
        if (asAttackerAlly)
        {
            if (attackerAllies.Contains(civ)|| defenderAllies.Contains(civ)) { return; }
            if (attackerCiv != civ)
            {
                civ.atWarWith.Add(defenderCiv.CivID);
                defenderCiv.atWarWith.Add(civ.CivID);
                foreach (var defenderAlly in defenderAllies)
                {
                    defenderAlly.atWarWith.Add(civ.CivID);
                    civ.atWarWith.Add(defenderAlly.CivID);
                    if (civ.allies.Contains(defenderAlly.CivID))
                    {
                        civ.BreakAlliance(defenderAlly.CivID);
                    }
                }
                attackerCiv.atWarTogether.Add(civ.CivID);
                civ.atWarTogether.Add(attackerCiv.CivID);
                foreach (var attackerAlly in attackerAllies)
                {
                    attackerAlly.atWarTogether.Add(civ.CivID);
                    civ.atWarTogether.Add(attackerAlly.CivID);
                }
                attackerAllies.Add(civ);
            }
            
            
        }
        else
        {
            if (defenderAllies.Contains(civ) || attackerAllies.Contains(civ)) { return; }
            if (defenderCiv != civ)
            {
                civ.atWarWith.Add(attackerCiv.CivID);
                attackerCiv.atWarWith.Add(civ.CivID);
                foreach (var attackerAlly in attackerAllies)
                {
                    attackerAlly.atWarWith.Add(civ.CivID);
                    civ.atWarWith.Add(attackerAlly.CivID);
                    if (civ.allies.Contains(attackerAlly.CivID))
                    {
                        civ.BreakAlliance(attackerAlly.CivID);
                    }
                }
                defenderCiv.atWarTogether.Add(civ.CivID);
                civ.atWarTogether.Add(defenderCiv.CivID);
                foreach (var defenderAlly in defenderAllies)
                {
                    defenderAlly.atWarTogether.Add(civ.CivID);
                    civ.atWarTogether.Add(defenderAlly.CivID);
                }
                defenderAllies.Add(civ);
            }
           
        }
        UpdateWarScore();
    }
    public bool Between(int civID1,int civID2)
    {
        if ((attackerCiv.CivID == civID1 && defenderCiv.CivID == civID2)|| (defenderCiv.CivID == civID1 && attackerCiv.CivID == civID2)) { return true; }
        return false;
    }
    public Civilisation GetOpposingLeader(int civID)
    {
        if(attackerCiv.CivID == civID || attackerAllies.Exists(i=>i.CivID == civID)) { return defenderCiv; }
        else { return attackerCiv; }
    }
    public War(Civilisation attacker, Civilisation defender)
    {
        attackerCiv = attacker;
        defenderCiv = defender;
        warScore = 0;
        Game.main.ongoingWars.Add(this);
        Game.main.hourTick.AddListener(HourTick);
        Game.main.dayTick.AddListener(DayTick);
        WarID = 0;
        active = true;
        UpdateWarScore();
    }
    void HourTick()
    {
        lengthOfWar++;
    }
    void DayTick()
    {
        CheckOccupations();
        UpdateWarScore();
        if(warGoal != Vector3Int.zero && warGoalTimer < 25)
        {
            warGoalTimer++;
        }
    }
    public void LeaveWar(int civID)
    {
        Civilisation leaver = Game.main.civs[civID];
        if(leaver.subjects.Count > 0)
        {
            foreach (var subject in leaver.subjects)
            {
                LeaveWar(subject);
            }
        }
        bool attacker = true;
        if (attackerAllies.Contains(leaver))
        {
            attacker = true;
        }
        else if (defenderAllies.Contains(leaver))
        {
            attacker = false;
        }
        else
        {
            return;
        }
        if (attacker)
        {
            defenderCiv.atWarWith.Remove(leaver.CivID);
            leaver.atWarWith.Remove(defenderCiv.CivID);
            foreach (var defenderAlly in defenderAllies)
            {
                leaver.atWarWith.Remove(defenderAlly.CivID);
                defenderAlly.atWarWith.Remove(leaver.CivID);                
            }
            UnsiegeProvs(leaver, true);
            foreach (var province in defenderCiv.GetAllCivTiles().ToList())
            {
                TileData tile = Map.main.GetTile(province);
                if (tile.occupiedByID == leaver.CivID)
                {
                    tile.occupied = false;
                    tile.siege = null;
                }
            }
            attackerAllies.Remove(leaver);

            attackerCiv.atWarTogether.Remove(leaver.CivID);
            leaver.atWarTogether.Remove(attackerCiv.CivID);
            foreach (var attackerAlly in attackerAllies)
            {
                attackerAlly.atWarTogether.Remove(leaver.CivID);
                leaver.atWarTogether.Remove(attackerAlly.CivID);
            }
        }
        else
        {
            attackerCiv.atWarWith.Remove(leaver.CivID);
            leaver.atWarWith.Remove(attackerCiv.CivID);
            foreach (var attackerAlly in attackerAllies)
            {
                leaver.atWarWith.Remove(attackerAlly.CivID);
                attackerAlly.atWarWith.Remove(leaver.CivID);
            }
            UnsiegeProvs(leaver,false);
            foreach (var province in attackerCiv.GetAllCivTiles().ToList())
            {
                TileData tile = Map.main.GetTile(province);
                if (tile.occupiedByID == leaver.CivID)
                {
                    tile.occupied = false;
                    tile.siege = null;
                }
            }
            defenderAllies.Remove(leaver);

            defenderCiv.atWarTogether.Remove(leaver.CivID);
            leaver.atWarTogether.Remove(defenderCiv.CivID);
            foreach (var defenderAlly in defenderAllies)
            {
                defenderAlly.atWarTogether.Remove(leaver.CivID);
                leaver.atWarTogether.Remove(defenderAlly.CivID);
            }
        }              
    }
    void UnsiegeProvs(Civilisation civ,bool attacker)
    {
        foreach (var province in civ.GetAllCivTiles().ToList())
        {
            TileData tile = Map.main.GetTile(province);
            if (tile.occupiedByID == (attacker ?defenderCiv.CivID: attackerCiv.CivID) || (attacker ? defenderAllies.Exists(i => i.CivID == tile.occupiedByID) : attackerAllies.Exists(i => i.CivID == tile.occupiedByID)) )
            {
                tile.occupied = false;
                tile.siege = null;
            }
        }
    }
    public void EndWar()
    {
        attackerCiv.atWarWith.Remove(defenderCiv.CivID);
        defenderCiv.atWarWith.Remove(attackerCiv.CivID);


        foreach (var attackerAlly in attackerAllies.ToList())
        {
            attackerAlly.atWarWith.RemoveAll(i=>defenderAllies.Exists(a => a.CivID == i));
            defenderCiv.atWarWith.Remove(attackerAlly.CivID);
            attackerAlly.atWarWith.Remove(defenderCiv.CivID);

            attackerAlly.atWarTogether.RemoveAll(i => attackerAllies.Exists(a => a.CivID == i));
            attackerCiv.atWarTogether.Remove(attackerAlly.CivID);
            attackerAlly.atWarTogether.Remove(attackerCiv.CivID);

            UnsiegeProvs(attackerAlly, true);
        }

        foreach (var defenderAlly in defenderAllies.ToList())
        {
            defenderAlly.atWarWith.RemoveAll(i => attackerAllies.Exists(a => a.CivID == i));
            attackerCiv.atWarWith.Remove(defenderAlly.CivID);
            defenderAlly.atWarWith.Remove(attackerCiv.CivID);

            defenderAlly.atWarTogether.RemoveAll(i => defenderAllies.Exists(a => a.CivID == i));
            defenderCiv.atWarTogether.Remove(defenderAlly.CivID);
            defenderAlly.atWarTogether.Remove(defenderCiv.CivID);

            UnsiegeProvs(defenderAlly, false);
        }


        UnsiegeProvs(attackerCiv, true);
        UnsiegeProvs(defenderCiv, false);

        Game.main.ongoingWars.Remove(this);
        Game.main.hourTick.RemoveListener(HourTick);
        active = false;
    }
    public void AddBattle(float battleScore)
    {
        battleResults.Add(battleScore);
        UpdateWarScore();
    }
    public float OccupiedAndBesiegedProvs(Civilisation target)
    {
        var aTiles = target.GetAllCivTiles();
        if(target.GetTotalDev() == 0) { return 40; }
        float score = 0f;
        foreach (var tile in aTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if (prov.occupied)
            {
                score += 40 * prov.totalDev / target.GetTotalDev();
            }
            else if (prov.underSiege)
            {
                score += 20 * prov.totalDev / target.GetTotalDev();
            }
        }
        return Mathf.Round(score);
    }
    public float IndividualWarScore(Civilisation target,bool isAttacker)
    {
        Civilisation from = isAttacker ? defenderCiv : attackerCiv;
        var aTiles = target.GetAllCivTiles();
        float score = 0f;
        foreach (var tile in aTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if (prov.occupied && prov.occupiedByID == from.CivID)
            {
                score += prov.GetWarScore(from.CivID) / target.GetTotalWarScore(from.CivID);
            }
        }
        return Mathf.Round(score * 100f);
    }
    public void CheckOccupations()
    {
        if (warGoal != Vector3Int.zero)
        {
            TileData warGoalTile = Map.main.GetTile(warGoal);
            if (!warGoalTile.occupied && attackerControlsWarGoal)
            {
                warGoalTimer = 0;
                attackerControlsWarGoal = false;
            }
            else if (warGoalTile.occupied && !attackerControlsWarGoal)
            {
                warGoalTimer = 0;
                attackerControlsWarGoal = true;
            }
        }
        var aTiles = attackerCiv.GetAllCivTiles();       
        attackerAllies.ForEach(i => aTiles.AddRange(i.GetAllCivTiles()));
        if (aTiles.Count == 0) { return; }
        int defenderOccupations = 0;
        int attackerOccupations = 0;
        bool fullyOccupiedAttacker = true;
        bool fullyOccupiedDefender = true;
        float totalAttackingSideWarScore = attackerCiv.GetTotalWarScore(defenderCiv.CivID);
        attackerAllies.ForEach(i => totalAttackingSideWarScore += i.GetTotalWarScore(defenderCiv.CivID));
        float totalDefendingSideWarScore = defenderCiv.GetTotalWarScore(attackerCiv.CivID);
        defenderAllies.ForEach(i => totalDefendingSideWarScore += i.GetTotalWarScore(attackerCiv.CivID));
        siegeResults.Clear();
        foreach (var tile in aTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if(prov.occupied && (prov.occupiedByID == defenderCiv.CivID || defenderAllies.Exists(i=>i.CivID == prov.occupiedByID)))
            {
                siegeResults.Add(-75f * prov.GetWarScore(defenderCiv.CivID)/ totalAttackingSideWarScore);
                defenderOccupations++;
            }
            else
            {
                fullyOccupiedAttacker = false;
            }
        }

        var dTiles = defenderCiv.GetAllCivTiles();
        defenderAllies.ForEach(i => dTiles.AddRange(i.GetAllCivTiles()));
        if (dTiles.Count == 0) { return; }
        foreach (var tile in dTiles)
        {
            TileData prov = Map.main.GetTile(tile);
            if (prov.occupied && (prov.occupiedByID == attackerCiv.CivID || attackerAllies.Exists(i => i.CivID == prov.occupiedByID)))
            {
                siegeResults.Add(75f  * prov.GetWarScore(attackerCiv.CivID) / totalDefendingSideWarScore);
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
        warScore += warGoalTimer *( attackerControlsWarGoal ? 1 : -1);
        warScore = Mathf.Clamp(warScore, -99f, 99f);
        if (attackerSurrender) { warScore = -100f; }
        else if (defenderSurrender) { warScore = 100f; }
    }
}
