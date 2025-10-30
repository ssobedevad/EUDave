using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Boat : MonoBehaviour
{
    public int civID;
    public bool inBattle;
    public bool retreating;
    public bool isMerging;
    public List<Army> mergeTargets = new List<Army>();
    public List<Vector3Int> path = new List<Vector3Int>();
    public float moveTimer, moveTime;
    public bool exiled;
    public bool isMercenary;
    public General general;
    public Vector3Int pos => Map.main.tileMapManager.tilemap.WorldToCell(transform.position);
    public Vector3Int lastPos;

    public TileData tile => Map.main.GetTile(pos);

    [SerializeField] Transform moveArrowRotatePoint;
    [SerializeField] Image MoveArrowFill;

    private void Update()
    {
        if (Player.myPlayer.selectedBoats.Count == 1 && Player.myPlayer.selectedBoats[0] == this)
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
    }
    public void OnExitTile()
    {

    }
    public void OnEnterTile()
    {

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
                    if (Pathfinding.CanMoveToTile(path.First(), false))
                    {
                        Vector3Int target = path.First();
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
}
