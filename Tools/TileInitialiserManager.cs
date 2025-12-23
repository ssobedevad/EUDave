using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInitialiserManager : MonoBehaviour
{
    [SerializeField] string[] names;
    [SerializeField] GameObject[] Regions;
    [SerializeField] GameObject dataPrefab;
    [SerializeField] Tilemap civMap;
    [EditorButton]
    void NameAllTiles()
    {
        var objects = GetComponentsInChildren<TileDataInit>();
        foreach (var obj in objects)
        {
            obj.name = obj.Name;
        }
    }

    [EditorButton]
    void AddDevelopmentNoise(int region = -1)
    {
        var objects = region > -1 ? Regions[region].GetComponentsInChildren<TileDataInit>() : GetComponentsInChildren<TileDataInit>();
        foreach (var obj in objects)
        {
            int index = UnityEngine.Random.Range(0, 3);
            int change = UnityEngine.Random.Range(-1,2);
            if (index == 0)
            {
                obj.developmentA = Mathf.Max(0, obj.developmentA + change);
            }
            else if(index == 1)
            {
                obj.developmentB = Mathf.Max(0, obj.developmentB + change);
            }
            else if (index == 2)
            {
                obj.developmentC = Mathf.Max(0, obj.developmentC + change);
            }
        }
    }

    [EditorButton]
    void ExtractRegion(int region)
    {
        var tiles = Regions[region].GetComponentsInChildren<TileDataInit>();
        string gamePath = "Assets/Assets/Data/TileData_Region_"+region+".txt";
        string text = "";
        foreach(var tile in tiles)
        {
            text += tile.Name + ",";
        }
        File.WriteAllText(gamePath, text);
    }

    [EditorButton]
    void ImportRegion(int region)
    {
        var tiles = Regions[region].GetComponentsInChildren<TileDataInit>();
        string gamePath = "Assets/Assets/Data/TileData_Region_" + region + ".txt";
        try
        {
            string data = File.ReadAllText(gamePath);
            string[] nameData = data.Split(',');
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].Name = nameData[i];
            }
        }
        catch
        {
            Debug.Log("Invalid Data For Region " + region + " count: " + tiles.Length);
        }

    }
    [EditorButton]
    void AddDatas(int region, int baseDev, string baseName, string regionName, string continentName, string tradeRegionName)
    {
        Transform regionObject = Regions[region].transform;
        var tiles = GetComponentsInChildren<TileDataInit>().ToList();
        int index = 1;
        List<Vector3Int> possible = new List<Vector3Int>();
        foreach (var tile in civMap.cellBounds.allPositionsWithin)
        {
            if (civMap.GetTile(tile) != null)
            {
                possible.Add(tile);
            }
        }
        foreach(var tile in tiles)
        {
            Vector3Int tilePos = civMap.WorldToCell(tile.transform.position);
            if (possible.Contains(tilePos))
            {
                possible.Remove(tilePos);
            }
        }
        foreach(var tile in possible)
        {
            TileDataInit tileData = Instantiate(dataPrefab, civMap.CellToWorld(tile), Quaternion.identity, regionObject).GetComponent<TileDataInit>();
            tileData.Name = baseName + index;
            tileData.Region = regionName;
            tileData.Continent = continentName;
            tileData.TradeRegion = tradeRegionName;
            tileData.developmentA = baseDev / 3;
            tileData.developmentB = baseDev / 3;
            tileData.developmentC = baseDev / 3;
            Debug.Log("ADDED TILE " + index);
            index++;
        }
    }
}

