using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMoveFleetsWar
{
    public static List<Fleet> fleets = new List<Fleet>();
    public static List<Fleet> fleetsMoving = new List<Fleet>();
    public static List<Fleet> fleetsSieging = new List<Fleet>();
    public static List<Fleet> fleetsLowSailorsStrength = new List<Fleet>();
    public static List<Fleet> fleetsToMerge = new List<Fleet>();
    public static List<Fleet> fleetsInBattle = new List<Fleet>();
    public static List<Fleet> fleetsRetreating = new List<Fleet>();
    public static List<Fleet> fleetsMerging = new List<Fleet>();
    public static List<Fleet> fleetsExiled = new List<Fleet>();

    public static List<EnemyFleet> enemyFleets = new List<EnemyFleet>();

    public static List<Vector3Int> enemyCoastalProvinces = new List<Vector3Int>();
    public static List<Vector3Int> allyCoastalProvinces = new List<Vector3Int>();
    public static void MoveAtWar(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        fleets.Clear();
        fleetsMoving.Clear();
        fleetsSieging.Clear();
        fleetsLowSailorsStrength.Clear();
        fleetsToMerge.Clear();
        fleetsInBattle.Clear();
        fleetsRetreating.Clear();
        fleetsMerging.Clear();
        fleetsExiled.Clear();

        enemyFleets.Clear();

        enemyCoastalProvinces.Clear();
        allyCoastalProvinces.Clear();

        BuildEnemyFleets(civ);
        ProcessFleetPositions(civ);

        if (fleetsToMerge.Count <= 1)
        {
            fleets.AddRange(fleetsToMerge);
        }
        if (fleetsExiled.Count > 0)
        {
            fleetsExiled.ForEach(i => i.SetPath(i.RetreatProvince()));
        }
        SupportBattles(civ);
        CheckRunAway(civID);
        if (fleets.Count > 0 || fleetsMoving.Count > 0 || fleetsSieging.Count > 0)
        {          
            if (enemyFleets.Count > 0)
            {
                MoveToEnemyArmies(civID);
            }
        }
        if (fleetsMerging.Count > 1)
        {
            MergeArmies(civID);
        }
    }
    static void SupportBattles(Civilisation civ)
    {
        List<NavalBattle> battles = Game.main.ongoingNavalBattles.FindAll(i => i != null && i.active && (civ.atWarWith.Contains(i.attackerCiv.CivID) || civ.atWarWith.Contains(i.defenderCiv.CivID)));
        List<Fleet> civFleets = GetOwnFleets();
        foreach (var battle in battles)
        {
            bool attacker = civ.atWarWith.Contains(battle.defenderCiv.CivID);
            float enemyEffectiveSize = attacker ? battle.defenderCount : battle.attackerCount;
            List<Fleet> civArmiesTemp = civFleets.ToList();
            List<int> dists = GetMoveDists(battle.pos, civFleets);
            List<Fleet> moveArmies = new List<Fleet>();
            List<int> moveDists = new List<int>();
            float moveArmies_effectiveSize = attacker ? battle.attackerCount : battle.defenderCount;
            int loops = 0;
            while (civArmiesTemp.Count > 0 && loops < 100)
            {
                int bestID = 0;

                for (int i = dists.Count - 1; i > 0; i--)
                {
                    if (dists[bestID] > dists[i])
                    {
                        bestID = i;
                    }
                }
                Fleet selected = civArmiesTemp[bestID];
                bool sieging = selected.tile.underSiege;
                if (Pathfinding.FindBestPath(civArmiesTemp[bestID].pos, battle.pos,fleet: civArmiesTemp[bestID],isBoat:true).Length > 0)
                {
                    if (!sieging)
                    {
                        moveArmies.Add(civArmiesTemp[bestID]);
                        moveDists.Add(dists[bestID]);
                        moveArmies_effectiveSize += civArmiesTemp[bestID].boats.Count;
                        civFleets.Remove(civArmiesTemp[bestID]);
                    }
                }
                civArmiesTemp.RemoveAt(bestID);
                dists.RemoveAt(bestID);
                if (moveArmies_effectiveSize >= enemyEffectiveSize)
                {
                    if (moveArmies.Count > 0)
                    {
                        int minDist = moveDists.First();
                        int maxDist = moveDists.Last();
                        Vector3Int closePos = moveArmies.First().pos;
                        for (int i = 0; i < moveArmies.Count; i++)
                        {
                            if (moveDists[i] > minDist)
                            {
                                Fleet army = moveArmies[i];
                                army.SetPath(closePos);
                            }
                            else if (minDist >= maxDist - 1)
                            {
                                Fleet army = moveArmies[i];
                                army.SetPath(battle.pos);
                            }
                        }
                        moveArmies.Clear();
                        break;
                    }
                }
            }
            if (loops >= 1000)
            {
                Debug.Log("Break 100 loops army");
            }
        }
    }
    static void CheckRunAway(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        foreach (var fleet in GetOwnFleets())
        {
            bool siege = fleet.tile.underSiege;
            List<Fleet> oFleetTemp = GetOwnFleets().ToList();
            oFleetTemp.Remove(fleet);
            float effectiveStrength = fleet.NavyStrength();
            foreach (var oFleet in oFleetTemp)
            {
                if (TileData.evenr_distance(oFleet.pos, fleet.pos) < 2)
                {
                    effectiveStrength += oFleet.NavyStrength();
                }
            }
            foreach (var eFleet in enemyFleets)
            {
                if (TileData.evenr_distance(eFleet.pos, fleet.pos) < 2)
                {
                    if (eFleet.effectiveSize > effectiveStrength * 1.1f)
                    {   
                        if (!siege)
                        {
                            if (eFleet.moving)
                            {
                                fleet.SetPath(fleet.RetreatProvince());
                            }
                            else if (!eFleet.sieging)
                            {
                                fleet.path.Clear();
                            }
                        }
                    }
                }
            }

        }
    }
    static void MoveToEnemyArmies(int civID)
    {
        SortEnemyArmies(civID);
        List<Fleet> civFleets = GetOwnFleets();
        Civilisation civ = Game.main.civs[civID];
        foreach (var eFleet in enemyFleets)
        {
            List<EnemyFleet> enemyTemp = enemyFleets.ToList();
            enemyTemp.Remove(eFleet);
            float enemyEffectiveSize = eFleet.effectiveSize;
            List<Fleet> civFleetsTemp = civFleets.ToList();
            List<int> dists = GetMoveDists(eFleet.pos, civFleets);
            List<Fleet> moveFleets = new List<Fleet>();
            List<int> moveDists = new List<int>();
            float moveFleets_effectiveSize = 0;

            int loops = 0;
            while (civFleetsTemp.Count > 0 && loops < 100)
            {
                int bestID = 0;

                for (int i = dists.Count - 1; i > 0; i--)
                {
                    if (dists[bestID] > dists[i])
                    {
                        bestID = i;
                    }
                }
                Fleet selected = civFleetsTemp[bestID];
                bool sieging = selected.tile.underSiege;
                if (Pathfinding.FindBestPath(civFleetsTemp[bestID].pos, eFleet.pos,fleet: civFleetsTemp[bestID], isBoat: true).Length > 0)
                {
                    if (!sieging)
                    {
                        moveFleets.Add(civFleetsTemp[bestID]);
                        moveDists.Add(dists[bestID]);
                        moveFleets_effectiveSize += civFleetsTemp[bestID].NavyStrength();
                        civFleets.Remove(civFleetsTemp[bestID]);
                    }
                }
                civFleetsTemp.RemoveAt(bestID);
                dists.RemoveAt(bestID);
                if (moveFleets_effectiveSize >= enemyEffectiveSize)
                {
                    if (moveFleets.Count > 0)
                    {
                        int minDist = moveDists.First();
                        int maxDist = moveDists.Last();
                        Vector3Int closePos = moveFleets.First().pos;
                        for (int i = 0; i < moveFleets.Count; i++)
                        {
                            if (moveDists[i] > minDist)
                            {
                                Fleet army = moveFleets[i];
                                army.SetPath(closePos);
                            }
                            else if (minDist >= maxDist - 1)
                            {
                                Fleet army = moveFleets[i];
                                army.SetPath(eFleet.pos);
                            }
                        }
                        moveFleets.Clear();
                        break;
                    }
                }
            }
            if (loops >= 1000)
            {
                Debug.Log("Break 100 loops army");
            }
        }
    }
    static void SortEnemyArmies(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        for (int i = 0; i < enemyFleets.Count; i++)
        {
            enemyFleets[i].distance = TileData.evenr_distance(enemyFleets[i].pos, Game.main.civs[civID].capitalPos);
        }
        enemyFleets.Sort((x, y) => TileData.evenr_distance(y.pos, civ.capitalPos).CompareTo(TileData.evenr_distance(x.pos, civ.capitalPos)));
    }
    static List<Fleet> GetOwnFleets()
    {
        List<Fleet> civFleets = new List<Fleet>();
        civFleets.AddRange(fleets);
        civFleets.AddRange(fleetsMoving);
        civFleets.AddRange(fleetsSieging);
        //civArmies.AddRange(armiesLowMoraleManpower);
        return civFleets;
    }
    static List<int> GetMoveDists(Vector3Int pos, List<Fleet> fleets)
    {
        List<int> dists = new List<int>();
        foreach (var fleet in fleets)
        {
            dists.Add(TileData.evenr_distance(pos, fleet.pos));
        }
        return dists;
    }
    static List<int> GetMoveDists(Vector3Int pos, List<EnemyFleet> fleets)
    {
        List<int> dists = new List<int>();
        foreach (var fleet in fleets)
        {
            dists.Add(TileData.evenr_distance(pos, fleet.pos));
        }
        return dists;
    }
    static void MergeArmies(int civID)
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
    static void BuildEnemyFleets(Civilisation civ)
    {
        foreach (var id in civ.atWarWith)
        {
            Civilisation target = Game.main.civs[id];
            foreach (var fleet in target.fleets)
            {

                if (enemyFleets.Exists(i => i.pos == fleet.pos))
                {
                    EnemyFleet a = enemyFleets.Find(i => i.pos == fleet.pos);
                    a.effectiveSize += fleet.NavyStrength();
                }
                else
                {
                    enemyFleets.Add(new EnemyFleet(fleet.pos, fleet.NavyStrength(), fleet.path.Count > 0, fleet.tile.underSiege && fleet.path.Count == 0, fleet.path.Count > 0 && fleet.moveTimer >= fleet.moveTime * 0.5f));
                }
            }
        }
        foreach (var eFleet in enemyFleets.ToList())
        {
            float effectiveEnemy = eFleet.effectiveSize;
            List<EnemyFleet> enemyTemp = enemyFleets.ToList();
            enemyTemp.Remove(eFleet);
            foreach (var eFleet2 in enemyTemp.ToList())
            {
                if (TileData.evenr_distance(eFleet2.pos, eFleet.pos) < 2)
                {
                    effectiveEnemy += eFleet2.effectiveSize;
                    enemyTemp.Remove(eFleet2);
                }
            }
            enemyFleets.Find(i => i.pos == eFleet.pos).effectiveSize = effectiveEnemy;
        }
    }
    public static bool ShouldMergeFleet(Civilisation civ, Fleet fleet)
    {
        return (civ.fleets.Count > 1 && fleet.CombatWidth() < 31);
    }
    public static bool ShouldSplitFleet(Civilisation civ, Fleet fleet)
    {
        return (fleet.CombatWidth() > 62);
    }
    static void ProcessFleetPositions(Civilisation civ)
    {
        foreach (var fleet in civ.fleets)
        {
            if (fleet != null)
            {
                if (fleet.inBattle) { fleetsInBattle.Add(fleet); continue; }
                if (fleet.retreating) { fleetsRetreating.Add(fleet); continue; }
                if (fleet.exiled) { fleetsExiled.Add(fleet); continue; }
                TileData tile = Map.main.GetTile(fleet.pos);
                if (tile.underSiege)
                {
                    fleetsSieging.Add(fleet);
                }
                else if (fleet.TotalSailors() < 0.5f * fleet.TotalMaxSailors())
                {
                    fleetsLowSailorsStrength.Add(fleet);
                }
                else if (ShouldMergeFleet(civ, fleet))
                {
                    if (!fleet.isMerging)
                    {
                        fleetsToMerge.Add(fleet);
                    }
                    else if (fleet.isMerging)
                    {
                        fleetsMerging.Add(fleet);
                    }
                }
                else if (ShouldSplitFleet(civ, fleet))
                {
                    fleet.Split();
                }
                else if (fleet.path.Count == 0)
                {
                    if (fleet.boats.Count > 0)
                    {
                        fleets.Add(fleet);
                    }
                }
                else
                {
                    fleetsMoving.Add(fleet);
                }
            }
        }
    }
}
public class EnemyFleet
{
    public Vector3Int pos;
    public float effectiveSize;
    public float distance;
    public bool moving;
    public bool sieging;
    public bool committed;

    public EnemyFleet(Vector3Int pos, float effectiveSize, bool moving, bool sieging, bool committed)
    {
        this.pos = pos;
        this.effectiveSize = effectiveSize;
        this.moving = moving;
        this.sieging = sieging;
        this.committed = committed;
    }
}
