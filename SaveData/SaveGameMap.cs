using MessagePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameMap
{
    public SaveGameTile[] tiles;
    public SaveGameGreatProj[] projects;
    public SaveGameMap()
    {       
    }

    public void SaveTiles()
    {
        TileData[,] tileMap = Map.main.tiles;
        BoundsInt bounds = Map.main.tileMapManager.tilemap.cellBounds;
        int boundsMinX = bounds.xMin;
        int boundsMinY = bounds.yMin;
        int boundsMaxX = bounds.xMax;
        int boundsMaxY = bounds.yMax;
        List<SaveGameTile> tileList = new List<SaveGameTile>();
        List<SaveGameGreatProj> projList = new List<SaveGameGreatProj>();
        foreach (Vector3Int pos in Map.main.tileMapManager.tilemap.cellBounds.allPositionsWithin)
        {
            if (bounds.Contains(pos))
            {
                TileData tile = tileMap[pos.x - boundsMinX, pos.y - boundsMinY];
                tileList.Add(new SaveGameTile(tile));
                if(tile.greatProject != null)
                {
                    projList.Add(new SaveGameGreatProj(tile.greatProject,pos));
                }
            }      
        }
        projects = projList.ToArray();
        tiles = tileList.ToArray();
    }
    public void LoadTiles()
    {
        BoundsInt bounds = Map.main.tileMapManager.tilemap.cellBounds;
        int boundsMinX = bounds.xMin;
        int boundsMinY = bounds.yMin;
        int boundsMaxX = bounds.xMax;
        int boundsMaxY = bounds.yMax;
        int index = 0;
        int projIndex = 0;
        foreach (Vector3Int pos in Map.main.tileMapManager.tilemap.cellBounds.allPositionsWithin)
        {
            if (bounds.Contains(pos))
            {
                TileData tile = Map.main.tiles[pos.x - boundsMinX, pos.y - boundsMinY];
                tiles[index].LoadToTile(tile);
                if (tile.greatProject != null)
                {
                    projects[projIndex].SetupProject();
                    projIndex++;
                }
                index++;
            }
        }
    }
}