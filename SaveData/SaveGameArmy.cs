using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[System.Serializable] public class SaveGameArmy
{
    public int civID;
    public bool inBattle;
    public bool retreating;
    public bool exiled;
    public float moveTimer;
    public float moveTime;

    public General general;

    public Vector3Int pos;
    public Vector3Int lastPos;
    public Vector3Int lastPosNoZOC;

    public List<Vector3Int> path;

    public List<Regiment> regiments;

    public SaveGameArmy()
    {

    }

    public SaveGameArmy(Army army)
    {
        civID = army.civID;
        inBattle = army.inBattle;
        retreating = army.retreating;
        exiled = army.exiled;
        moveTimer = army.moveTimer;
        moveTime = army.moveTime;

        general = army.general;

        pos = army.pos;
        lastPos = army.lastPos;
        lastPosNoZOC = army.lastPosNoZOC;

        path = army.path;

        regiments = army.regiments;
    }
    public Army NewArmy()
    {
        return Army.NewArmy(this);
    }
    public void LoadToArmy(Army army)
    {
        army.civID = civID;
        army.inBattle = inBattle;
        army.retreating = retreating;
        army.exiled = exiled;
        army.moveTimer = moveTimer;
        army.moveTime = moveTime;
        army.general = general;

        army.transform.position = Map.main.tileMapManager.tilemap.CellToWorld(pos);
        army.lastPos = lastPos;
        army.lastPosNoZOC = lastPosNoZOC;

        army.path = path;

        army.regiments = regiments;
    }
}