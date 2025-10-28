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
        Debug.LogWarning("Unknown bend type " + prevDir + " " + nextDir);
        return 2;
    }
    public static List<Vector3Int> GetPerimFrom(Vector3Int startPos, List<Vector3Int> area)
    {
        List<Vector3Int> perimeterCube = new List<Vector3Int>();
        List<Vector3Int> areaCube = area.ConvertAll(i => TileData.evenr_to_cube(i));
        int travelDirection = 5;
        Vector3Int currentCellCube = TileData.evenr_to_cube(startPos);
        bool found = false;
        int loops = 0;
        while (found == false && loops < area.Count * 10)
        {
            List<int> directions = DirectionPriority(travelDirection);
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
            Debug.Log("Max Loops " + perimeterCube.Count);
            return perimeterCube.ConvertAll(i => TileData.cube_to_evenr(i));
        }
        return perimeterCube.ConvertAll(i => TileData.cube_to_evenr(i));
    }
    public static List<Vector3Int> GetPerimeter(List<Vector3Int> area)
    {
        if(area.Count == 0) { return new List<Vector3Int>(); }
        if(area.Count == 1) { return area; }
       
        area.Sort((x, y) => y.y.CompareTo(x.y));
        int smallestY = area[0].y;
        List<Vector3Int> highest = area.FindAll(i => i.y == smallestY);
        highest.Sort((x, y) => y.x.CompareTo(x.x));
        Vector3Int startPos = highest[0];
        List<Vector3Int> perim = GetPerimFrom(startPos, area);  
        if(highest.Count > 1)
        {
            List<Vector3Int> perimLeft = GetPerimFrom(highest[highest.Count-1], area);
            if(perimLeft.Distinct().Count() > perim.Distinct().Count())
            {
                perim = perimLeft;
            }
        }
        return perim;
    }
    public static List<int> DirectionPriority(int current)
    {
        List<int> prio = new List<int>();
        prio.Add(Shift(current,1));
        prio.Add(Shift(current, 0));
        prio.Add(Shift(current, -1));
        prio.Add(Shift(current, -2));
        prio.Add(Shift(current, -3));
        prio.Add(Shift(current, -4));
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
