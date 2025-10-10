using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

public class Map : MonoBehaviour
{
    public static Map main;
    [SerializeField] public TileMapManager tileMapManager;
    [SerializeField] public Transform indicatorTransform;
    [SerializeField] public ResourceType[] resourceTypes;
    [SerializeField] public Terrain[] terrains;
    [SerializeField] public Advisor[] advisorsA;
    [SerializeField] public Advisor[] advisorsD;
    [SerializeField] public Advisor[] advisorsM;
    [SerializeField] public Tech[] TechA;
    [SerializeField] public Tech[] TechD;
    [SerializeField] public Tech[] TechM;
    [SerializeField] public IdeaGroup[] IdeasA;
    [SerializeField] public IdeaGroup[] IdeasD;
    [SerializeField] public IdeaGroup[] IdeasM;
    [SerializeField] public Building[] Buildings;
    [SerializeField] public EventData[] pulseEvents;
    [SerializeField] public GameObject capitalIndicatorPrefab,resourceIndicatorPrefab,armyPrefab,fortPrefab;
    public Dictionary<string, Terrain> terrainDict = new Dictionary<string, Terrain>();
    public Dictionary<string,ResourceType> resourceDict = new Dictionary<string,ResourceType>();
    public TileData[,] tiles;
    private BoundsInt bounds;
    [SerializeField] Tilemap resources, terrain,civs;

    public bool withinBounds(Vector3Int pos)
    {
        return bounds.Contains(pos); 
    }
    private void Awake()
    {
        main = this;
        bounds = tileMapManager.tilemap.cellBounds;
        tiles = new TileData[bounds.xMax - bounds.xMin + 1, bounds.yMax - bounds.yMin + 1];
        foreach (var terrain in terrains)
        {
            terrainDict.Add(terrain.name, terrain);
        }
        foreach (var resource in resourceTypes)
        {
            resourceDict.Add(resource.name, resource);
        }
        foreach (Vector3Int pos in tileMapManager.tilemap.cellBounds.allPositionsWithin)
        {
            if (withinBounds(pos))
            {
                tiles[pos.x - bounds.xMin, pos.y - bounds.yMin] = new TileData(pos, -1);
            }
        }
        foreach (Vector3Int pos in resources.cellBounds.allPositionsWithin)
        {
            if(resources.GetTile(pos) == null) { continue; }
            string name = resources.GetTile(pos).name;
            if (resourceDict.ContainsKey(name))
            {
                GetTile(pos).tileResource = resourceDict[name];
                ApplyTileResourceLocalModifier(GetTile(pos));
            }
        }
        foreach (Vector3Int pos in terrain.cellBounds.allPositionsWithin)
        {
            if (terrain.GetTile(pos) == null) { continue; }
            string name = terrain.GetTile(pos).name;
            if (terrainDict.ContainsKey(name))
            {
                GetTile(pos).terrain = terrainDict[name];
                GetTile(pos).localDevCost.AddModifier(new Modifier(terrainDict[name].devCostMod, ModifierType.Flat, "Terrain"));
            }
        }
        foreach (Vector3Int pos in civs.cellBounds.allPositionsWithin)
        {
            if (civs.GetTile(pos) == null) { continue; }
            string name = civs.GetTile(pos).name;
            int id = -1;
            if (int.TryParse(name, out id))
            {
                GetTile(pos).civID = id;
                GetTile(pos).cores.Add(id);
            }        
        }
        Destroy(resources.gameObject);
        Destroy(terrain.gameObject);
        Destroy(civs.gameObject);
    }
    

    public TileData GetTile(Vector3Int pos)
    {
        if(!withinBounds(pos)){ Debug.Log(pos + " Not Valid"); return null; }
        return tiles[pos.x - bounds.xMin,pos.y - bounds.yMin];
    }

    public string GetTileName(Vector3Int pos)
    {
        if (tileMapManager.tilemap.GetTile(pos) == null) { Debug.Log(pos + " Not Valid"); return ""; }
        return tileMapManager.tilemap.GetTile(pos).name;
    }

    void ApplyTileResourceLocalModifier(TileData data)
    {
        string modifierName = data.tileResource.localTileEffect;
        float strength = data.tileResource.localTileEffectStrength;
        switch (modifierName)
        {
            case "Fort Defence":
                data.localDefensiveness.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Construction Cost":
                data.localConstructionCost.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Construction Time":
                data.localConstructionTime.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Force Limit":
                data.localForceLimit.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Population Growth":
                data.localPopulationGrowth.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Maximum Population":
                data.localMaxPopulation.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Development Cost":
                data.localDevCost.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Tax Efficiency":
                data.localTaxEfficiency.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Daily Control":
                data.dailyControl.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Production Value":
                data.localProductionValue.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Production Amount":
                data.localProductionQuantity.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Movement Speed":
                data.localMovementSpeed.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Recruitment Cost":
                data.localRecruitmentCost.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Recruitment Time":
                data.localRecruitmentTime.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            case "Attacker Dice Roll":
                data.localAttackerDiceRoll.AddModifier(new Modifier(strength, ModifierType.Flat, "Producing " + data.tileResource.name));
                break;
            default:
                break;
        }
    }
}
