using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable] public class SaveGameMap
{
    public SaveGameTile[,] tiles;
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
        tiles = new SaveGameTile[boundsMaxX - boundsMinX + 1,boundsMaxY - boundsMinY + 1];
        foreach (Vector3Int pos in Map.main.tileMapManager.tilemap.cellBounds.allPositionsWithin)
        {
            if (bounds.Contains(pos))
            {
                TileData tile = tileMap[pos.x - boundsMinX, pos.y - boundsMinY];
                tiles[pos.x - boundsMinX, pos.y - boundsMinY] = new SaveGameTile(tile);
            }      
        }
    }
    public void LoadTiles()
    {
        BoundsInt bounds = Map.main.tileMapManager.tilemap.cellBounds;
        int boundsMinX = bounds.xMin;
        int boundsMinY = bounds.yMin;
        int boundsMaxX = bounds.xMax;
        int boundsMaxY = bounds.yMax;        
        foreach (Vector3Int pos in Map.main.tileMapManager.tilemap.cellBounds.allPositionsWithin)
        {
            if (bounds.Contains(pos))
            {
                TileData tile = Map.main.tiles[pos.x - boundsMinX, pos.y - boundsMinY];
                tiles[pos.x - boundsMinX, pos.y - boundsMinY].LoadToTile(tile);
            }
        }
    }
}