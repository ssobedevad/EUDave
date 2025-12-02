using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameArmy
{
    public int id;
    public bool b;
    public bool r;
    public bool e;
    public float tr;
    public float t;
    public float s;

    public General g;

    public SaveGameVector3Int p;
    public SaveGameVector3Int lp;
    public SaveGameVector3Int nz;

    public List<SaveGameVector3Int> pt;

    public List<Regiment> rs;

    public SaveGameArmy()
    {

    }

    public SaveGameArmy(Army army)
    {
        id = army.civID;
        b = army.inBattle;
        r = army.retreating;
        e = army.exiled;
        tr = army.moveTimer;
        t = army.moveTime;
        if (army.tile.underSiege)
        {
            if (army.tile.siege.armiesSieging.Contains(army))
            {
                s = army.tile.siege.progress;
            }
        }
        g = army.general;

        p = new SaveGameVector3Int(army.pos);
        lp = new SaveGameVector3Int(army.lastPos);
        nz = new SaveGameVector3Int(army.lastPosNoZOC)    ;

        pt = army.path.ConvertAll(i=>new SaveGameVector3Int(i));

        rs = army.regiments;
    }
    public Army NewArmy()
    {
        return Army.NewArmy(this);
    }
    public void LoadToArmy(Army army)
    {
        army.civID = id;
        army.inBattle = b;
        army.retreating = r;
        army.exiled = e;
        army.moveTimer = tr;
        army.moveTime = t;
        army.general = g;

        army.transform.position = Map.main.tileMapManager.tilemap.CellToWorld(p.GetVector3Int());
        army.lastPos = lp.GetVector3Int();
        army.lastPosNoZOC = nz.GetVector3Int();

        army.path = pt.ConvertAll(i=>i.GetVector3Int());

        army.regiments = rs;
    }
}