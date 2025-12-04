using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PerimeterHelper
{
    public const float outerRadius = 0.95f;
    public const float innerRadius = outerRadius;
    public static List<Vector3> corners = new List<Vector3>(){
    {new Vector3(0f, outerRadius, 0f)},
    {new Vector3(innerRadius, 0.5f * outerRadius, 0f)},
    {new Vector3(innerRadius, -0.5f * outerRadius, 0f)},
    {new Vector3(0f, -outerRadius, 0f)},
    {new Vector3(-innerRadius, -0.5f * outerRadius, 0f)},
    {new Vector3(-innerRadius, 0.5f * outerRadius, 0f)},
  };
    static void AddDir(List<Vector3> ret, Vector3Int cell, int dir)
    {
        ret.Add(corners[dir] + Map.main.GetTile(cell).worldPos());
    }
    public static List<Vector3> GetLinePositions(List<Vector3Int> perimeter)
    {
        List<Vector3> vertices = new List<Vector3>();
        int lastDir;
        int nextDir;
        if(perimeter.Count == 0) { return new List<Vector3>();  }
        if (perimeter.Count == 1) 
        {
            int currentCorner = startingCornerMap[0];
            for (int i = 0; i < 6; i++)
            {
                AddDir(vertices, perimeter[0], currentCorner);
                currentCorner = Shift(currentCorner, -1);
            }
            return vertices;
        }
        List<Vector3Int> perimeterCube = perimeter.ConvertAll(i => TileData.evenr_to_cube(i));
        Vector3Int firstCellCube = perimeterCube[0];
        Vector3Int lastCellCube = perimeterCube[perimeterCube.Count - 1];
        Vector3Int nextCellCube;
        for(int p = 0; p < perimeterCube.Count; p++)
        {
            Vector3Int pos = perimeterCube[p];
            if(p > 0)
            {
                lastCellCube = perimeterCube[p - 1];
            }
            lastDir = TileData.cube_direction_vectors.ToList().IndexOf(pos - lastCellCube);
            if(p+1 < perimeterCube.Count)
            {
                nextCellCube = perimeterCube[p+1];
            }
            else
            {
                nextCellCube = firstCellCube;
            }
            nextDir = TileData.cube_direction_vectors.ToList().IndexOf(nextCellCube - pos);

            int bendVertices = GetBendVertices(lastDir, nextDir);

            int currentCorner = startingCornerMap[lastDir];
            for (int i = 0; i < bendVertices; i++)
            {
                AddDir(vertices, TileData.cube_to_evenr(pos), currentCorner);
                currentCorner = Shift(currentCorner,-1);
            }
        }
        return vertices;
    }
    static int[] startingCornerMap = new int[6]{4,5,0,1,2,3};
    enum HexBendType
    {
        Inner = 1,
        Straight = 2,
        Outer = 3,
        Acute = 4,
        Uturn = 5
    }
    public static int GetBendVertices(int prevDir,int nextDir)
    {
        if(prevDir == Shift(nextDir,0))
        {
            return (int)HexBendType.Outer;
        }
        if (prevDir == Shift(nextDir, -1))
        {
            return (int)HexBendType.Straight;
        }
        if (prevDir == Shift(nextDir, -3))
        {
            return (int)HexBendType.Uturn;
        }
        if (prevDir == Shift(nextDir, 2))
        {
            return (int)HexBendType.Acute;
        }
        if (prevDir == Shift(nextDir, 1))
        {
            return (int)HexBendType.Outer;
        }
        if (prevDir == Shift(nextDir, -2))
        {
            return (int)HexBendType.Uturn;
        }
        return 2;
    }
    public static List<Vector3Int> GetPerimFrom(Vector3Int startPos, List<Vector3Int> area)
    {
        List<Vector3Int> perimeterCube = new List<Vector3Int>();
        List<Vector3Int> areaCube = area.ConvertAll(i => TileData.evenr_to_cube(i));
        List<int> directions = new List<int>{ 2, 3,4, 5, 0, 1 };
        int travelDirection = directions[0];
        Vector3Int currentCellCube = TileData.evenr_to_cube(startPos);
        bool found = false;
        int loops = 0;
        foreach (int direction in directions)
        {
            Vector3Int nextCellCube = TileData.cube_neighbour(currentCellCube, direction);            
            if (areaCube.Contains(nextCellCube))
            {
                travelDirection = direction;
                found = true;
                break;
            }
        }
        if(found == false)
        {
            return new List<Vector3Int>() { startPos };
        }
        found = false;
        while (found == false && loops < area.Count * 10)
        {
            directions = DirectionPriority(travelDirection);
            foreach (int direction in directions)
            {
                Vector3Int nextCellCube = TileData.cube_neighbour(currentCellCube, direction);
                if (areaCube.Contains(nextCellCube))
                {
                    
                    perimeterCube.Add(nextCellCube);
                    
                    travelDirection = direction;
                    currentCellCube = nextCellCube;
                    if (nextCellCube == TileData.evenr_to_cube(startPos)) { found = true; }
                    break;
                }
            }
            loops++;
        }
        if (loops >= area.Count * 9)
        {
            return perimeterCube.ConvertAll(i => TileData.cube_to_evenr(i));
        }
        return perimeterCube.ConvertAll(i => TileData.cube_to_evenr(i));
    }
    public static List<List<Vector3Int>> GetPerimeter(List<Vector3Int> area)
    {
        if(area.Count == 0) { return new List<List<Vector3Int>>(); }
        if(area.Count == 1) { return new List<List<Vector3Int>>() { area }; }
        List<Vector3Int> initalArea = area.ToList();
        List<Vector3Int> perim = GetPerimFrom(BestStartPos(area), initalArea);
        List<List<Vector3Int>> perims = new List<List<Vector3Int>>() { perim };
        area.RemoveAll(i => NeigborsInArea(i, initalArea) == 6);
        area.RemoveAll(i => perim.Contains(i));
        int loops = 0;
        while (area.Count > 0 && loops < 50)
        {
            if (area.Count == 1) { perims.Add(area); break; }
            perim = GetPerimFrom(BestStartPos(area), area);
            perims.Add(perim);
            area.RemoveAll(i => perim.Contains(i));
            loops++;
        }
        return perims;
    }
    public static Vector3Int BestStartPos(List<Vector3Int> area)
    {
        area.Sort((x, y) => NeigborsSameCiv(Map.main.GetTile(x)).CompareTo(NeigborsSameCiv(Map.main.GetTile(y))));
        int minNeighbors = NeigborsSameCiv(Map.main.GetTile(area[0]));
        List<Vector3Int> possibleStartPos = area.FindAll(i => NeigborsSameCiv(Map.main.GetTile(i)) == minNeighbors);
        possibleStartPos.Sort((x, y) => y.y.CompareTo(x.y));
        int maxY = possibleStartPos[0].y;
        possibleStartPos = possibleStartPos.FindAll(i => i.y == maxY);
        possibleStartPos.Sort((x, y) => x.x.CompareTo(y.x));
        return possibleStartPos[0];
    }
    public static int NeigborsInArea(Vector3Int pos, List<Vector3Int> area)
    {
        List<Vector3Int> neighbors = Map.main.GetTile(pos).GetNeighbors();
        int inArea = 0;
        foreach(var n in neighbors)
        {
            if (area.Contains(n))
            {
                inArea++;
            }
        }
        return inArea;
    }
    public static int NeigborsSameCiv(TileData tile)
    {
        List<TileData> neighbors = tile.GetNeighborTiles();
        int inArea = 0;
        foreach (var n in neighbors)
        {
            if (tile.civID == n.civID)
            {
                inArea++;
            }
        }
        return inArea;
    }
    public static List<int> DirectionPriority(int current)
    {
        List<int> prio = new List<int>();
        prio.Add(Shift(current,1));
        prio.Add(Shift(current, 0));
        prio.Add(Shift(current, -1));
        prio.Add(Shift(current, -2));
        prio.Add(Shift(current, -3));
        return prio;
    }
    public static int Shift(int current,int amount)
    {
        if(current + amount > 5)
        {
            return current + amount - 6;
        }
        else if (current + amount < 0)
        {
            return (current + amount) + 6;
        }
        return current + amount;
    }
}
