using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMoveArmiesWar
{
    public static List<Army> armies = new List<Army>();
    public static List<Army> armiesMoving = new List<Army>();
    public static List<Army> armiesSieging = new List<Army>();
    public static List<Army> armiesLowMoraleManpower = new List<Army>();
    public static List<Army> armiesToMerge = new List<Army>();
    public static List<Army> armiesInBattle = new List<Army>();
    public static List<Army> armiesRetreating = new List<Army>();
    public static List<Army> armiesMerging = new List<Army>();
    public static List<Army> armiesExiled = new List<Army>();

    public static List<EnemyArmy> enemyArmies = new List<EnemyArmy>();

    public static List<Vector3Int> enemyProvinces = new List<Vector3Int>();
    public static List<Vector3Int> allyProvinces = new List<Vector3Int>();
    public static bool shouldCarpetSiege = false;
    public static bool defensiveStance = false;

    public static bool ShouldRecruit(Civilisation civ)
    {
        if(civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value)
        {
            if (civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.5f)
            {
                return true;
            }
            else if (civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value * 0.9f && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.7f)
            {
                return true;
            }
            else if (civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value * 0.8f && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.6f)
            {
                return true;
            }
            else if (civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value * 0.7f && civ.GetTotalTilePopulation() > civ.GetTotalMaxPopulation() * 0.5f)
            {
                return true;
            }
            else if (civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value * 0.6f)
            {
                return true;
            }
        }
        return false;
    }
    public static void MoveAtWar(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        if (ShouldRecruit(civ))
        {
            TileData safeTile = Map.main.GetTile(civ.SafeProvince());
            if (!safeTile.occupied && !safeTile.underSiege && civ.coins >= safeTile.GetRecruitCost(0) && safeTile.recruitQueue.Count == 0)
            {
                if (civ.TotalMaxArmySize() / 1000f < civ.forceLimit.value * 0.6f && civ.GetTotalTilePopulation() < civ.GetTotalMaxPopulation() * 0.5f)
                {
                    List<MercenaryGroup> possibleMercs = civ.GetPossibleMercs();
                    if (possibleMercs.Count > 0)
                    {
                        int first = Map.main.mercenaries.ToList().IndexOf(possibleMercs[0]);
                        float cost = safeTile.GetMercRecruitCost(first);
                        if(civ.coins < cost && civ.loans.Count < civ.GetMaxLoans())
                        {
                            civ.TakeLoan();
                            //Debug.Log("AI Take Loan for Mercs " + civ.civName);
                        }
                        if (civ.coins >= cost)
                        {
                            safeTile.StartRecruitingMercenary(first);
                            //Debug.Log("AI Hire mercs " + civ.civName);
                        }
                    }
                    else
                    {
                        safeTile.StartRecruiting(0);
                    }
                }
                else
                {
                    safeTile.StartRecruiting(0);
                }
            }
        }
        armies.Clear();
        armiesMoving.Clear();
        armiesSieging.Clear();
        armiesLowMoraleManpower.Clear();
        armiesToMerge.Clear();
        armiesInBattle.Clear();
        armiesRetreating.Clear();
        armiesMerging.Clear();
        armiesExiled.Clear();

        shouldCarpetSiege = false;
        defensiveStance = false;

        enemyArmies.Clear();

        enemyProvinces.Clear();
        allyProvinces.Clear();
        BuildEnemyArmy(civ);
        ProcessArmyPositions(civ);

        if (armiesToMerge.Count <= 1)
        {
            armies.AddRange(armiesToMerge);
        }
        if (armiesExiled.Count > 0)
        {
            armiesExiled.ForEach(i => i.SetPath(i.RetreatProvince()));
        }
        SupportBattles(civ);
        CheckRunAway(civID);
        //Debug.Log("Processing...");
        if (armies.Count > 0 || armiesMoving.Count > 0 || armiesSieging.Count > 0)
        {
            //Debug.Log("Looking For Enemies");          
            if(enemyArmies.Count > 0)
            {
                //Debug.Log("Enemies found " + enemyArmies.Count);
                MoveToEnemyArmies(civID);
            }
        }
        if (armiesToMerge.Count > 1)
        {
            Army central = armiesToMerge[0];
            central.isMerging = true;
            for (int i = 1; i < armiesToMerge.Count; i++)
            {
                Army army = armiesToMerge[i];
                army.mergeTargets.Add(central);
                central.mergeTargets.Add(army);
                army.isMerging = true;
                army.SetPath(central.pos);
            }
        }
        if (armies.Count > 0)
        {
            DeterminePossibleProvinces(civ);
            if (enemyProvinces.Count > 0 || allyProvinces.Count > 0)
            {
                MoveArmiesToProvinces(civ);
            }
        }
    }
    public static float GetSiegeStayDesire(Army army)
    {
        float stay = 0f;
        TileData tile = Map.main.GetTile(army.pos);
        if (tile.siege.armiesSieging.Count < 2)
        {
            stay += (1f + tile.siege.progress + tile.siege.fortLevel) * 100f;
        }
        else if (tile.siege.unitsSieging() - army.ArmySize() <= tile.fortLevel * 3000 + 1000)
        {
            stay += (1f + tile.siege.progress + tile.siege.fortLevel) * 100f;
        }
        else
        {
            stay += (1f + tile.siege.progress + tile.siege.fortLevel) * 10f;
        }
        return Mathf.Pow(stay,2);
    }
    public static void CheckRunAway(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        foreach (var army in GetOwnArmies())
        {
            bool siege = army.tile.underSiege;
            List<Army> oarmytemp = GetOwnArmies().ToList();
            oarmytemp.Remove(army);
            float effectiveStrength = army.ArmyStrength();
            foreach (var oArmy in oarmytemp)
            {
                if (TileData.evenr_distance(oArmy.pos, army.pos) < 2)
                {
                    effectiveStrength += oArmy.ArmyStrength();
                }
            }
            foreach (var eArmy in enemyArmies)
            {               
                if (TileData.evenr_distance(eArmy.pos,army.pos) < 2)
                {
                    if (eArmy.effectiveSize > effectiveStrength * 1.1f)
                    {
                        if (siege && GetSiegeStayDesire(army) < (eArmy.effectiveSize - effectiveStrength)/25f)
                        {                           
                            if (eArmy.moving)
                            {
                                army.SetPath(army.RetreatProvince());
                            }
                            else if (!eArmy.sieging)
                            {
                                army.path.Clear();
                            }
                        }
                        else if (!siege)
                        {
                            if (eArmy.moving)
                            {
                                army.SetPath(army.RetreatProvince());
                            }
                            else if (!eArmy.sieging)
                            {
                                army.path.Clear();
                            }
                        }
                    }
                }
            }

        }
    }

    public static void ProcessArmyPositions(Civilisation civ)
    {
        foreach(var army in civ.armies)
        {
            if (army != null)
            {
                if (army.inBattle) { armiesInBattle.Add(army);continue; }
                if (army.retreating) { armiesRetreating.Add(army);continue; }
                if (army.exiled) { armiesExiled.Add(army); continue; }
                TileData tile = Map.main.GetTile(army.pos);
                if (tile.underSiege)
                {
                    armiesSieging.Add(army);
                }
                else if (army.AverageMorale() < 0.5f)
                {
                    armiesLowMoraleManpower.Add(army);
                }
                else if (ShouldMergeArmy(civ, army))
                {
                    if (!army.isMerging)
                    {
                        armiesToMerge.Add(army);
                    }
                    else if (army.isMerging)
                    {
                        armiesMerging.Add(army);
                    }
                }
                else if (ShouldSplitArmy(civ, army))
                {
                    army.Split();                    
                }
                else if (army.path.Count == 0)
                {
                    if (army.regiments.Count > 0)
                    {
                        armies.Add(army);                                       
                    }
                }
                else
                {
                    
                    armiesMoving.Add(army);
                    
                }
            }
        }
    }
    public static bool ShouldMergeArmy(Civilisation civ, Army army)
    {
        return (civ.armies.Count > 1 && army.regiments.Count < civ.combatWidth.value);
    }
    public static bool ShouldSplitArmy(Civilisation civ, Army army)
    {
        return (army.regiments.Count > civ.combatWidth.value * 2);
    }
    public static void BuildEnemyArmy(Civilisation civ)
    {
        foreach(var id in civ.atWarWith)
        {
            Civilisation target = Game.main.civs[id];
            foreach(var army in target.armies)
            {
                
                if (enemyArmies.Exists(i=>i.pos == army.pos))
                {
                    EnemyArmy a = enemyArmies.Find(i => i.pos == army.pos);
                    a.effectiveSize += army.ArmyStrength();
                }
                else
                {
                    enemyArmies.Add(new EnemyArmy(army.pos, army.ArmyStrength(),army.path.Count > 0,army.tile.underSiege && army.path.Count == 0));
                }                
            }
        }
        foreach (var eArmy in enemyArmies.ToList())
        {
            float effectiveEnemy = eArmy.effectiveSize;
            List<EnemyArmy> enemyTemp = enemyArmies.ToList();
            enemyTemp.Remove(eArmy);
            foreach (var eArmy2 in enemyTemp.ToList())
            {
                if (TileData.evenr_distance(eArmy2.pos, eArmy.pos) < 2)
                {
                    effectiveEnemy += eArmy2.effectiveSize;
                    enemyTemp.Remove(eArmy2);
                }
            }
            enemyArmies.Find(i=>i.pos == eArmy.pos).effectiveSize = effectiveEnemy;
        }
    }
    public static void SupportBattles(Civilisation civ)
    {
        List<Battle> battles = Game.main.ongoingBattles.FindAll(i =>i!= null&&i.active && !i.AttackerRebels && !i.DefenderRebels && ( civ.atWarWith.Contains(i.attackerCiv.CivID) || civ.atWarWith.Contains(i.defenderCiv.CivID)));
        List<Army> civArmies = GetOwnArmies();
        foreach (var battle in battles)
        {
            bool attacker = civ.atWarWith.Contains(battle.defenderCiv.CivID);
            float enemyEffectiveSize = attacker? battle.defenderCount :  battle.attackerCount;
            List<Army> civArmiesTemp = civArmies.ToList();
            List<int> dists = GetMoveDists(battle.pos, civArmies);
            List<Army> moveArmies = new List<Army>();
            List<int> moveDists = new List<int>();
            float moveArmies_effectiveSize = 0;
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
                Army selected = civArmiesTemp[bestID];
                bool sieging = selected.tile.underSiege;
                if (Pathfinding.FindBestPath(civArmiesTemp[bestID].pos, battle.pos, civArmiesTemp[bestID]).Length > 0)
                {
                    if ((sieging && GetSiegeStayDesire(selected) < 50) || !sieging)
                    {
                        moveArmies.Add(civArmiesTemp[bestID]);
                        moveDists.Add(dists[bestID]);
                        moveArmies_effectiveSize += civArmiesTemp[bestID].ArmyStrength();
                        civArmies.Remove(civArmiesTemp[bestID]);
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
                                Army army = moveArmies[i];
                                army.SetPath(closePos);
                            }
                            else if (minDist >= maxDist - 1)
                            {
                                Army army = moveArmies[i];
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
    public static void MoveToEnemyArmies(int civID)
    {
        SortEnemyArmies(civID);
        List<Army> civArmies = GetOwnArmies();
        Civilisation civ = Game.main.civs[civID];
        foreach(var eArmy in enemyArmies)
        {
            List<EnemyArmy> enemyTemp = enemyArmies.ToList();
            enemyTemp.Remove(eArmy);
            float enemyEffectiveSize = eArmy.effectiveSize;
            List<Army> civArmiesTemp = civArmies.ToList();
            List<int> dists = GetMoveDists(eArmy.pos, civArmies);
            List<Army> moveArmies = new List<Army>();
            List<int> moveDists = new List<int>();
            float moveArmies_effectiveSize = 0;

            int loops = 0;
            while(civArmiesTemp.Count > 0 && loops < 100)
            {
                int bestID = 0; 

                for(int i = dists.Count - 1; i > 0; i--)
                {
                    if (dists[bestID] > dists[i])
                    {
                        bestID = i;
                    }
                }
                Army selected = civArmiesTemp[bestID];
                bool sieging = selected.tile.underSiege;
                if (Pathfinding.FindBestPath(civArmiesTemp[bestID].pos, eArmy.pos, civArmiesTemp[bestID]).Length > 0)
                {
                    if ((sieging && GetSiegeStayDesire(selected) < 50) || !sieging )
                    {
                        moveArmies.Add(civArmiesTemp[bestID]);
                        moveDists.Add(dists[bestID]);
                        moveArmies_effectiveSize += civArmiesTemp[bestID].ArmyStrength();
                        civArmies.Remove(civArmiesTemp[bestID]);
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
                                Army army = moveArmies[i];
                                army.SetPath(closePos);                                
                            }
                            else if(minDist >= maxDist - 1) 
                            {
                                Army army = moveArmies[i];
                                army.SetPath(eArmy.pos);
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
    public static void SortEnemyArmies(int civID)
    {
        Civilisation civ = Game.main.civs[civID];
        for (int i = 0; i < enemyArmies.Count; i++)
        {
            enemyArmies[i].distance = TileData.evenr_distance(enemyArmies[i].pos, Game.main.civs[civID].capitalPos);
        }
        enemyArmies.Sort((x, y) => ProvinceScore(y.pos,civ.capitalPos).CompareTo(ProvinceScore(x.pos, civ.capitalPos)));
    }
    public static List<Army> GetOwnArmies()
    {
        List<Army> civArmies = new List<Army>();
        civArmies.AddRange(armies);
        civArmies.AddRange(armiesMoving);
        civArmies.AddRange(armiesSieging);
        //civArmies.AddRange(armiesLowMoraleManpower);
        return civArmies;
    }
    public static List<int> GetMoveDists(Vector3Int pos, List<Army> armies)
    {
        List<int> dists = new List<int>();
        foreach(var army in armies)
        {
            dists.Add(TileData.evenr_distance(pos,army.pos));
        }
        return dists;
    }
    public static List<int> GetMoveDists(Vector3Int pos, List<EnemyArmy> armies)
    {
        List<int> dists = new List<int>();
        foreach (var army in armies)
        {
            dists.Add(TileData.evenr_distance(pos, army.pos));
        }
        return dists;
    }
    public static void MergeArmies(int civID)
    {
        Debug.Log("Merge Armies");
    }
    public static void DeterminePossibleProvinces(Civilisation civ)
    {
        AddHomeProvinces(civ);
        AddEnemyProvinces(civ);
    }
    public static void AddHomeProvinces(Civilisation civ)
    {
        foreach(var province in civ.GetAllCivTiles()) 
        {
            TileData data = Map.main.GetTile(province);
            if (data.occupied||data.underSiege)
            {
                allyProvinces.Add(province);
            }
        }
        foreach(var allyid in civ.allies)
        {
            Civilisation ally = Game.main.civs[allyid];
            if (ally.atWarWith.Exists(i => civ.atWarWith.Contains(i)))
            {
                foreach (var province in ally.GetAllCivTiles())
                {
                    TileData data = Map.main.GetTile(province);
                    if ((data.occupied && (civ.atWarWith.Contains(data.occupiedByID) || data.occupiedByID == -1) ) || data.underSiege)
                    {
                        allyProvinces.Add(province);
                    }
                }
            }
        }
    }
    public static void AddEnemyProvinces(Civilisation civ)
    {
        foreach (var id in civ.atWarWith)
        {
            Civilisation target = Game.main.civs[id];
            foreach (var province in target.GetAllCivTiles())
            {
                TileData data = Map.main.GetTile(province);
                if (!data.occupied || data.occupiedByID == -1)
                {
                    if (!data.underSiege || data.siege.unitsSieging() < Mathf.Max(data.fortLevel * 3000,1000))
                    {
                        enemyProvinces.Add(province);
                    }
                }
                
            }
        }
    }
    public static void MoveArmiesToProvinces(Civilisation civ)
    {
        List<Vector3Int> provs = new List<Vector3Int>();
        provs.AddRange(allyProvinces);
        provs.AddRange(enemyProvinces);
        for (int i = 0; i < armies.Count; i++)
        {
            Army army = armies[i];
            provs.Sort((x, y) => ProvinceScore(y, army.pos).CompareTo(ProvinceScore(x, army.pos)));
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
    public static float ProvinceScore(Vector3Int pos, Vector3Int fromPos)
    {
        TileData data = Map.main.GetTile(pos);
        if (data == null) { return 0; }
        float score = 0;
        EnemyArmy tileArmy = enemyArmies.Find(x => x.pos == pos);
        if (tileArmy != null) 
        {
            score -= tileArmy.effectiveSize;
        }
        score += data.totalDev;
        score += data.tileResource.Value;
        score -= TileData.evenr_distance(pos, fromPos) * 5f;
        score += data.fortLevel * 10;
        if (data.civID > -1)
        {
            score += (data.civ.capitalPos == pos) ? 50f : 0f;
        }
        return score;
    }
    public class EnemyArmy
    {
        public Vector3Int pos;
        public float effectiveSize;
        public float distance;
        public bool moving;
        public bool sieging;

        public EnemyArmy(Vector3Int pos, float effectiveSize, bool moving,bool sieging)
        {
            this.pos = pos;
            this.effectiveSize = effectiveSize;
            this.moving = moving;
            this.sieging = sieging;
        }
    }
}
