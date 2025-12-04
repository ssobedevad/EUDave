using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public static Map main;
    [SerializeField] public TileMapManager tileMapManager;
    [SerializeField] public Transform buildingTransform,fortTransform,capitalTransform,unitTransform,borderTransform,tileTextTransform;
    [SerializeField] public ResourceType[] resourceTypes;
    [SerializeField] public Sprite[] statusSprites;
    [SerializeField] public Sprite portSprite;
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
    [SerializeField] public Religion[] religions;
    [SerializeField] public GovernmentType[] governmentTypes;
    [SerializeField] public MercenaryGroup[] mercenaries;
    [SerializeField] public CasusBelli[] casusBellis;
    [SerializeField] public GameObject capitalIndicatorPrefab,resourceIndicatorPrefab,armyPrefab,fortPrefab,boatPrefab,settlementPrefab,tileTextPrefab;
    public Dictionary<string, TradeRegion> tradeRegions = new Dictionary<string, TradeRegion>();
    public Dictionary<string, Terrain> terrainDict = new Dictionary<string, Terrain>();
    public Dictionary<string,ResourceType> resourceDict = new Dictionary<string,ResourceType>();
    public TileData[,] tiles;
    public int boundsMinX,boundsMinY,boundsMaxX,boundsMaxY;
    [SerializeField] Tilemap resources, terrain,civs,religion;
    [SerializeField] public GameObject civBorderPrefab,civNamePrefab;
    [SerializeField] public Trait[] rulerTraits;

    private void Awake()
    {
        main = this;
        BoundsInt bounds = tileMapManager.tilemap.cellBounds;
        boundsMinX = bounds.xMin;
        boundsMinY = bounds.yMin;
        boundsMaxX = bounds.xMax;
        boundsMaxY = bounds.yMax;
        tiles = new TileData[boundsMaxX - boundsMinX + 1, boundsMaxY - boundsMinY + 1];
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
            if (bounds.Contains(pos))
            {
                tiles[pos.x - boundsMinX, pos.y - boundsMinY] = new TileData(pos, -1);                
            }
            tileMapManager.tilemap.SetTileFlags(pos, TileFlags.None);

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
                if (terrainDict[name].isSea)
                {                   
                    tileMapManager.tilemap.SetTile(pos, tileMapManager.dropped);
                    tileMapManager.tilemap.SetTileFlags(pos, TileFlags.None);
                }
                else if (name == "Mountains" || name == "Glacier")
                {
                    tileMapManager.tilemap.SetTile(pos, tileMapManager.raised);
                    tileMapManager.tilemap.SetTileFlags(pos, TileFlags.None);
                }                
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
        foreach (Vector3Int pos in religion.cellBounds.allPositionsWithin)
        {
            if (religion.GetTile(pos) == null) { continue; }
            string name = religion.GetTile(pos).name;
            int id = -1;
            if (int.TryParse(name, out id))
            {
                GetTile(pos).religion = id;
            }
        }
        foreach (var tile in tiles)
        {
            if (tile != null)
            {
                tile.SetNeighbors();
                tile.selectedTileObj = Instantiate(Game.main.civTileSelected, tile.worldPos(), Quaternion.identity, tileMapManager.transform).GetComponent<SpriteRenderer>();
            }
        }
        Destroy(resources.gameObject);
        Destroy(terrain.gameObject);
        Destroy(civs.gameObject);
        Destroy(religion.gameObject);
    }
    

    public TileData GetTile(Vector3Int pos)
    {
        try
        {
            return tiles[pos.x - boundsMinX, pos.y - boundsMinY];
        }
        catch
        { 
            return null;
        }
    }

    public string GetTileName(Vector3Int pos)
    {
        if (tileMapManager.tilemap.GetTile(pos) == null) { return ""; }
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
