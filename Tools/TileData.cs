using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Unity.VisualScripting.Member;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class TileData
{
    public string Name;
    public Vector3Int pos;
    public int civID;
    public Civilisation civ => Game.main.civs[civID];
    public bool underSiege => siege != null && siege.inProgress;
    public int fortLevel;
    public bool hasFort;
    public bool hasZOC;
    public Siege siege;
    public bool occupied;
    public int occupiedByID;
    public ResourceType tileResource,tileSecondaryResource = null;   
    public Terrain terrain;
    public List<Army> armiesOnTile = new List<Army>();
    public GameObject civTile;
    public GameObject selectedTileObj;
    public GameObject resourceIndicator;
    public Battle _battle = null;
    public int recruitTimer = -1;
    public List<int> recruitQueue = new List<int>();
    public int buildTimer = -1;
    public List<int> buildQueue = new List<int>();
    public string region;
    public int population;
    public int avaliablePopulation;
    public List<int> buildings = new List<int>();
    public List<int> cores = new List<int>();
    public int coreTimer = -1;
    public bool hasCore => cores.Contains(civID);
    public int avaliableMaxPopulation => (int)(maxPopulation * control/100f);
    public int populationGrowth => (int)(developmentA * 6 * (1f + localPopulationGrowth.value + (civID > -1 ? civ.populationGrowth.value : 0f)));
    public int maxPopulation => (int)(totalDev * 200 * (1f + localMaxPopulation.value + (civID > -1 ?civ.maximumPopulation.value : 0f)));
    public int developmentA, developmentB, developmentC;
    public Stat localDevCostMod = new Stat(0f, "Local Development Cost Modifier");
    public Stat localDevCost = new Stat(0f, "Local Development Cost");
    public Stat localProductionValue = new Stat(0f, "Local Production Value");
    public Stat localProductionQuantity = new Stat(0f, "Local Production Quantity");
    public Stat localTaxEfficiency = new Stat(0f, "Local Tax Efficiency");
    public float control;
    public float maxControl;
    public Stat localConstructionCost = new Stat(0f, "Local Construction Cost");
    public Stat localConstructionTime = new Stat(0f, "Local Construction Time");
    public Stat localMovementSpeed = new Stat(0f, "Local Movement Speed");
    public Stat localRecruitmentCost = new Stat(0f, "Local Recruitment Cost");
    public Stat localRecruitmentTime = new Stat(0f, "Local Recruitment Time");
    public Stat dailyControl = new Stat(0.01f, "Daily Control", true);
    public Stat localDefensiveness = new Stat(0f, "Local Defensiveness");
    public Stat localPopulationGrowth = new Stat(0f, "Local Population Growth");
    public Stat localMaxPopulation = new Stat(0f, "Local Max Population");
    public Stat localForceLimit = new Stat(0f, "Local Force Limit");
    public Stat localAttackerDiceRoll = new Stat(0f, "Local Attacker Dice Roll");
    public Stat localUnrest = new Stat(0f, "Local Unrest", true);
    public int seperatism = 0;
    public float unrest => localUnrest.value + (civID > -1 ? civ.globalUnrest.value : 0f);
    public int totalDev => developmentA + developmentB + developmentC;
    public string tileTypeName => Map.main.GetTileName(pos);
    public TileData(Vector3Int Pos,int CivID)
    {
        pos = Pos;
        civID = CivID;
    }
    public bool needsCoring()
    {
        if (hasCore || coreTimer != -1) { return false; }
        return true;
    }
    public void StartBuilding(int id)
    {
        Building building = Map.main.Buildings[id];
        float cost = building.GetCost(this, civ);
        if (civ.coins >= cost)
        {
            civ.coins -= cost;
        }
        else { return; }
        if (buildQueue.Count == 0)
        {           
            buildTimer = (int)building.GetTime(this, civ);
        }
        buildQueue.Add(id);
        
    }
    public void StartRecruiting(int type)
    {
        float cost = GetRecruitCost(type);
        if (civ.coins >= cost)
        {
            civ.coins -= cost;
        }
        else { return; }
        if (recruitQueue.Count == 0)
        {
            recruitTimer = GetRecruitTime();
        }
        recruitQueue.Add(type);
    }
    public int GetRecruitTime()
    {
        return (int)Mathf.Max(144 * (1f + localRecruitmentTime.value + civ.recruitmentTime.value),1);
    }
    public void Build(int id)
    {
        Building building = Map.main.Buildings[id];
        if (!buildings.Contains(id))
        {
            buildings.Add(id);
            if (building.effect.Length > 0)
            {
                ApplyTileLocalModifier(building.effect, building.effectSrength, building.effectType, building.Name);
            }
            if(building.fortLevel > 0)
            {
                fortLevel += building.fortLevel;
                if (!hasFort)
                {
                    GameObject.Instantiate(Map.main.fortPrefab, worldPos(), Quaternion.identity);
                    hasFort = true;
                    ApplyZOC();
                }
            }
        }
    }
    public void StartCore()
    {
        if (civID == -1) { return; }
        if (hasCore || coreTimer != -1) { return; }
        if(civ.adminPower >= GetCoreCost())
        {
            coreTimer = GetCoreTime();
            civ.adminPower -= GetCoreCost();
        }
    }
    public int GetCoreTime()
    {
        int baseTime = 180;
        baseTime = (int)(baseTime * (1f + civ.coreCost.value));
        return Mathf.Max(baseTime,1);
    }
    public int GetCoreCost()
    {
        int baseCost = totalDev * 5;
        baseCost = (int)(baseCost * (1f + civ.coreCost.value));
        return Mathf.Max(baseCost,1);
    }
    public void SetMaxControl()
    {
        float maximumControl = 100f;
        maximumControl -= Mathf.Pow(evenr_distance(pos, civ.capitalPos),2) * (1f + civ.controlDecay.value);
        maximumControl += totalDev;
        maxControl = Mathf.Clamp(maximumControl, 0f, hasCore ? 100f : 10f);
        control = Mathf.Min(control, maxControl);
    }
    public float GetDevProdIncrease()
    {
        if (civID == -1) { return 0; }
        float value = tileResource.Value * (1f + localProductionValue.value + civ.productionValue.value) * (1f + localProductionQuantity.value + civ.productionAmount.value);
        value *= 0.01f;
        if (!hasCore) { value *= 0.25f; }
        return value;
    }
    public float GetAnyDevForceLimitIncrease()
    {
        if (civID == -1) { return 0; }
        float value = 0.2f;
        value *= (float)(200 * control/100f) / 1000f;
        if (!hasCore) { value *= 0.25f; }
        return value;
    }
    public float GetDailyTax()
    {
        if(civID == -1) { return 0; }
        float value = 0.025f * (1f+ localTaxEfficiency.value + civ.taxEfficiency.value);
        value *= 1f/360f;
        value *= avaliablePopulation;
        if (!hasCore) { value *= 0.25f; }
        return value;
    }
    public float GetForceLimit()
    {
        if (civID == -1) { return 0; }
        float value = 0.2f;
        value *= (float)avaliableMaxPopulation/1000f;
        if (!hasCore) { value *= 0.25f; }
        return value + localForceLimit.value * control/100f;
    }
    public float GetRecruitCost(int type)
    {
        if (civID == -1) { return 9999f; }
        float baseCost = civ.units[type].baseCost;
        baseCost *= (1f + localRecruitmentCost.value + civ.regimentCost.value);
        return Mathf.Max(baseCost,1f);
    }
    public void CreateRebelArmy(int size, RebelArmyStats stats)
    {
        if (civID == -1) { return; }
        if (size > 0)
        {
            List<Regiment> regiments = new List<Regiment>();
            while (size > 0)
            {
                int stack = Mathf.Min(1000, size);
                size -= stack;
                Regiment regiment = new Regiment(Size: stack, CivID: -1);
                regiment.maxMorale = stats.morale;
                regiment.morale = stats.morale;
                regiment.meleeDamage = stats.meleeDamage;
                regiments.Add(regiment);
            }
            Game.main.rebelFactions.Add(Army.NewArmy(this, -1, regiments));
            Game.main.rebelStats.Add(stats);
        }
    }
    public void CreateNewArmy(int type)
    {
        if (civID == -1) { return; }
        if (civ.AvaliablePopulation() >= 1000)
        {
            int size = civ.RemovePopulation(1000);
            if (size >= 500)
            {
                List<Regiment> regiments = new List<Regiment> { new Regiment(Size: size, CivID: civID,Type: type) };
                Army.NewArmy(this, civID, regiments);
            }
            
        }
    }
    public float GetDailyProductionValue()
    {
        if (civID == -1) { return 0; }
        if (tileResource == null) { return 0f; }
        float value = tileResource.Value * (1f + localProductionValue.value + civ.productionValue.value) * (1f + localProductionQuantity.value + civ.productionAmount.value);
        value *= (float)Mathf.Clamp(avaliablePopulation,0f, 200f * developmentB) / 10000f;
        if (!hasCore) { value *= 0.25f; }
        return value;
    }
    public void SetDevCost()
    {
        int devCount = totalDev - 9;
        float val = 0;
        while (devCount > 0)
        {
            val += 0.03f * devCount;
            devCount -= 10;
        }
        if (localDevCost.modifiers.Exists(i => i.name == "Development"))
        {
            Modifier modifier = localDevCost.modifiers.Find(i => i.name == "Development");
            modifier.value = val;
            localDevCost.RemoveModifier(modifier);
            localDevCost.AddModifier(modifier);
        }
        else
        {
            Modifier modifier = new Modifier(val, ModifierType.Flat,"Development");
            localDevCost.AddModifier(modifier);
        }
    }
    public float GetWarScore(int forCiv)
    {
        float cost = 5;
        Civilisation civilization = Game.main.civs[forCiv];
        cost += Mathf.Min(30f, totalDev);
        if(civ.capitalPos == pos) { cost += 0.2f * cost; }
        cost *= (1f - Mathf.Min((civ.GetTotalDev() * 0.01f)/ 15f, 0.33f));
        cost *= (1f + civilization.warScoreCost.value);
        return cost;
    }
    public void AddDevelopment(int index,int fromCiv = -1)
    {
        if (civID == -1) { return; }
        if (!hasCore) { return; }
        int devCost = GetDevCost(fromCiv);
        bool buy = false;
        if (index == 0 && civ.adminPower >= devCost)
        {
            civ.adminPower -= devCost;
            developmentA++;
            buy = true;
        }
        else if(index == 1 && civ.diploPower >= devCost)
        {
            civ.diploPower -= devCost;
            developmentB++;
            buy = true;
        }
        else if(index == 2 && civ.milPower >= devCost)
        {
            civ.milPower -= devCost;
            developmentC++;
            buy = true;
        }
        if (buy)
        {
            SetMaxControl();
            control = Mathf.Min(maxControl, control + 1);
            avaliablePopulation = Mathf.Min(avaliableMaxPopulation, avaliablePopulation + (int)(population * 0.01f), population);
            SetDevCost();
            civ.RefreshForceLimit();
        }
    }
    public void ApplyZOC()
    {
        hasZOC = true;
        var neighbors = GetNeighbors();
        foreach(var n in neighbors)
        {
            TileData data = Map.main.GetTile(n);
            data.hasZOC = true;           
        }
    }
    public bool HasNeighboringActiveFort(int civIDAffected)
    {
        if (civIDAffected == -1)
        {
            var neighbors = GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData data = Map.main.GetTile(n);
                if (data.hasFort && data.civID == civID)
                {
                    if (!data.occupied)
                    { return true; }
                }
            }
        }
        else
        {
            Civilisation civ = Game.main.civs[civIDAffected];
            var neighbors = GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData data = Map.main.GetTile(n);
                if (civ.atWarWith.Contains(data.civID) && data.hasFort)
                {
                    if (!data.occupied)
                    { return true; }
                }
                else if (data.occupied && civ.atWarWith.Contains(data.occupiedByID) && data.hasFort)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool HasNeighboringActiveOccupiedFort(int civIDAffected)
    {
        if (civIDAffected == -1)
        {
            var neighbors = GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData data = Map.main.GetTile(n);
                if (data.hasFort && data.civID == civID)
                {
                    if (data.occupied && data.occupiedByID != civIDAffected)
                    { 
                        return true;
                    }
                }
            }
        }
        else
        {
            Civilisation civ = Game.main.civs[civIDAffected];
            var neighbors = GetNeighbors();
            foreach (var n in neighbors)
            {
                TileData data = Map.main.GetTile(n);
                if (data.occupied && civ.atWarWith.Contains(data.occupiedByID) && data.hasFort)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public int GetDevCost(int FromCiv = -1)
    {
        if(civID == -1) { return 999; }
        Civilisation owner = civ;
        if(FromCiv > -1)
        {
            owner = Game.main.civs[FromCiv];
        }
        float val = 50 * (1f + localDevCostMod.value + owner.devCostMod.value) * Mathf.Max(0.1f,1f + localDevCost.value + owner.devCost.value);
        return Mathf.Max(0, (int)val);
    }
    public Vector3 worldPos()
    {
        return Map.main.tileMapManager.tilemap.CellToWorld(pos);
    }

    public Vector3Int cubePos => evenr_to_cube(pos);
    public List<Vector3Int> GetNeighbors()
    {
        Vector3Int cubePos = evenr_to_cube(pos);
        return GetCubeNeighbors(cubePos).ConvertAll(i=> cube_to_evenr(i));
    }
    public List<Vector3Int> GetRadius(int radius = 1)
    {
        Vector3Int cubePos = evenr_to_cube(pos);
        return GetCubeRadius(cubePos,radius).ConvertAll(i => cube_to_evenr(i));
    }

    public List<Vector3Int> GetRing(int radius = 1)
    {
        Vector3Int cubePos = evenr_to_cube(pos);
        return GetCubeRing(cubePos, radius).ConvertAll(i => cube_to_evenr(i));
    }

    public static Vector3Int[] cube_direction_vectors = new Vector3Int[] {
    new Vector3Int(1, 0,-1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
    new Vector3Int(-1, 0,1),new Vector3Int (-1, 1,0),new Vector3Int (0, 1,-1)
        };
    public static Vector3Int cube_neighbour(Vector3Int cubePos , int direction)
    {
        return cubePos + cube_direction_vectors[direction];
    }
    public static List<Vector3Int> GetCubeNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        Vector3Int[] cube_direction_vectors = new Vector3Int[] {
    new Vector3Int(1, 0,-1), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 1),
    new Vector3Int(-1, 0,1),new Vector3Int (-1, 1,0),new Vector3Int (0, 1,-1)
        };
        foreach (Vector3Int dir in cube_direction_vectors)
        {
            neighbors.Add(pos + dir);
        }
        return neighbors;
    }
    public static List<Vector3Int> GetCubeRing(Vector3Int pos, int radius = 1)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        Vector3Int startPos = pos + cube_direction_vectors[4] * radius;
        for(int i = 0; i < 6; i++)
        {
            for(int j = 0; j < radius; j++)
            {
                results.Add(startPos);
                startPos = cube_neighbour(startPos, i);
            }
        }
        return results;
    }
    public static List<Vector3Int> GetCubeRadius(Vector3Int pos , int radius = 1)
    {
        List<Vector3Int> results = new List<Vector3Int>();
        for (int i = -radius; i < radius + 1; i++)
        {
            for (int j = Mathf.Max(-radius, -i -radius); j < Mathf.Min(radius, radius - i) + 1;j++)
            {   
                if(i == 0 && i == j) { continue; }
                int k = -i - j;
                results.Add(pos + new Vector3Int(i,j,k));
            }
        }
        return results;
    }
    public static Vector3Int evenr_to_cube(Vector3Int pos)
    {
        int parity = pos.y & 1;
        int q = pos.x - (pos.y - parity) / 2;
        int r = pos.y;

        return new Vector3Int(q, r, -q-r);
    }

    public static Vector3Int cube_to_evenr(Vector3Int pos)
    {
        int parity = pos.y & 1;
        int q = pos.x + (pos.y - parity) / 2;
        int r = pos.y;

        return new Vector3Int(q, r, 0);
    }
    public static int cube_distance(Vector3Int ax1, Vector3Int ax2) 
    {
        return (Mathf.Abs(ax1.x - ax2.x) + Mathf.Abs(ax1.y - ax2.y) + Mathf.Abs(ax1.z - ax2.z)) / 2;
    }

    public static int evenr_distance(Vector3Int ax1, Vector3Int ax2)
    {
        ax1 = evenr_to_cube(ax1);
        ax2 = evenr_to_cube(ax2);
        return (Mathf.Abs(ax1.x - ax2.x) + Mathf.Abs(ax1.y - ax2.y) + Mathf.Abs(ax1.z - ax2.z)) / 2;
    }
    public Stat GetStat(string name)
    {
        switch (name)
        {
            case "Fort Defence":
                return localDefensiveness;
            case "Construction Cost":
                return localConstructionCost;
            case "Construction Time":
                return localConstructionTime;
            case "Force Limit":
                return localForceLimit;
            case "Population Growth":
                return localPopulationGrowth;
            case "Maximum Population":
                return localMaxPopulation;
            case "Development Cost":
                return localDevCost;
            case "Tax Efficiency":
                return localTaxEfficiency;
            case "Daily Control":
                return dailyControl;
            case "Production Value":
                return localProductionValue;
            case "Production Amount":
                return localProductionQuantity;
            case "Movement Speed":
                return localMovementSpeed;
            case "Recruitment Cost":
                return localRecruitmentCost;
            case "Recruitment Time":
                return localRecruitmentTime;
            case "Attacker Dice Roll":
                return localAttackerDiceRoll;
            default:
                return null;
        }
    }
    public void ApplyTileLocalModifier(string modifierName, float strength, int type = 1,string source = "",int time = -1)
    {
        Stat stat = GetStat(modifierName);
        if (stat != null) 
        { 
            stat.AddModifier(new Modifier(strength, type, source, time));
        }
    }
}
