using System.Collections.Generic;
using TMPro;

using UnityEngine;

public class TileData
{
    public string Name;
    public Vector3Int pos;
    public Vector3Int portTile;
    List<TileData> NeighborTiles;
    List<Vector3Int> NeighborPos;
    public int civID;
    public bool isCoastal;
    public Color currentCol = Color.white;
    public Color selectorCol = Color.white;
    public bool hasMarket = false;
    public int marketLevel = 0;
    public int infrastructureLevel = 0;
    public int status = 0;
    public Civilisation civ => Game.main.civs[civID];
    public bool underSiege => siege != null && siege.inProgress;
    public int fortLevel;
    public bool hasFort;
    public bool hasZOC;
    public Siege siege;
    public bool occupied;
    public int occupiedByID;
    public ResourceType tileResource, tileSecondaryResource = null;
    public Terrain terrain;
    public List<Army> armiesOnTile = new List<Army>();
    public List<Fleet> fleetsOnTile = new List<Fleet>();
    public SpriteRenderer selectedTileObj,settlementSprite;
    public Battle _battle = null;
    public NavalBattle _navalBattle = null;
    public int recruitTimer = -1;
    public int boatTimer = -1;
    public int mercenaryTimer = -1;
    public List<int> recruitQueue = new List<int>();
    public List<int> boatQueue = new List<int>();
    public List<int> mercenaryQueue = new List<int>();
    public int buildTimer = -1;
    public List<int> buildQueue = new List<int>();
    public string region;
    public string tradeRegion;
    public int tradeRegionID;
    public string continent;
    public int religionTimer = -1;
    public int religion;
    public int population;
    public int avaliablePopulation;
    public List<int> buildings = new List<int>();
    public List<int> cores = new List<int>();
    public int coreTimer = -1;
    public GameObject fort;
    public GreatProject greatProject = null;
    public TextMeshProUGUI tileText = null;
    public bool hasCore => cores.Contains(civID);
    public int avaliableMaxPopulation => Mathf.Max(0, (int)(maxPopulation * control / 100f));
    public int populationGrowth => Mathf.Max(0, (int)((1 + developmentC * 6) * (1f + localPopulationGrowth.value) * (civID > -1 ? 1f + civ.populationGrowth.value : 1f)));
    public int maxPopulation => Mathf.Max(0, (int)((totalDev + developmentA * 0.5) * 200 * (1f + localMaxPopulation.value) * (civID > -1 ? 1f + civ.maximumPopulation.value : 1f)));
    public int developmentA, developmentB, developmentC;
    public float control;
    public float maxControl;
    public Stat localDevCostMod = new Stat(0f, "Local Development Cost Modifier");
    public Stat localDevCost = new Stat(0f, "Local Development Cost");
    public Stat localProductionValue = new Stat(0f, "Local Production Value");
    public Stat localProductionQuantity = new Stat(0f, "Local Production Quantity");
    public Stat localTaxEfficiency = new Stat(0f, "Local Tax Efficiency");
    public Stat localGoverningCost = new Stat(0f, "Local Governing Cost", true);
    public Stat localGoverningCostMod = new Stat(0f, "Local Governing Cost Modifier", false);
    public Stat localAttritionForEnemies = new Stat(0f, "Local Attrition for Enemies");
    public Stat localMinimumControl = new Stat(0f, "Local Minimum Control");
    public Stat localConstructionCost = new Stat(0f, "Local Construction Cost");
    public Stat localConstructionTime = new Stat(0f, "Local Construction Time");
    public Stat localMovementSpeed = new Stat(0f, "Local Movement Speed");
    public Stat localRecruitmentCost = new Stat(0f, "Local Recruitment Cost");
    public Stat localRecruitmentTime = new Stat(0f, "Local Recruitment Time");
    public Stat dailyControl = new Stat(0.01f, "Daily Control", true);
    public Stat localDefensiveness = new Stat(0f, "Local Defensiveness");
    public Stat localFortMaintenance = new Stat(0f, "Local Fort Maintenance");
    public Stat localPopulationGrowth = new Stat(0f, "Local Population Growth");
    public Stat localMaxPopulation = new Stat(0f, "Local Max Population");
    public Stat localForceLimit = new Stat(0f, "Local Force Limit");
    public Stat localAttackerDiceRoll = new Stat(0f, "Local Attacker Dice Roll", true);
    public Stat localUnrest = new Stat(0f, "Local Unrest", true);
    public int seperatism = 0;
    public int rebelHeldTime = 0;
    public int heldByID = -1;
    public int heldByType = -1;
    public float unrest => localUnrest.value + (civID > -1 ? civ.globalUnrest.value : 0f);
    public int totalDev => developmentA + developmentB + developmentC;
    public string tileTypeName => Map.main.GetTileName(pos);

    public int supplyLimit => 6 + (terrain != null ? terrain.supplyLimitBonus : 0);

    public float GetGoverningCost()
    {
        float cost = totalDev + localGoverningCost.value;
        cost = Mathf.Max(0, cost * (1f + (status == 0 ? -0.5f : status == 1 ? 0f : status == 2 ? 1f : 2f)) - (civ.capitalPos == pos ? 1f:0));
        return cost * Mathf.Max(0, (1f + localGoverningCostMod.value + civ.governingCostModifier.value));
    }
    public void BreakToRebels()
    {
        if (heldByType > -1)
        {
            if (heldByType == 0)
            {
                control -= 50;
                localUnrest.AddModifier(new Modifier(-2, 1, "Rebel Demands", 720));
                occupied = false;
            }
            else if (heldByType == 1)
            {
                int oldCiv = civID;
                int newCiv = heldByID;
                civID = newCiv;
                Game.main.civs[oldCiv].updateBorders = true;
                if (pos == Game.main.civs[oldCiv].capitalPos)
                {
                    Game.main.civs[oldCiv].NewCapital(new List<Vector3Int>() { pos });
                }
                Game.main.civs[newCiv].updateBorders = true;
                if (!Game.main.civs[newCiv].isActive())
                {
                    Game.main.civs[newCiv].Rebirth();
                }
                SetMaxControl();
                if (cores.Contains(newCiv))
                {
                    control = 100;
                    localUnrest.AddModifier(new Modifier(-100, 1, "Rebel Demands", 720));
                }
                else
                {
                    control = 25;
                }
                occupied = false;
            }
            else if (heldByType == 2)
            {
                control -= 50;
                religion = heldByID;
                occupied = false;
            }
        }
    }
    public TileData(Vector3Int Pos, int CivID)
    {
        pos = Pos;
        civID = CivID;
        religion = -1;
        currentCol = Color.white;
        greatProject = null;
    }
    public bool needsCoring()
    {
        if (hasCore || coreTimer != -1) { return false; }
        return true;
    }
    public bool needsConverting()
    {
        if (civID == -1) { return false; }
        if (religion == civ.religion || religionTimer != -1)
        {
            return false;
        }
        return true;
    }
    public void StartBuilding(int id, int fromCiv = -1)
    {
        Building building = Map.main.Buildings[id];
        Civilisation FromCiv = fromCiv == -1 ? civ : Game.main.civs[fromCiv];
        float cost = building.GetCost(this, civ);
        if (FromCiv.coins >= cost)
        {
            FromCiv.coins -= cost;
        }
        else { return; }
        if (buildQueue.Count == 0)
        {
            buildTimer = (int)building.GetTime(this, civ);
        }
        buildQueue.Add(id);
        if (fromCiv == civ.overlordID && civ.overlordID > -1)
        {
            civ.libertyDesireTemp.IncreaseModifier("Built Building", -10f, 1, Decay: true);
        }

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
    public void StartRecruitingBoat(int type)
    {
        float cost = GetRecruitCost(type);
        if (civ.coins >= cost)
        {
            civ.coins -= cost;
        }
        else { return; }
        if (boatQueue.Count == 0)
        {
            boatTimer = GetRecruitTime();
        }
        boatQueue.Add(type);
    }
    public void StartRecruitingMercenary(int mercID)
    {
        MercenaryGroup merc = Map.main.mercenaries[mercID];
        float cost = GetMercRecruitCost(mercID);
        if (civ.coins >= cost)
        {
            civ.coins -= cost;
        }
        else { return; }
        if (mercenaryQueue.Count == 0)
        {
            mercenaryTimer = GetRecruitTime();
        }
        mercenaryQueue.Add(mercID);
        civ.mercTimers[mercID] = 6;
    }
    public int GetRecruitTime()
    {
        return (int)Mathf.Max(144 * (1f + localRecruitmentTime.value + civ.recruitmentTime.value), 1);
    }
    public void Build(int id)
    {
        Building building = Map.main.Buildings[id];
        if (!buildings.Contains(id))
        {
            buildings.Add(id);
            if (building.effects.name.Length > 0)
            {
                ApplyTileLocalModifier(building.effects.name, building.effects.amount, building.effects.type, building.Name);
            }
            if (building.fortLevel > 0)
            {
                fortLevel += building.fortLevel;
                if (!hasFort)
                {
                    fort = GameObject.Instantiate(Map.main.fortPrefab, worldPos(), Quaternion.identity, Map.main.fortTransform);
                    hasFort = true;
                    ApplyZOC();
                }
            }
        }
    }
    public void RemoveBuilding(int id)
    {
        if (!buildings.Contains(id))
        {
            return;
        }
        Building building = Map.main.Buildings[id];
        buildings.Remove(id);
        if (building.fortLevel > 0)
        {
            fortLevel -= building.fortLevel;
            hasFort = false;
            if (fort != null)
            {
                GameObject.Destroy(fort);
            }
        }
        if (building.effects.name.Length > 0)
        {
            GetStat(building.effects.name).TryRemoveModifier(building.Name);
        }
    }
    public void StartConvert()
    {
        if (civID == -1) { return; }
        if (religion == civ.religion || religionTimer != -1) { return; }
        else if (control >= GetConvertControl() && civ.diploPower >= GetConvertCost())
        {
            religionTimer = GetConvertTime();
            civ.diploPower -= GetConvertCost();
        }
    }
    public float GetConvertControl()
    {
        float baseControl = 90f * (1f + civ.conversionCost.value);
        return Mathf.Max(baseControl, 0);
    }
    public int GetConvertTime()
    {
        int baseTime = (int)(totalDev * 12 * (1f + civ.conversionCost.value));
        return Mathf.Max(baseTime, 1);
    }
    public int GetConvertCost()
    {
        int baseCost = (int)(totalDev * 5 * (1f + civ.conversionCost.value));
        return Mathf.Max(baseCost, 1);
    }
    public void StartCore()
    {
        if (civID == -1) { return; }
        if (hasCore || coreTimer != -1) { return; }
        if (civ.adminPower >= GetCoreCost())
        {
            coreTimer = GetCoreTime();
            civ.adminPower -= GetCoreCost();
        }
    }
    public void TransferOccupation(int civTo, bool integrated = false)
    {
        civID = civTo;
        if (integrated)
        {
            if (!cores.Contains(civTo))
            {
                cores.Add(civTo);
            }            
        }
        if (status > 0 && civID > -1)
        {
            civ.controlCentres.Remove(pos);
            status = 0;
            UpdateStatusModifiers();
        }
        SetMaxControl();
        buildQueue.Clear();
        recruitQueue.Clear();
        mercenaryQueue.Clear();
        boatQueue.Clear();
        coreTimer = -1;
        religionTimer = -1;
        buildTimer = -1;
        recruitTimer = -1;
        mercenaryTimer = -1;
        boatTimer = -1;
        control = Mathf.Min(75f, control, maxControl);
    }
    public int GetCoreTime()
    {
        int baseTime = 90;
        baseTime = (int)(baseTime * (1f + civ.coreCost.value + (civ.claims.Contains(pos) ? -0.25f : 0f)));
        return Mathf.Max(baseTime,1);
    }
    public int GetCoreCost()
    {
        int baseCost = totalDev * 10;
        baseCost = (int)(baseCost * (1f + civ.coreCost.value + (civ.claims.Contains(pos) ? -0.25f : 0f)));
        return Mathf.Max(baseCost,1);
    }
    public bool CanPromoteStatus()
    {
        if (civID == -1) { return false; }
        if(civ.maxSettlements.value <= civ.controlCentres.Count && status == 0) { return false; }
        if (status >= 3) { return false; }
        if(totalDev < status * 10f + 5f) { return false; }
        return civ.adminPower >= PromoteStatusCost();
    }
    public int PromoteStatusCost()
    {
        return (int)Mathf.Round((status * 50 + 50) * (1f + civ.promoteSettlementCost.value));
    }
    public void UpdateInfrastructureModifiers()
    {
        localTaxEfficiency.UpdateModifier("Infrastructure", infrastructureLevel * 0.1f, 1);
        localDevCost.UpdateModifier("Infrastructure", infrastructureLevel * -0.15f, 1);
        localProductionQuantity.UpdateModifier("Infrastructure", infrastructureLevel * 0.05f, 1);
        localProductionValue.UpdateModifier("Infrastructure", infrastructureLevel * 0.05f, 1);
        localPopulationGrowth.UpdateModifier("Infrastructure", infrastructureLevel * 0.05f, 1);
        localMaxPopulation.UpdateModifier("Infrastructure", infrastructureLevel * 0.1f, 1);
        localConstructionCost.UpdateModifier("Infrastructure", infrastructureLevel * -0.05f, 1);
        localConstructionTime.UpdateModifier("Infrastructure", infrastructureLevel * -0.05f, 1);
        localDefensiveness.UpdateModifier("Infrastructure", infrastructureLevel * 0.05f, 1);
        localRecruitmentTime.UpdateModifier("Infrastructure", infrastructureLevel * -0.15f, 1);
        localGoverningCost.UpdateModifier("Infrastructure", infrastructureLevel * 5f, 1);
        localGoverningCostMod.UpdateModifier("Infrastructure", infrastructureLevel * 0.1f, 1);
    }
    public void UpdateStatusModifiers()
    {
        if (status > 1)
        {
            int level = status - 1;
            localDevCostMod.UpdateModifier("Status", -0.05f * level, 1);
            localConstructionTime.UpdateModifier("Status", -0.1f * level, 1);
            localConstructionCost.UpdateModifier("Status", -0.05f * level, 1);
            localUnrest.UpdateModifier("Status", -1f * level, 1);
        }
        settlementSprite.sprite = Map.main.statusSprites[status];
    }
    public void PromoteStatus()
    {
        if (!CanPromoteStatus())
        {
            return;
        }
        civ.adminPower -= PromoteStatusCost();
        status += 1;
        if (!civ.controlCentres.ContainsKey(pos))
        {
            civ.controlCentres.Add(pos, status);
        }
        else
        {
            civ.controlCentres[pos] = status;
        }
        SetMaxControl();
        control = Mathf.Clamp(control + 25, 0, maxControl);
        UpdateStatusModifiers();
    }
    public void SetMaxControl()
    {
        float maximumControl = 10f;
        foreach (var contolCentre in civ.controlCentres)
        {
            float controlDecay = (contolCentre.Value == 0 ? 100f : contolCentre.Value == 1 ? 50f : contolCentre.Value == 2 ? 25f : 10f) * (1f + civ.controlDecay.value);
            float possibleMaximumControl = 100f - controlDecay * evenr_distance(pos,contolCentre.Key) + totalDev;
            maximumControl = Mathf.Max(possibleMaximumControl, maximumControl);
        }
        float minimumControl = pos == civ.capitalPos ? 100f : localMinimumControl.value + civ.minControl.value;
        maxControl = Mathf.Clamp(maximumControl, minimumControl, hasCore ? 100f : 25f);
        control = Mathf.Max(minimumControl, Mathf.Min(control, maxControl));
    }
    public float GetDevProdIncrease()
    {
        if (civID == -1) { return 0; }
        float value = tileResource.Value * (1f + localProductionValue.value) * (1f + civ.productionValue.value) * (1f + localProductionQuantity.value) * (1f + civ.productionAmount.value);
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
        float value = 0.025f * (1f+ localTaxEfficiency.value )*(1f + civ.taxEfficiency.value);
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
    public float GetMercRecruitCost(int mercID)
    {
        if (civID == -1) { return 9999f; }
        MercenaryGroup merc = Map.main.mercenaries[mercID];
        float baseCost = merc.costPerRegiment * (merc.baseRegiments + merc.regimentsPerYearExtra * Game.main.gameTime.years) * (7 + Game.main.gameTime.months + Game.main.gameTime.years * 12);
        baseCost *= (1f + localRecruitmentCost.value + civ.regimentCost.value);
        return Mathf.Max(baseCost, 1f);
    }
    public void CreateNewArmy(int type)
    {
        if (civID == -1) { return; }
        if (civ.avaliablePopulation >= 1000)
        {
            int size = civ.RemovePopulation(1000);
            if (size >= 500)
            {
                List<Regiment> regiments = new List<Regiment> { new Regiment(Size: size, CivID: civID,Type: type) };
                Army.NewArmy(this, civID, regiments);
            }
            
        }
    }
    public void CreateNewBoat(int type)
    {
        if (civID == -1) { return; }
        if (civ.avaliablePopulation >= 200)
        {
            int size = civ.RemovePopulation(200);
            if (size >= 100)
            {
                List<Boat> boats = new List<Boat> { new Boat( CivID: civID, Type: type) };
                Fleet.NewFleet(this, civID, boats);
            }

        }
    }
    public void CreateMercenaryGroup(int mercID)
    {
        MercenaryGroup merc = Map.main.mercenaries[mercID];
        if (civID == -1) { return; }
        List<Regiment> regiments = new List<Regiment>();
        int regimentCount = merc.baseRegiments + merc.regimentsPerYearExtra * Game.main.gameTime.years;
        int cavCount = (int)(regimentCount * merc.cavalryPercent);
        for(int i = 0; i < regimentCount; i++)
        {
            regiments.Add(new Regiment(civID, 1000, 1000, i >= cavCount ? 0 : 1,true));
        }
        Army.NewArmy(this, civID, regiments,true);
    }
    public float GetDailyProductionAmount()
    {
        if (civID == -1) { return 0; }
        if (tileResource == null) { return 0f; }
        float amount = (1f + localProductionQuantity.value) *(1f + civ.productionAmount.value) * Mathf.Clamp(avaliablePopulation, 0f, 200f * developmentB) / 10000f;
        if (!hasCore) { amount *= 0.25f; }
        return amount;
    }
    public float GetDailyProductionValue()
    {
        if (civID == -1) { return 0; }
        if (tileResource == null) { return 0f; }
        float value = tileResource.Value * (1f + localProductionValue.value )*(1f + civ.productionValue.value) * GetDailyProductionAmount();
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
    public bool canDev(int index)
    {
        if (index == 0)
        {
           if(developmentA > developmentB + developmentC) { return false; }
        }
        else if (index == 1)
        {
            if (developmentB > developmentA + developmentC) { return false; }
        }
        else if (index == 2)
        {
            if (developmentC > developmentB + developmentA) { return false; }
        }
        return true;
    }
    public void AddDevelopment(int index,int fromCiv = -1)
    {
        if (civID == -1) { return; }
        if (!hasCore) { return; }
        if(!canDev(index)) { return; }
        Civilisation FromCiv = fromCiv == -1 ? civ : Game.main.civs[fromCiv];
        int devCost = GetDevCost(fromCiv);
        bool buy = false;
        if (index == 0 && FromCiv.adminPower >= devCost)
        {
            FromCiv.adminPower -= devCost;
            developmentA++;
            buy = true;
        }
        else if(index == 1 && FromCiv.diploPower >= devCost)
        {
            FromCiv.diploPower -= devCost;
            developmentB++;
            buy = true;
        }
        else if(index == 2 && FromCiv.milPower >= devCost)
        {
            FromCiv.milPower -= devCost;
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
            if(fromCiv == civ.overlordID && civ.overlordID > -1)
            {
                civ.libertyDesireTemp.IncreaseModifier("Developed Our Land", -5f, 1, Decay: true);
            }
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

    public void SetNeighbors()
    {
        NeighborPos = getNeighbors();
        NeighborTiles = new List<TileData>();
        isCoastal = false;
        foreach (var n in NeighborPos)
        {
            TileData tile = Map.main.GetTile(n);
            if (tile != null)
            {
                NeighborTiles.Add(tile);         
                if(tile.terrain != null && terrain != null)
                {
                    if (tile.terrain.isSea && !terrain.isSea)
                    {
                        if (!isCoastal)
                        {
                            portTile = n;
                            
                        }
                        isCoastal = true;
                        Vector3 worldpos  = worldPos();
                        Vector3 portPos = Map.main.GetTile(portTile).worldPos();
                        Vector3 pos = (worldpos + portPos)/2;
                        Vector3 dir = portPos - worldpos;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90f));
                        GameObject.Instantiate(Map.main.settlementPrefab, pos, rotation, Map.main.buildingTransform).GetComponent<SpriteRenderer>().sprite = Map.main.portSprite;

                    }
                }
            }
        }
    }
    public List<Vector3Int> GetNeighbors()
    {
        return NeighborPos;
    }
    public List<TileData> GetNeighborTiles()
    {
        return NeighborTiles;
    }
    public Vector3Int cubePos => evenr_to_cube(pos);
    private List<Vector3Int> getNeighbors()
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
        return Add(cubePos, cube_direction_vectors[direction]);
    }
    public static List<Vector3Int> GetCubeNeighbors(Vector3Int pos)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();
        foreach (Vector3Int dir in cube_direction_vectors)
        {
            neighbors.Add(Add(pos, dir));
        }
        return neighbors;
    }
    public static Vector3Int Add(Vector3Int one,Vector3Int two)
    {       
        one.x += two.x;
        one.y += two.y;
        one.z += two.z;
        return one;
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
                results.Add(Add(pos, new Vector3Int(i,j,k)));
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
            case "Fort Maintenance":
                return localFortMaintenance;
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
            case "Minimum Control":
                return localMinimumControl;
            case "Attrition for Enemies":
                return localAttritionForEnemies;
            case "Governing Cost":
                return localGoverningCost;
            case "Governing Cost Modifier":
                return localGoverningCostMod;
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
    public void RemoveTileLocalModifier(string statname, string modname)
    {
        Stat stat = GetStat(statname);
        if (stat != null)
        {
            if (stat.modifiers.Exists(item => item.name == modname))
            {
                stat.RemoveModifier(stat.modifiers.Find(item => item.name == modname));
            }
        }
    }
}
