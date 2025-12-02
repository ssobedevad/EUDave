using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameGreatProj
{
    public SaveGameVector3Int pos;
    public int level;
    public int upgradeTimer;
    
    public SaveGameGreatProj()
    {
    }

    public SaveGameGreatProj(GreatProject proj,Vector3Int tilePos)
    {
        pos = new SaveGameVector3Int(tilePos);
        level = proj.tier;
        upgradeTimer = proj.buildTimer;

    }

    public void SetupProject()
    {
        TileData data = Map.main.GetTile(pos.GetVector3Int());
        if(data.greatProject != null && data.civID > -1)
        {
            GreatProject proj = data.greatProject;
            proj.tier = level;
            proj.buildTimer = upgradeTimer;
            if(upgradeTimer > -1)
            {
                proj.isBuilding = true;
            }
            if (proj.CanUse(data.civ))
            {
                proj.AddProject(data.civ);
            }
        }
    }

}