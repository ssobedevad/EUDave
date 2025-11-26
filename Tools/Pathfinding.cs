using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding
{
    public static Vector3Int[] FindBestPath(Vector3Int startPos, Vector3Int destination, Army army = null,Fleet fleet = null , bool isBoat = false)
    {
        if (Map.main.GetTile(startPos) == null || Map.main.GetTile(destination) == null) { Debug.Log("Start or end does not exist start: " + startPos + " end: " + destination); return new Vector3Int[0]; }
        
        
        if (!isBoat &&
            (Map.main.GetTile(startPos).terrain == null || Map.main.GetTile(startPos).terrain.isSea ||
            Map.main.GetTile(destination).terrain == null || Map.main.GetTile(destination).terrain.isSea)
            )
        { 
            Debug.Log("Start or end is sea tile. Start: " + startPos + " End: " + destination);
            return new Vector3Int[0];
        }

        PriorityQueue<Vector3Int, int> frontier = new PriorityQueue<Vector3Int, int>();
        frontier.Enqueue(startPos, 0);


        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        cameFrom[startPos] = startPos;

        Dictionary<Vector3Int, int> cost_so_far = new Dictionary<Vector3Int, int>();
        cost_so_far[startPos] = 0;

        bool found = false;
        int maxLoops = 300;
        int loops = 0;
        while (frontier.Count > 0 && loops < maxLoops)
        {
            Vector3Int current = frontier.Dequeue();
            if (current == destination) { found = true; break; }
            TileData currentTile = Map.main.GetTile(current);
            foreach (var n in currentTile.GetNeighbors())
            {
                TileData tile = Map.main.GetTile(n);
                int newCost = cost_so_far[current] + 1;
                if (!CanMoveToTile(current,n, isBoat)) { continue; }
                if (army != null)
                {
                    if (!army.exiled && army.civID > -1)
                    {                       
                        Civilisation civ = Game.main.civs[army.civID];
                        if (!army.HasAccess(tile.civID)) 
                        {
                            if (tile.civID > -1 && tile.civ.AccessOffer(civ) && !civ.isPlayer && civ.diplomaticCapacity < civ.diplomaticCapacityMax.value)
                            {
                                civ.AccessRequest(tile.civID);
                            }
                            else
                            {
                                continue;
                            }
                        }
                        if (!army.CanMoveHostileZOC(n, current)) { continue; }
                        if (tile.armiesOnTile.Exists(i => civ.atWarWith.Contains(i.civID)) && n != destination && !army.retreating) { continue; }
                    }
                }
                else if (fleet != null)
                {
                    if (!fleet.exiled && fleet.civID > -1)
                    {
                        Civilisation civ = Game.main.civs[fleet.civID];
                        if (!Army.HasAccess(tile.civID,civ)) { continue; }
                        if (tile.civID > -1)
                        {
                            if (fleet.army.Count <= 0 && (tile.civID != civ.CivID || tile.occupied && tile.occupiedByID != civ.CivID)) { continue; }
                        }
                        if (tile.fleetsOnTile.Exists(i => civ.atWarWith.Contains(i.civID)) && n != destination && !fleet.retreating) { continue; }
                    }
                }
                if (!cost_so_far.Keys.Contains(n) || newCost < cost_so_far[n])
                {
                    cost_so_far[n] = newCost;
                    frontier.Enqueue(n, newCost + TileData.evenr_distance(n, destination));
                    cameFrom[n] = current;
                }
            }
            loops++;
        }
        if (!found) 
        { 
            //Debug.Log("Could not find after " + loops + " loops. From: " + startPos + " To: " + destination); 
            return new Vector3Int[0];
        }
        Vector3Int currentPos = destination;
        List<Vector3Int> newPath = new List<Vector3Int>();
        while (currentPos != startPos)
        {
            newPath.Add(currentPos);
            currentPos = cameFrom[currentPos];
        }
        //newPath.Add(cubeStart);
        newPath.Reverse();

        return newPath.ToArray();
    }

    public static int CoringDistance(Vector3Int startPos, Vector3Int destination)
    {
        if (Map.main.GetTile(startPos) == null || Map.main.GetTile(destination) == null) { Debug.Log("Start or end does not exist start: " + startPos + " end: " + destination); return -1; }

        PriorityQueue<Vector3Int, int> frontier = new PriorityQueue<Vector3Int, int>();
        frontier.Enqueue(startPos, 0);

        Dictionary<Vector3Int, int> cost_so_far = new Dictionary<Vector3Int, int>();
        cost_so_far[startPos] = 0;

        bool found = false;
        int maxLoops = 300;
        int loops = 0;
        while (frontier.Count > 0 && loops < maxLoops)
        {
            Vector3Int current = frontier.Dequeue();
            if (current == destination) { found = true; break; }
            TileData currentTile = Map.main.GetTile(current);
            foreach (var n in currentTile.GetNeighbors())
            {
                TileData nTile = Map.main.GetTile(n);
                if (!nTile.terrain.isSea && n != destination) { continue; }
                int newCost = cost_so_far[current] + 1;               
                if (!cost_so_far.Keys.Contains(n) || newCost < cost_so_far[n])
                {
                    cost_so_far[n] = newCost;
                    frontier.Enqueue(n, newCost + TileData.evenr_distance(n, destination));
                }
            }
            loops++;
        }
        if (!found)
        {
            return -1;
        }
        return cost_so_far[destination];
    }
    public static int BestCost_cube(Vector3Int cubeStartPos, Vector3Int cubeEndPos)
    {
        return TileData.cube_distance(cubeStartPos, cubeEndPos);
    }
    public static Vector3Int[] StraightPath(Vector3Int from, Vector3Int to)
    {
        Vector3Int cubeStartPos = TileData.evenr_to_cube(from);
        Vector3Int cubeEndPos = TileData.evenr_to_cube(to);
        int dist = TileData.cube_distance(cubeStartPos, cubeEndPos);
        Vector3Int[] newPath = new Vector3Int[dist + 1];
        Vector3 worldPosStart = Map.main.tileMapManager.tilemap.CellToWorld(from);
        Vector3 worldPosEnd = Map.main.tileMapManager.tilemap.CellToWorld(to) + new Vector3(0.01f, 0.01f, 0f);
        newPath[0] = from;
        for (int i = 1; i < dist + 1; i++)
        {
            Vector3 targetPos = worldPosStart + (worldPosEnd - worldPosStart) * 1f / dist * i + new Vector3(0.01f, 0.01f, 0f);
            newPath[i] = Map.main.tileMapManager.tilemap.WorldToCell(targetPos);
        }
        return newPath;
    }

    public static bool CanMoveToTile(Vector3Int from,Vector3Int pos, bool isBoat)
    {
        if (Map.main.tileMapManager.tilemap.GetTile(pos) == null) { return false; }
        TileData tileData = Map.main.GetTile(pos);
        TileData currentTile = Map.main.GetTile(from);
        if (tileData.terrain == null) { return false; }
        if (isBoat)
        {
            return (currentTile.terrain.isSea && tileData.terrain.isSea) || (tileData.isCoastal && from == tileData.portTile) || (currentTile.isCoastal && pos == currentTile.portTile);
        }
        else
        {
            return !tileData.terrain.isSea;
        }
    }
}
