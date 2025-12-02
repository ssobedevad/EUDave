using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Army : MonoBehaviour
{
    public int civID;
    public List<Regiment> regiments = new List<Regiment>();
    public bool inBattle;
    public bool retreating;
    public bool isMerging;
    public List<Army> mergeTargets = new List<Army>();
    public List<Vector3Int> path = new List<Vector3Int>();
    public float moveTimer,moveTime;
    public bool exiled;
    public bool isMercenary;
    public General general;
    public float attrition;
    float savedSiegeProgress;
    public Vector3Int pos => Map.main.tileMapManager.tilemap.WorldToCell(transform.position);
    public Vector3Int lastPos;
    public Vector3Int lastPosNoZOC;

    public TileData tile => Map.main.GetTile(pos);

    [SerializeField] Transform moveArrowRotatePoint;
    [SerializeField] Image MoveArrowFill;

    public void AssignGeneral(General General)
    {
        if(general != null && !general.equipped)
        {            
            general.equipped = false;
        }
        General.equipped = true;
        general = General;
    }
    public void EnterBattle()
    {
        inBattle = true;
        foreach (var regiment in regiments)
        {
            regiment.inBatle = true;
        }
    }
    public void SetAttrition()
    {
        float percent = 0f;
        float armysize = ArmySize();
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            foreach (var army in tile.armiesOnTile)
            {
                if (civ.atWarTogether.Contains(army.civID) && army != this)
                {
                    armysize += army.ArmySize();
                }
            }
            if (civ.atWarWith.Contains(tile.civID))
            {
                percent += 1f + tile.fortLevel;
                percent += tile.localAttritionForEnemies.v;
                percent += tile.civ.attritionForEnemies.v;
            }           
        }
        if (armysize / 1000f > tile.supplyLimit)
        {
            percent += Mathf.Clamp((armysize / 1000f - tile.supplyLimit) * (10f / ((float)tile.supplyLimit)), 0f, 5f);
        }
        if(civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            percent *= (1f + civ.landAttrition.v);
        }
       attrition = percent;
    }
    public void DayTick()
    {
        SetAttrition();
        foreach (var regiment in regiments)
        {
            regiment.size = Mathf.Max((int)(regiment.size * (100f - attrition) / 100f), 0);
            if (!regiment.inBatle && regiment.type > -1 && civID > -1)
            {
                regiment.RecoverMorale();
                if (!regiment.mercenary)
                {
                    regiment.RefillRegiment();
                }
            }
        }       
    }
    public static Army NewArmy(SaveGameArmy save)
    {
        TileData tile = Map.main.GetTile(save.p.GetVector3Int());
        if(tile == null) { return null; }
        Army a = Instantiate(Map.main.armyPrefab, tile.worldPos(), Quaternion.identity, Map.main.unitTransform).GetComponent<Army>();
        ArmyUIProvince uIProvince = Instantiate(UIManager.main.ArmyUIPrefab, tile.worldPos(), Quaternion.identity, UIManager.main.unitCanvas).GetComponent<ArmyUIProvince>();
        uIProvince.army = a;
        a.civID = save.id;
        a.regiments.AddRange(save.rs);
        UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
        Game.main.dayTick.AddListener(a.DayTick);
        tile.armiesOnTile.Add(a);
        a.inBattle = save.b;
        a.retreating = save.r;
        a.exiled = save.e;
        a.moveTimer = save.tr;
        a.moveTime = save.t;
        a.general = save.g;
        a.path = save.pt.ConvertAll(i=>i.GetVector3Int());
        a.lastPos = save.lp.GetVector3Int();
        a.lastPosNoZOC = save.nz.GetVector3Int();
        a.savedSiegeProgress = save.s;
        return a;
    }
    public static Army NewArmy(TileData tile, int civID, List<Regiment> regiments,bool merc = false)
    {
        Army a = Instantiate(Map.main.armyPrefab, tile.worldPos(), Quaternion.identity, Map.main.unitTransform).GetComponent<Army>();
        ArmyUIProvince uIProvince = Instantiate(UIManager.main.ArmyUIPrefab, tile.worldPos(), Quaternion.identity, UIManager.main.unitCanvas).GetComponent<ArmyUIProvince>();
        uIProvince.army = a;
        UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
        Game.main.dayTick.AddListener(a.DayTick);
        a.civID = civID;
        tile.armiesOnTile.Add(a);
        a.regiments.AddRange(regiments);
        a.isMercenary = merc;
        return a;
    }
    public void WinBattleMorale()
    {
        foreach (var regiment in regiments)
        {
            regiment.morale = Mathf.Min(regiment.morale + regiment.maxMorale * 0.5f, regiment.maxMorale);
        }
    }
    public void ExitBattle()
    {
        inBattle = false;
        foreach(var regiment in regiments)
        {
            regiment.inBatle = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I) && Player.myPlayer.selectedArmies.Contains(this))
        {
            var neighbors = tile.GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData neighbour = Map.main.GetTile(n);
                SpriteRenderer image = Instantiate(UIManager.main.DebugCirclePrefab, neighbour.worldPos(), Quaternion.identity).GetComponent<SpriteRenderer>();
                image.color = CanMoveHostileZOC(n, pos)? Color.green : Color.red;
            }           
        }
        if (Input.GetKeyDown(KeyCode.U) && Player.myPlayer.selectedArmies.Contains(this))
        {
            var neighbors = tile.GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData neighbour = Map.main.GetTile(n);
                SpriteRenderer image = Instantiate(UIManager.main.DebugCirclePrefab, neighbour.worldPos(), Quaternion.identity).GetComponent<SpriteRenderer>();
                image.color = HasAccess(neighbour.civID) ? Color.blue : Color.red;
            }
        }
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            foreach (var regiment in regiments)
            {
                regiment.maxMorale = civ.moraleMax.v;
                regiment.meleeDamage = civ.units[regiment.type].meleeDamage.v;
                regiment.flankingDamage = civ.units[regiment.type].flankingDamage.v;
                regiment.rangedDamage = civ.units[regiment.type].rangedDamage.v;
                regiment.flankingRange = civ.units[regiment.type].flankingRange;
            }
            if (tile.civID != civID)
            {
                if (!HasAccess(tile.civID) && !civ.atWarWith.Contains(tile.civID))
                {
                    exiled = true;
                }
            }
            else
            {
                if (exiled)
                {
                    exiled = false;
                    path.Clear();
                }
            }
            if(savedSiegeProgress > 0 && path.Count > 0)
            {
                savedSiegeProgress = 0;
            }
        }       
        if (inBattle)
        {           
            if(Player.myPlayer.selectedArmies.Contains(this))
            {
                Player.myPlayer.selectedBattle = Game.main.ongoingBattles.Find(i=>i.GetInvolvedArmies().Contains(this));
            }
        }
        else 
        {
            CheckBattle();
            if (Player.myPlayer.selectedArmies.Count == 1 && Player.myPlayer.selectedArmies[0] == this)
            {
                LineRenderer renderer = Player.myPlayer.armyMove;               
                if (path.Count == 0)
                {
                    renderer.gameObject.SetActive(false);
                    renderer.positionCount = 0;
                }
                else
                {
                    renderer.gameObject.SetActive(true);
                    renderer.positionCount = path.Count + 1;
                    List<Vector3> positions = new List<Vector3>() { Map.main.tileMapManager.tilemap.CellToWorld(pos) };
                    positions.AddRange(path.ConvertAll(i => Map.main.tileMapManager.tilemap.CellToWorld(i)).ToArray());
                    renderer.SetPositions(positions.ToArray());
                }
            }
            if (path.Count > 0)
            {
                moveArrowRotatePoint.gameObject.SetActive(true);
                Vector3 dir = Map.main.tileMapManager.tilemap.CellToWorld(path.First()) - Map.main.tileMapManager.tilemap.CellToWorld(pos);
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                moveArrowRotatePoint.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                MoveArrowFill.fillAmount = moveTimer / moveTime;

            }
            else
            {
                moveArrowRotatePoint.gameObject.SetActive(false);
            }
            if (isMerging)
            {
                mergeTargets.RemoveAll(i => i == null);
                if (mergeTargets.Count == 1 && mergeTargets[0].pos == pos)
                {
                    CombineInto(mergeTargets[0]);
                    mergeTargets[0].mergeTargets.Remove(this);
                }
                if (path.Count == 0)
                {
                    foreach(var target in mergeTargets.ToList())
                    {
                        if(target.path.Count == 0)
                        {
                            target.mergeTargets.Remove(this);
                            target.isMerging = false;
                            mergeTargets.Remove(target);
                        }
                    }
                }
                if (mergeTargets.Count == 0)
                {
                    isMerging = false;
                }
            }           
        }
    }

    private void Start()
    {
        moveTime = 12f;
        if (civID > -1)
        {
            Game.main.civs[civID].armies.Add(this);
        }
        Game.main.tenMinTick.AddListener(UpdateMovement);
    }
    public void OnDestroy()
    {
        if (civID > -1)
        {
            Game.main.civs[civID].armies.Remove(this);
        }
    }
    public void CombineInto(Army army)
    {
        if (army.civID == civID)
        {
            army.regiments.AddRange(regiments);
            regiments.Clear();
            OnExitTile();
            Destroy(gameObject);
        }
    }
    public void Split()
    {
        if(regiments.Count == 1) { return; }
        List <Regiment> newRegiments = new List<Regiment>();
        List<Regiment> temp = regiments.ToList();
        for(int i = temp.Count/2; i < temp.Count; i++)
        {
            newRegiments.Add(temp[i]);
            regiments.Remove(temp[i]);
        }
        if (newRegiments.Count > 0)
        {
            Army a = NewArmy(tile, civID, newRegiments);
            a.lastPos = lastPos;
        }
    }
    public void SplitOff(int amount)
    {
        if(amount <= 0) { return; }
        if (regiments.Count <= amount) { return; }
        List<Regiment> newRegiments = new List<Regiment>();
        List<Regiment> temp = regiments.ToList();
        for (int i = 0; i < Mathf.Min(temp.Count,amount); i++)
        {
            newRegiments.Add(temp[i]);
            regiments.Remove(temp[i]);
        }
        if (newRegiments.Count > 0)
        {
            Army a = NewArmy(tile, civID, newRegiments);
            a.lastPos = lastPos;
        }
    }
    public void Consolidate(bool removeEmpty = true)
    {
        regiments.Sort((x,y)=> y.morale.CompareTo(x.morale));
        for(int i = 0; i < regiments.Count -1 ;i++)
        {
            Regiment regiment = regiments[i];
            if(regiment.size < regiment.maxSize)
            {
                for(int j = i + 1; j < regiments.Count; j++)
                {
                    Regiment regimentFrom = regiments[j];
                    if(regimentFrom.size > 0 && regimentFrom.type == regiment.type && regimentFrom.mercenary == regiment.mercenary)
                    {
                        int amount = Mathf.Min(regiment.maxSize - regiment.size, regimentFrom.size);
                        regiment.size += amount;
                        regimentFrom.size -= amount;
                        if(regiment.size == regiment.maxSize) { break; }
                    }
                }
            }
        }
        if (removeEmpty) 
        {
            List<Regiment> temp = regiments.ToList();
            temp.RemoveAll(i => i.size == 0);
            regiments = temp.ToList();
        }
    }
    public float ArmyStrength()
    {
        float strength = 0;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            foreach(var unit in regiments)
            {
                if(unit.type < 0 || unit.type >= civ.units.Count) { continue; }
                float power = (float)unit.size/(float)unit.maxSize;
                power *= unit.morale;
                power *= Mathf.Max(civ.units[unit.type].meleeDamage.v * (1f + ((general != null &&general.active) ? general.meleeSkill * 0.1f : 0f))
                    , civ.units[unit.type].flankingDamage.v * (1f + ((general != null && general.active) ? general.flankingSkill * 0.1f : 0f))
                    , civ.units[unit.type].rangedDamage.v * (1f + ((general != null && general.active) ? general.rangedSkill * 0.1f : 0f)));
                power *= 1f + civ.units[unit.type].combatAbility.v;

                strength += power;
            }            
        }       
        return strength;
    }
    public float AverageMorale()
    {
        float morale = 0;
        if (regiments.Count > 0)
        {
            foreach (var reg in regiments)
            {              
                morale += reg.morale;                
            }
        }
        return morale / regiments.Count;
    }
    public float AverageMaxMorale()
    {
        float morale = 0;
        if (regiments.Count > 0)
        {
            foreach (var reg in regiments)
            {
                morale += reg.maxMorale;
            }
        }
        return morale / regiments.Count;
    }
    public float ArmySize()
    {
        float units = 0;
        if (regiments.Count > 0)
        {
            foreach (var reg in regiments)
            {
                units += reg.size;
            }
        }
        return units;
    }
    public float ArmyMaxSize()
    {
        float unitsT = 0;
        if (regiments.Count > 0)
        {
            foreach (var reg in regiments)
            {
                unitsT += reg.maxSize;
            }
        }
        return unitsT;
    }
    public void UpdateMovement()
    {
        TileData tileData = Map.main.GetTile(pos);
       
        float movementSpeed = 1f;
        if(civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            movementSpeed += civ.movementSpeed.v;
        }
        if(general != null && general.active)
        {
            movementSpeed += general.maneuverSkill * 0.05f;
        }
        if (!inBattle)
        {
            if (path.Count > 0)
            {
                if (!CanMoveHostileZOC(path[0],pos)) { path.Clear(); return; }
                if (moveTimer >= moveTime)
                {                  
                    if (Pathfinding.CanMoveToTile(pos,path[0], false))
                    {                        
                        Vector3Int target = path[0];
                        path.RemoveAt(0);
                        
                        OnExitTile();
                        transform.position = Map.main.tileMapManager.tilemap.CellToWorld(target);
                        OnEnterTile();
                        moveTimer = 0;                       
                    }
                }
                else
                {
                    moveTimer += movementSpeed * (retreating? 2f : 1f);
                    TileData targetTile = Map.main.GetTile(path.First());
                    moveTime = Mathf.Max(24f *(1f - (targetTile.terrain != null ? targetTile.terrain.moveSpeedMod : 0f)),1f);
                }
            }
            if(path.Count == 0)
            {
                retreating = false;
                moveTimer = 0;            
                TrySiege(tileData);
            }
        }
    }
    void CheckBattle()
    {
        TileData tileData = Map.main.GetTile(pos);
        if (civID == -1 || tileData.armiesOnTile.Count == 0) { return; }
        Civilisation civ = Game.main.civs[civID];
        if (tileData._battle != null)
        {
            if (tileData._battle.attackerCiv.CivID == civID || civ.atWarWith.Contains(tileData._battle.defenderCiv.CivID))
            {
                tileData._battle.AddToBattle(this, true);
            }
            else if (tileData._battle.defenderCiv.CivID == civID || civ.atWarWith.Contains(tileData._battle.attackerCiv.CivID))
            {
                tileData._battle.AddToBattle(this, false);
            }
        }
        if (tileData._battle == null && !retreating && !exiled)
        {
            List<Army> defenders = new List<Army>();
            List<Army> attackers = new List<Army>();
            bool isAttacker = true;
            if(tileData.fortLevel > 0 &&
                (
                (tileData.occupied == false && (tileData.civID == civID || civ.atWarTogether.Contains(tileData.civID)))||
                (tileData.occupied == true && (tileData.occupiedByID == civID || civ.atWarTogether.Contains(tileData.occupiedByID)))
                )
                )
            {
                isAttacker = false;
            }
            if (civ.atWarWith.Count > 0 && tileData.armiesOnTile.Exists(i => civ.atWarWith.Contains(i.civID)))
            {
                if (isAttacker)
                {
                    defenders = tileData.armiesOnTile.FindAll(i => civ.atWarWith.Contains(i.civID) && !i.retreating && !i.exiled);
                    attackers = tileData.armiesOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders, isAttacker);
                }
                else
                {
                    attackers = tileData.armiesOnTile.FindAll(i => civ.atWarWith.Contains(i.civID) && !i.retreating && !i.exiled);
                    defenders = tileData.armiesOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders,isAttacker);
                }
            }
            else if (tileData.armiesOnTile.Exists(i => i.civID == -1))
            {
                if (isAttacker)
                {
                    defenders = tileData.armiesOnTile.FindAll(i => i.civID == -1 && !i.retreating && !i.exiled);
                    attackers = tileData.armiesOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders, isAttacker);
                }
                else
                {
                    attackers = tileData.armiesOnTile.FindAll(i => i.civID == -1 && !i.retreating && !i.exiled);
                    defenders = tileData.armiesOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders, isAttacker);
                }
            }
        }
    }
    void StartBattle(List<Army> attackers,List<Army> defenders,bool isAttacker)
    {        
        if (defenders.Count > 0 && attackers.Count > 0)
        {
            War war = Game.main.ongoingWars.Find(i => i.InvolvingAll(new List<int>() { attackers[0].civID, defenders[0].civID }));
            Battle newBattle;
            if (war != null)
            {
                newBattle = new Battle(pos, attackers[0], defenders[0], warID: war.WarID);
            }
            else
            {
                newBattle = new Battle(pos, attackers[0], defenders[0]);
            }

            if (defenders.Count > 0)
            {
                for (int i = 0; i < defenders.Count; i++)
                {
                    Army defender = defenders[i];
                    if (!defender.retreating && !newBattle.GetInvolvedArmies().Contains(defender))
                    {
                        newBattle.AddToBattle(defender, false);
                    }
                }
            }
            if (attackers.Count > 0)
            {
                for (int i = 0; i < attackers.Count; i++)
                {
                    Army attacker = attackers[i];
                    if (!attacker.retreating && !newBattle.GetInvolvedArmies().Contains(attacker))
                    {
                        newBattle.AddToBattle(attacker, true);
                    }
                }
            }

        }
    }
    public void OnExitTile()
    {
        TileData tileData = Map.main.GetTile(pos);
        tileData.armiesOnTile.Remove(this);
        lastPos = pos;
        if(!tileData.hasZOC || (tileData.hasZOC && !tileData.HasNeighboringActiveFort(civID)))
        {
            lastPosNoZOC = pos;
        }
    }
    public void TrySiege(TileData tile)
    {
        if(tile.civID == -1 || exiled) { return; }
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            bool canrelease = tile.occupied && (tile.civID == civID || civ.atWarTogether.Contains(tile.civID) || civ.subjects.Contains(tile.civID));
            bool canSiege = civID != tile.civID && 
                (
                (tile.civ.atWarWith.Contains(civID) && !tile.occupied) ||
                (tile.occupied && tile.occupiedByID == -1)
                );
            if (canSiege || canrelease)
            {
                if (!tile.underSiege)
                {
                    tile.siege = new Siege(tile, civID);
                    tile.siege.progress = savedSiegeProgress;
                    tile.siege.armiesSieging.Add(this);
                }
                else if (tile.underSiege && !tile.siege.armiesSieging.Contains(this))
                {
                    tile.siege.armiesSieging.Add(this);
                }
            }
        }
        else
        {
            if (!tile.underSiege && !(tile.occupied && tile.occupiedByID == -1))
            {
                tile.siege = new Siege(tile, civID);
                tile.siege.progress = savedSiegeProgress;
                tile.siege.armiesSieging.Add(this);
            }
            else if (tile.underSiege && !tile.siege.armiesSieging.Contains(this))
            {
                tile.siege.armiesSieging.Add(this);
            }
        }
    }
    public void OnEnterTile()
    {
        TileData tileData = Map.main.GetTile(pos);
        tileData.armiesOnTile.Add(this);
        CheckBattle();
        TrySiege(tileData);
    }
    public Vector3Int RetreatProvince()
    {
        if(civID == -1) { return tile.pos; }
        Civilisation civ = Game.main.civs[civID];
        Vector3Int retreat = civ.capitalPos;
        List<Vector3Int> possible = civ.GetAllCivTiles();
        List<Vector3Int> enemyPos = new List<Vector3Int>();
        for(int i = 0; i < civ.atWarWith.Count; i++)
        {
            Civilisation civ2 = Game.main.civs[civ.atWarWith[i]];
            possible.AddRange(civ2.GetAllCivTiles());
            for(int j =0; j < civ2.armies.Count; j++)
            {
                Vector3Int pos = civ2.armies[j].pos;
                if (!enemyPos.Contains(pos)) 
                {
                    enemyPos.Add(pos);
                }
            }
        }
        List<float> score = new List<float>();
        foreach(var tile in possible)
        {
            TileData tileData = Map.main.GetTile(tile);
            int dist = TileData.evenr_distance(pos, tile);
            float tileScore = dist < 10? dist * 10 : 50;
            if (tileData.civID == civID)
            {
                foreach(var enemy in enemyPos)
                {
                    dist = TileData.evenr_distance(enemy, tile);
                    if (dist < 2)
                    {
                        tileScore += -100;
                    }
                    else
                    {
                        tileScore += Mathf.Pow(dist,2);
                    }
                }
                if (tileData.occupied || tileData.underSiege || Pathfinding.FindBestPath(pos,tile,this).Length == 0) 
                { 
                    score.Add(-1000); 
                    continue;
                }
                
                tileScore += tileData.hasZOC ? 2 : 0;
                tileScore += tileData.hasFort ? 5 : 0;
            }
            else
            {
                tileScore -= 100;
            }
            tileScore += tile == civ.capitalPos ? 10 : 0;
            tileScore += tileData.totalDev;
            score.Add(tileScore);
        }
        float bestScoreSoFar = -1000;
        for(int i = 0; i < score.Count; i++)
        {
            if (score[i] > bestScoreSoFar)
            {
                retreat = possible[i];
                bestScoreSoFar = score[i];
            }
        }
        return retreat;
    }
    public void SetRetreat(Vector3Int destination)
    {
        retreating = true;
        foreach(var regiment in regiments)
        {
            regiment.morale = Mathf.Min(regiment.morale, 0.5f);
        }
        path.Clear();
        var newPath = Pathfinding.FindBestPath(pos, destination, army: this, isBoat: false);
        if (newPath.Length > 0)
        {
            path.AddRange(newPath.ToList());
        }
    }
    public bool SetPath(Vector3Int destination)
    {
        if (retreating) { return false; }
        Vector3Int[] newPath = new Vector3Int[0];
        if (moveTimer < moveTime * 0.5f)
        {
            path.Clear();
            newPath = Pathfinding.FindBestPath(pos, destination,army: this,isBoat: false);
        }
        else if (path.Count > 0)
        {
            Vector3Int goal = path.First();
            path.Clear();
            path.Add(goal);
            newPath = Pathfinding.FindBestPath(goal, destination, army: this, isBoat: false);
        }
        if(newPath.Length > 0)
        {
            path.AddRange(newPath.ToList());
            return true;
        }
        else
        {            
            return false;
        }
    }
    public bool HasAccess(int CivID)
    {
        if (civID == CivID) { return true; }
        if (CivID == -1) { return true; }
        return HasAccess(CivID, Game.main.civs[civID]);
    }
    public static bool HasAccess(int CivID,Civilisation civ)
    {
        if (civ.CivID == CivID) { return true; }
        if (CivID == -1) { return true; }
        Civilisation target = Game.main.civs[CivID];
        if (civ.atWarWith.Contains(CivID)) { return true; }
        if (civ.subjects.Contains(CivID)) { return true; }
        if (civ.atWarWith.Contains(CivID)) { return true; }
        if (civ.militaryAccess.Contains(CivID)) { return true; }

        if (civ.atWarTogether.Exists(i => Game.main.civs[i].militaryAccess.Contains(CivID))) { return true; }
        if (civ.atWarTogether.Exists(i => Game.main.civs[i].atWarWith.Contains(CivID))) { return true; }
        if (civ.atWarTogether.Exists(i => Game.main.civs[i].atWarTogether.Contains(CivID))) { return true; }
        if (civ.atWarTogether.Exists(i => Game.main.civs[i].subjects.Contains(CivID))) { return true; }

        if (civ.atWarWith.Exists(i => Game.main.civs[i].militaryAccess.Contains(CivID))) { return true; }
        if (civ.atWarWith.Exists(i => Game.main.civs[i].atWarWith.Contains(CivID))) { return true; }
        if (civ.atWarWith.Exists(i => Game.main.civs[i].atWarTogether.Contains(CivID))) { return true; }
        if (civ.atWarWith.Exists(i => Game.main.civs[i].subjects.Contains(CivID))) { return true; }

        return false;
    }
    public void DisbandMercs()
    {
        regiments.RemoveAll(i => i.mercenary);
        if (regiments.Count == 0)
        {
            OnExitTile();
            Destroy(gameObject);
        }
    }
    public void Disband()
    {
        regiments.RemoveAll(i => i.mercenary);
        int space = Game.main.civs[civID].AddPopulation((int)ArmySize());
        if (space >= (int)ArmySize())
        {
            OnExitTile();
            Destroy(gameObject);
        }
        else
        {
            while (space > 0 && regiments.Count > 0)
            {
                if (space >= regiments[0].size)
                {
                    space -= regiments[0].size;
                    regiments.RemoveAt(0);
                }
                else
                {
                    regiments[0].size -= space;
                    space = 0;
                }
            }
            if (regiments.Count == 0)
            {
                OnExitTile();
                Destroy(gameObject);
            }
        }
    }
    public void DetatchMercs()
    {
        List<Regiment> newRegiments = new List<Regiment>();
        List<Regiment> temp = regiments.ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            if (temp[i].mercenary)
            {
                newRegiments.Add(temp[i]);
                regiments.Remove(temp[i]);
            }
        }
        if (newRegiments.Count > 0)
        {
            Army a = NewArmy(tile, civID, newRegiments);
            a.lastPos = lastPos;
        }
        if(regiments.Count <= 0)
        {
            OnExitTile();
            Destroy(gameObject);
        }
    }
    public bool CanMoveHostileZOC(Vector3Int moveTo,Vector3Int moveFrom)
    {
        if (exiled || civID == -1) { return true; }
        TileData To = Map.main.GetTile(moveTo);
        TileData From = Map.main.GetTile(moveFrom);
        Civilisation civ = Game.main.civs[civID];
        if(
            ((From.occupied && From.civID == civID )|| From.HasNeighboringActiveOccupiedFort(civID) || From.civID != civID) 
            && From.hasZOC &&
            (From.HasNeighboringActiveFort(civID)|| (civ.atWarWith.Contains(From.civID) && From.hasFort && !From.occupied))
            )
        {
            if(To.civID == civID && !To.HasNeighboringActiveOccupiedFort(civID))
            {
                return true;
            }
            if (moveTo == lastPos || moveTo == lastPosNoZOC)
            {
                return true;
            }
            if (To.hasFort && !From.hasFort)
            {
                return true;
            }
            else if (To.hasFort && From.hasFort)
            {
                if(To.civID != From.civID)
                { return false; }
                if (!From.occupied)
                { return false; }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}
