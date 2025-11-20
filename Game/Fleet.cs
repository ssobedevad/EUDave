using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fleet : MonoBehaviour
{
    public int civID;
    public bool inBattle;
    public List<Boat> boats = new List<Boat>();
    public List<Regiment> army = new List<Regiment>();
    public List<Fleet> mergeTargets = new List<Fleet>();
    public bool retreating;
    public bool isMerging;
    public List<Vector3Int> path = new List<Vector3Int>();
    public float moveTimer, moveTime;
    public bool exiled;
    public bool isMercenary;
    public General general;
    public int timeAtSea;
    public Vector3Int pos => Map.main.tileMapManager.tilemap.WorldToCell(transform.position);
    public Vector3Int lastPos;

    public TileData tile => Map.main.GetTile(pos);

    [SerializeField] Transform moveArrowRotatePoint;
    [SerializeField] Image MoveArrowFill;

    public static Fleet NewFleet(TileData tile, int civID, List<Boat> boat, bool merc = false)
    {
        Fleet a = Instantiate(Map.main.boatPrefab, tile.worldPos(), Quaternion.identity, Map.main.unitTransform).GetComponent<Fleet>();       
        a.civID = civID;
        a.boats.AddRange(boat);
        a.isMercenary = merc;
        tile.fleetsOnTile.Add(a);
        FleetUIProvince uIProvince = Instantiate(UIManager.main.FleetUIPrefab, tile.worldPos(), Quaternion.identity, UIManager.main.unitCanvas).GetComponent<FleetUIProvince>();
        uIProvince.fleet = a;
        UIManager.main.WorldSpaceUI.Add(uIProvince.gameObject);
        return a;
    }
    public int CombatWidth()
    {
        int width = 0;
        foreach (var unit in boats)
        {
            width += unit.width;
        }
        return width;
    }
    public void Split()
    {
        if (boats.Count == 1) { return; }
        List<Boat> newBoats = new List<Boat>();
        List<Boat> temp = boats.ToList();
        for (int i = temp.Count / 2; i < temp.Count; i++)
        {
            newBoats.Add(temp[i]);
            boats.Remove(temp[i]);
        }
        if (newBoats.Count > 0)
        {
            Fleet a = NewFleet(tile, civID, newBoats);
            a.lastPos = lastPos;
        }
    }
    public float NavyStrength()
    {
        float strength = 0;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            foreach (var unit in boats)
            {
                if (unit.type < 0 || unit.type >= civ.boats.Count) { continue; }
                float power = (float)unit.sailors;
                power *= (0.01f + unit.cannons * 0.5f);
                power *= Mathf.Max((1f + ((general != null && general.active) ? general.meleeSkill * 0.1f : 0f))
                    , (1f + ((general != null && general.active) ? general.flankingSkill * 0.1f : 0f))
                    , (1f + ((general != null && general.active) ? general.rangedSkill * 0.1f : 0f)));
                power *= 1f + civ.units[unit.type].combatAbility.value;
                strength += power;
            }
        }

        return strength;
    }
    public void EnterBattle()
    {
        inBattle = true;
        foreach (var boat in boats)
        {
            boat.inBatle = true;
        }
    }
    public void ExitBattle()
    {
        inBattle = false;
        foreach (var boat in boats)
        {
            boat.inBatle = false;
        }
        if(boats.Count == 0)
        {
            OnExitTile();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            foreach (var boat in boats)
            {
                boat.cannons = (int)civ.boats[boat.type].cannons.value;
                boat.supplyMax = civ.boats[boat.type].supply.value;
                boat.hullStrengthMax = civ.boats[boat.type].hullStrength.value;
            }
            if (tile.civID != civID && !tile.terrain.isSea)
            {
                if (!Army.HasAccess(tile.civID,civ) && !civ.atWarWith.Contains(tile.civID))
                {
                    exiled = true;
                }
            }
            else
            {
                if (exiled && !tile.terrain.isSea)
                {
                    exiled = false;
                    path.Clear();
                }
            }
        }
        if (Player.myPlayer.selectedFleets.Count == 1 && Player.myPlayer.selectedFleets[0] == this)
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
        if (inBattle)
        {
            if (Player.myPlayer.selectedFleets.Contains(this))
            {
                Player.myPlayer.selectedNavalBattle = Game.main.ongoingNavalBattles.Find(i => i.GetInvolvedFleets().Contains(this));
            }
        }
        else
        {
            CheckBattle();
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
                foreach (var target in mergeTargets.ToList())
                {
                    if (target.path.Count == 0)
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
    private void Start()
    {
        moveTime = 12f;
        if (civID > -1)
        {
            Game.main.civs[civID].fleets.Add(this);
        }
        Game.main.tenMinTick.AddListener(UpdateMovement);
        Game.main.dayTick.AddListener(DayTick);
    }
    public float GetAttrition()
    {
        float percent = 0f;
        float armysize = army.Count * 1000f;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            if (!tile.terrain.isSea)
            {
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
                    percent += tile.localAttritionForEnemies.value;
                    percent += tile.civ.attritionForEnemies.value;
                }
            }
            else
            {

            }
        }
        if (armysize / 1000f > tile.supplyLimit)
        {
            percent += Mathf.Clamp((armysize / 1000f - tile.supplyLimit) * (10f / ((float)tile.supplyLimit)), 0f, 5f);
        }
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            percent *= (1f + civ.landAttrition.value);
        }
        return percent;
    }
    void DayTick()
    {
        if (tile.terrain.isSea)
        {
            timeAtSea++;
        }
        else
        {
            timeAtSea = 0;
            if(tile.civID == civID || tile.civID == -1)
            {
                boats.ForEach(i => i.supply = Mathf.Clamp(i.supply + tile.supplyLimit,0, i.supplyMax));
                boats.ForEach(i => i.hullStrength = Mathf.Clamp(i.hullStrength + tile.totalDev/10f, 0, i.hullStrengthMax));
                boats.ForEach(i => i.RefillSailors());
            }
        }
    }
    public void OnExitTile()
    {
        tile.fleetsOnTile.Remove(this);
    }
    public void OnDestroy()
    {
        if (civID > -1)
        {
            Game.main.civs[civID].fleets.Remove(this);
        }
    }
    public void OnEnterTile()
    {
        tile.fleetsOnTile.Add(this);
        if (!tile.terrain.isSea)
        {
            if(army.Count > 0)
            {
                Army.NewArmy(tile,civID, army);
                army.Clear();
            }
            if(tile.civID != civID || tile.occupied && tile.civID != civID)
            {
                SetPath(tile.portTile);
            }
        }
    }
    public void CombineInto(Fleet fleet)
    {
        if (fleet.civID == civID)
        {
            fleet.boats.AddRange(boats);
            boats.Clear();
            OnExitTile();
            Destroy(gameObject);
        }
    }
    void CheckBlockade()
    {
        TileData tileData = Map.main.GetTile(pos);
        if (civID == -1 || tileData.fleetsOnTile.Count == 0) { return; }
        Civilisation civ = Game.main.civs[civID];
    }
    void CheckBattle()
    {
        TileData tileData = Map.main.GetTile(pos);
        if (civID == -1 || tileData.fleetsOnTile.Count == 0) { return; }
        Civilisation civ = Game.main.civs[civID];
        if (tileData._navalBattle != null)
        {
            if (tileData._navalBattle.attackerCiv.CivID == civID || civ.atWarWith.Contains(tileData._navalBattle.defenderCiv.CivID))
            {
                tileData._navalBattle.AddToBattle(this, true);
            }
            else if (tileData._navalBattle.defenderCiv.CivID == civID || civ.atWarWith.Contains(tileData._navalBattle.attackerCiv.CivID))
            {
                tileData._navalBattle.AddToBattle(this, false);
            }
        }
        if (tileData._navalBattle == null && !retreating && !exiled)
        {
            List<Fleet> defenders = new List<Fleet>();
            List<Fleet> attackers = new List<Fleet>();
            bool isAttacker = true;
            if (civ.atWarWith.Count > 0 && tileData.fleetsOnTile.Exists(i => civ.atWarWith.Contains(i.civID)))
            {
                if (isAttacker)
                {
                    defenders = tileData.fleetsOnTile.FindAll(i => civ.atWarWith.Contains(i.civID) && !i.retreating && !i.exiled);
                    attackers = tileData.fleetsOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders, isAttacker);
                    Debug.Log("Start Naval Battle Attacker");
                }
                else
                {
                    attackers = tileData.fleetsOnTile.FindAll(i => civ.atWarWith.Contains(i.civID) && !i.retreating && !i.exiled);
                    defenders = tileData.fleetsOnTile.FindAll(i => i.civID == civID && !i.retreating && !i.exiled);
                    StartBattle(attackers, defenders, isAttacker);
                    Debug.Log("Start Naval Battle Defender");
                }
            }
        }
    }
    void StartBattle(List<Fleet> attackers, List<Fleet> defenders, bool isAttacker)
    {
        if (defenders.Count > 0 && attackers.Count > 0)
        {
            War war = Game.main.ongoingWars.Find(i => i.Between(defenders[0].civID, civID));
            NavalBattle newBattle;
            if (war != null)
            {
                newBattle = new NavalBattle(pos, attackers[0], defenders[0], warID: war.WarID);
            }
            else
            {
                newBattle = new NavalBattle(pos, attackers[0], defenders[0]);
            }

            if (defenders.Count > 0)
            {
                for (int i = 0; i < defenders.Count; i++)
                {
                    Fleet defender = defenders[i];
                    if (!defender.retreating && !newBattle.GetInvolvedFleets().Contains(defender))
                    {
                        newBattle.AddToBattle(defender, false);
                    }
                }
            }
            if (attackers.Count > 0)
            {
                for (int i = 0; i < attackers.Count; i++)
                {
                    Fleet attacker = attackers[i];
                    if (!attacker.retreating && !newBattle.GetInvolvedFleets().Contains(attacker))
                    {
                        newBattle.AddToBattle(attacker, true);
                    }
                }
            }

        }
    }
    public void UpdateMovement()
    {
        TileData tileData = Map.main.GetTile(pos);
        float movementSpeed = 1f;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            movementSpeed += civ.movementSpeed.value;
        }
        if (general != null && general.active)
        {
            movementSpeed += general.maneuverSkill * 0.05f;
        }
        if (!inBattle)
        {
            if (path.Count > 0)
            {              
                if (moveTimer >= moveTime)
                {
                    if (Pathfinding.CanMoveToTile(pos,path.First(), true))
                    {                       
                        Vector3Int target = path.First();
                        TileData targetTile = Map.main.GetTile(target);
                        if (targetTile.civID > -1)
                        {
                            if (army.Count <= 0 && (targetTile.civID != civID || targetTile.occupied && targetTile.civID != civID)) { path.Clear(); return; }
                        }
                        path.RemoveAt(0);

                        OnExitTile();
                        transform.position = Map.main.tileMapManager.tilemap.CellToWorld(target);
                        OnEnterTile();
                        moveTimer = 0;
                    }
                }
                else
                {
                    moveTimer += movementSpeed * (retreating ? 2f : 1f);                    
                    TileData targetTile = Map.main.GetTile(path.First());
                    moveTime = Mathf.Max(24f * (1f - (targetTile.terrain != null ? targetTile.terrain.moveSpeedMod : 0f)), 1f);
                }
            }
            if (path.Count == 0)
            {
                retreating = false;
                moveTimer = 0;
            }
        }
    }
    public bool SetPath(Vector3Int destination)
    {
        if (retreating) { return false; }
        Vector3Int[] newPath = new Vector3Int[0];
        if (moveTimer < moveTime * 0.5f)
        {
            path.Clear();
            newPath = Pathfinding.FindBestPath(pos, destination,isBoat: true,fleet: this);
        }
        else if (path.Count > 0)
        {
            Vector3Int goal = path.First();
            path.Clear();
            path.Add(goal);
            newPath = Pathfinding.FindBestPath(goal, destination, isBoat: true, fleet: this);
        }
        if (newPath.Length > 0)
        {
            path.AddRange(newPath.ToList());
            return true;
        }
        else
        {
            return false;
        }
    }
    public float TotalSailors()
    {
        float morale = 0;
        int count = 0;
        if (boats.Count > 0)
        {
            foreach (var reg in boats)
            {
                if (reg.sailors > 0)
                {
                    morale += reg.sailors;
                    count++;
                }
            }
        }
        if(count == 0)
        {
            return 0;
        }
        return morale / count;
    }
    public float TotalMaxSailors()
    {
        float morale = 0;
        int count = 0;
        if (boats.Count > 0)
        {
            foreach (var reg in boats)
            {
                if (reg.sailors > 0)
                {
                    morale += reg.maxSailors;
                    count++;
                }
            }
        }
        if (count == 0)
        {
            return 0;
        }
        return morale / boats.Count;


    }
    public Vector3Int RetreatProvince()
    {
        if (civID == -1) { return tile.pos; }
        Civilisation civ = Game.main.civs[civID];
        Vector3Int retreat = civ.capitalPos;
        List<TileData> possible = civ.civCoastalTiles.ToList();
        List<Vector3Int> enemyPos = new List<Vector3Int>();
        for (int i = 0; i < civ.atWarWith.Count; i++)
        {
            Civilisation civ2 = Game.main.civs[civ.atWarWith[i]];
            possible.AddRange(civ2.civCoastalTiles);
            for (int j = 0; j < civ2.armies.Count; j++)
            {
                Vector3Int pos = civ2.armies[j].pos;
                if (!enemyPos.Contains(pos))
                {
                    enemyPos.Add(pos);
                }
            }
        }
        List<float> score = new List<float>();
        foreach (var tile in possible)
        {
            int dist = TileData.evenr_distance(pos, tile.pos);
            float tileScore = dist < 10 ? dist * 10 : 50;
            if (tile.civID == civID)
            {
                foreach (var enemy in enemyPos)
                {
                    dist = TileData.evenr_distance(enemy, tile.pos);
                    if (dist < 2)
                    {
                        tileScore += -100;
                    }
                    else
                    {
                        tileScore += Mathf.Pow(dist, 2);
                    }
                }
                if (tile.occupied || tile.underSiege || Pathfinding.FindBestPath(pos, tile.pos,fleet: this,isBoat:true).Length == 0)
                {
                    score.Add(-1000);
                    continue;
                }
            }
            else
            {
                tileScore -= 100;
            }
            tileScore += tile.pos == civ.capitalPos ? 10 : 0;
            tileScore += tile.totalDev;
            score.Add(tileScore);
        }
        float bestScoreSoFar = -1000;
        for (int i = 0; i < score.Count; i++)
        {
            if (score[i] > bestScoreSoFar)
            {
                retreat = possible[i].pos;
                bestScoreSoFar = score[i];
            }
        }
        return retreat;
    }
    public void SetRetreat(Vector3Int destination)
    {
        retreating = true;
        foreach (var boat in boats)
        {
            boat.sailors = Mathf.Min(boat.sailors, 50);
        }
        path.Clear();
        var newPath = Pathfinding.FindBestPath(pos, destination, fleet: this,isBoat: true);
        if (newPath.Length > 0)
        {
            path.AddRange(newPath.ToList());
        }
    }
}
