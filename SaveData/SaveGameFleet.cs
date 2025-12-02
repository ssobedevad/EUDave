using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameFleet
{
    public int civID;
    public bool inBattle;
    public bool retreating;
    public bool exiled;
    public float moveTimer;
    public float moveTime;
    public int timeAtSea;

    public General general;

    public SaveGameVector3Int pos;
    public SaveGameVector3Int lastPos;

    public List<SaveGameVector3Int> path;

    public List<Regiment> army;
    public List<Boat> boats;

    public SaveGameFleet()
    {

    }

    public SaveGameFleet(Fleet fleet)
    {
        civID = fleet.civID;
        inBattle = fleet.inBattle;
        retreating = fleet.retreating;
        exiled = fleet.exiled;
        moveTimer = fleet.moveTimer;
        moveTime = fleet.moveTime;
        timeAtSea = fleet.timeAtSea;

        general = fleet.general;

        pos = new SaveGameVector3Int(fleet.pos);
        lastPos = new SaveGameVector3Int(fleet.lastPos);

        path = fleet.path.ConvertAll(i=>new SaveGameVector3Int(i));

        boats = fleet.boats;
        army = fleet.army;
    }
    public Fleet NewFleet()
    {
        return Fleet.NewFleet(this);
    }
    public void LoadToFleet(Fleet fleet)
    {
        fleet.civID = civID;
        fleet.inBattle = inBattle;
        fleet.retreating = retreating;
        fleet.exiled = exiled;
        fleet.moveTimer = moveTimer;
        fleet.moveTime = moveTime;
        fleet.timeAtSea = timeAtSea;

        fleet.general = general;

        fleet.transform.position = Map.main.tileMapManager.tilemap.CellToWorld(pos.GetVector3Int());
        fleet.lastPos = lastPos.GetVector3Int();

        fleet.path = path.ConvertAll(i=>i.GetVector3Int());

        fleet.boats = boats;
        fleet.army = army;
    }
}