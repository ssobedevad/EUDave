using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Civilisation
{
    public float AIAggressiveness,AIAdministrative,AIDiplomatic,AIMilitary;
    public Color c;
    public string civName;
    public Vector3Int capitalPos;
    public Dictionary<Vector3Int,int> controlCentres = new Dictionary<Vector3Int, int>();
    public int CivID; 
    public bool isPlayer;
    public GameObject capitalIndicator;
    public List<Vector3Int> visited = new List<Vector3Int>();
    public List<Vector3Int> explorePos = new List<Vector3Int>();
    public List<LineRenderer> borders = new List<LineRenderer>();
    public List<TextMeshProUGUI> countryNames = new List<TextMeshProUGUI>();
    public List<Army> armies = new List<Army>();
    public List<Fleet> fleets = new List<Fleet>();
    public List<int> atWarWith = new List<int>();
    public List<int> atWarTogether = new List<int>();
    public List<int> militaryAccess = new List<int>();
    public List<int> allies = new List<int>();
    public List<int> subjects = new List<int>();
    public List<int> eventHistory = new List<int>();
    public int[] rivals = new int[3] {-1,-1,-1};
    public int[] mercTimers;
    public List<General> generals = new List<General>();

    public int overlordID = -1;
    public int subjectType = -1;
    public float libertyDesire = 0f;

    public float religiousPoints;
    public float religiousUnity = 1f;
    public int[] religiousDevelopment;
    public List<Vector3Int> deployedMissionaries = new List<Vector3Int>();
    public int avaliableMissionaries => (int)maximumMissionaries.v - deployedMissionaries.Count;


    public Stat libertyDesireTemp = new Stat(0f, "Liberty Desire Temp");
    public int annexationProgress = 0;
    public bool integrating = false;
    public int focus = -1;
    public int focusCD = 0;

    public float reformProgress;
    public float governingCapacity;
    public float diplomaticCapacity;
    public List<DiplomatStatus> deployedDiplomats = new List<DiplomatStatus>();
    public int avaliableDiplomats => (int)maximumDiplomats.v - deployedDiplomats.Count;

    public float coins;
    public float prestige;
    public float governmentPower;
    public int stability;
    public int adminPower, diploPower, milPower;

    public Ruler ruler;
    public Ruler heir;

    public Advisor advisorA,advisorD,advisorM;
    public List<Advisor> advisorsA = new List<Advisor>();
    public List<Advisor> advisorsD = new List<Advisor>();
    public List<Advisor> advisorsM = new List<Advisor>();

    public int adminTech,diploTech,milTech;
    public int religion;
    public int government;
    public int governmentRank;

    public List<int> unlockedBuildings = new List<int>();
    public NationalIdeas nationalIdeas;
    public IdeaGroupData[] ideaGroups = new IdeaGroupData[8];
    public int totalIdeas = 0;
    public int unlockedIdeaGroupSlots = 0;
    public List<Loan> loans = new List<Loan>();
    public float armyTradition;
    public float overextension;
    public float maxAE;
    public float revanchism;
    public List<string> techUnlocks = new List<string>();
    public List<int> reforms = new List<int>();

    public List<UnitType> units = new List<UnitType>();
    public List<BoatType> boats = new List<BoatType>() { new BoatType("Transport", 0f, 15f, 10f, 2, 20f,100f,2), new BoatType("Supply Ship", 0f, 25f, 200f, 2, 50f,200f,3), new BoatType("Warship", 3f, 100f, 100f, 2, 200f,400f,5) };
        
    public Dictionary<string,Stat> stats = new Dictionary<string,Stat>();
    public Stat maxSettlements;
    public Stat promoteSettlementCost;
    public Stat governingCapacityMax;
    public Stat governingCostModifier;
    public Stat diplomaticCapacityMax;
    public Stat attritionForEnemies;
    public Stat landAttrition;
    public Stat attackerDiceRoll;
    public Stat defenderDiceRoll;
    public Stat generalCombatSkill;
    public Stat generalSiegeSkill;
    public Stat generalManeuverSkill;
    public Stat rulerAdminSkill;
    public Stat rulerDiploSkill;
    public Stat rulerMilSkill;
    public Stat reformProgressGrowth;
    public Stat militaryTactics;
    public Stat discipline;
    public Stat moraleMax;
    public Stat moraleRecovery;
    public Stat reinforceSpeed;
    public Stat combatWidth;
    public Stat devCostMod;
    public Stat devCost;
    public Stat fortDefence;
    public Stat fortMaintenance;
    public Stat siegeAbility;
    public Stat constructionCost;
    public Stat constructionTime;
    public Stat populationGrowth;
    public Stat maximumPopulation;
    public Stat taxEfficiency;
    public Stat taxIncome;
    public Stat productionValue;
    public Stat productionAmount;
    public Stat dailyControl;
    public Stat controlDecay;
    public Stat regimentMaintenanceCost;
    public Stat regimentCost;
    public Stat recruitmentTime;
    public Stat movementSpeed;
    public Stat forceLimit;
    public Stat prestigeDecay;
    public Stat monthlyPrestige;
    public Stat armyTraditionDecay;
    public Stat monthlyTradition;
    public Stat stabilityCost;
    public Stat coreCost;
    public Stat missionaryStrength;
    public Stat maximumAdvisors;
    public Stat advisorCosts;
    public Stat advisorCostsA;
    public Stat advisorCostsD;
    public Stat advisorCostsM;
    public Stat techCosts;
    public Stat techCostsA;
    public Stat techCostsD;
    public Stat techCostsM;
    public Stat ideaCosts;
    public Stat globalUnrest;
    public Stat trueFaithTolerance;
    public Stat infidelIntolerance;
    public Stat warScoreCost;
    public Stat dipAnnexCost;
    public Stat libDesireFromDevForSubjects;
    public Stat battlePrestige;
    public Stat battleTraditon;
    public Stat diploRep;
    public Stat libDesireInSubjects;
    public Stat incomeFromSubjects;
    public Stat improveRelations;
    public Stat aggressiveExpansionImpact;
    public Stat monthsOfSeperatism;
    public Stat coringRange;
    public Stat interestPerMonth;
    public Stat minControl;
    public Stat tradeValue;
    public Stat tradePenalty;
    public Stat tradeValPerCiv;
    public Stat maximumDiplomats;
    public Stat maximumMissionaries;


    public Stat[] opinionOfThem;

    public bool canHolyWar;
    public bool isMarshLeader;
    public List<Vector3Int> civTiles = new List<Vector3Int>();
    public List<Vector3Int> claims = new List<Vector3Int>();
    public List<Vector3Int> cores = new List<Vector3Int>();
    public bool[] tradeRegions = new bool[0];
    public List<TileData> civTileDatas = new List<TileData>();
    public List<TileData> civCoastalTiles = new List<TileData>();
    public List<int> civNeighbours = new List<int>();
    public int[] truces;
    public float[] spyNetwork;
    public Mission[] missions;
    public bool[] missionProgress;
    public bool hasUpdatedStartingResources = false;
    public bool updateBorders;
    public List<WeightedChoice> events = new List<WeightedChoice>();
    public int avaliablePopulation;
    public float startingEcon;
    public int startTiles;

    public int GetIntegrationCost(Civilisation overlord)
    {
        SubjectType type = subjectType > -1 ? Map.main.subjectTypes[subjectType] : Map.main.subjectTypes[0];
        int baseCost = 0;
        foreach(var tile in GetAllCivTileDatas())
        {
            if(tile.civID != CivID) { continue;}
            if (!tile.cores.Contains(overlordID))
            {
                baseCost+= 8 * tile.totalDev;
            }
        }
        return (int)(baseCost * (1f + overlord.dipAnnexCost.v) * type.DiploAnnexCostModifier);
    }
    public void UpdateDiplomaticCapacity()
    {
        diplomaticCapacity = 0f;
        foreach(var allyID in allies)
        {
            Civilisation ally = Game.main.civs[allyID];
            diplomaticCapacity += 25 + ally.governingCapacity * 0.5f;
        }
        foreach (var allyID in subjects)
        {
            Civilisation ally = Game.main.civs[allyID];
            SubjectType type = ally.subjectType > -1 ? Map.main.subjectTypes[ally.subjectType] : Map.main.subjectTypes[0];
            diplomaticCapacity += type.DiplomaticCapacityFlat + ally.governingCapacity * type.DiplomaticCapacityFromGoverningCapacity;
        }
        foreach (var allyID in militaryAccess)
        {
            Civilisation ally = Game.main.civs[allyID];
            diplomaticCapacity += ally.governingCapacity * 0.25f;
        }
        libDesireInSubjects.UpdateModifier("Over Diplomatic Capacity", diplomaticCapacity > diplomaticCapacityMax.v ? (diplomaticCapacity - diplomaticCapacityMax.v)/ diplomaticCapacityMax.v * 0.5f : 0, EffectType.Flat);
        diploRep.UpdateModifier("Over Diplomatic Capacity", diplomaticCapacity > diplomaticCapacityMax.v ? (diplomaticCapacity - diplomaticCapacityMax.v) / diplomaticCapacityMax.v * -2f : 0, EffectType.Flat);
        advisorCosts.UpdateModifier("Over Diplomatic Capacity", diplomaticCapacity > diplomaticCapacityMax.v ? (diplomaticCapacity - diplomaticCapacityMax.v) / diplomaticCapacityMax.v * 1f : 0, EffectType.Flat);
    }
    public List<MercenaryGroup> GetPossibleMercs()
    {
        List<MercenaryGroup> possiblemercs = Map.main.mercenaries.ToList();
        for (int i = 0; i < Map.main.mercenaries.Length; i++)
        {
            if (mercTimers[i] > 0)
            {
                possiblemercs.Remove(Map.main.mercenaries[i]);
            }
        }
        possiblemercs.RemoveAll(i => (i.requiredReligion != religion && i.requiredReligion > -1) || (i.requiredGovernmentType != government && i.requiredGovernmentType > -1));
        return possiblemercs;
    }
    public void SetLibertyDesire()
    {
        if (overlordID == -1) { return; }
        Civilisation overlord = Game.main.civs[overlordID];
        SubjectType type = subjectType > -1 ? Map.main.subjectTypes[subjectType] : Map.main.subjectTypes[0];
        float ld = 0;
        if (type.CountsOwnArmies)
        {
            ld += MathF.Min(100f, (TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f);
        }
        foreach(var ally in allies)
        {
            Civilisation allyCiv = Game.main.civs[ally];
            if (allyCiv.isActive())
            {
                ld += MathF.Min(100f, (Game.main.civs[ally].TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f);
            }
        }
        if (type.CountsOtherSubjectArmies)
        {
            foreach (var subject in overlord.subjects)
            {
                if(subject == CivID) { continue; }
                Civilisation subCiv = Game.main.civs[subject];
                SubjectType subCivType = subCiv.subjectType > -1 ? Map.main.subjectTypes[subCiv.subjectType] : Map.main.subjectTypes[0];
                if (subCivType.CountsOtherSubjectArmies)
                {
                    ld += MathF.Min(100f, (subCiv.TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f);
                }
            }
        }
        if (type.CountsOwnEconomy)
        {
            ld += MathF.Min(100f, (GetTotalIncome() + 0.1f) / (overlord.GetTotalIncome() + 0.1f) * 75f);
        }
        if (type.CountsOtherSubjectsEconomy)
        {
            foreach (var subject in overlord.subjects)
            {
                if (subject == CivID) { continue; }
                Civilisation subCiv = Game.main.civs[subject];
                SubjectType subCivType = subCiv.subjectType > -1 ? Map.main.subjectTypes[subCiv.subjectType] : Map.main.subjectTypes[0];
                if (subCivType.CountsOtherSubjectsEconomy)
                {
                    ld += MathF.Min(100f, (subCiv.GetTotalIncome() + 0.1f) / (overlord.GetTotalIncome() + 0.1f) * 75f);
                }
            }
        }

        ld -= 3f * overlord.diploRep.v;
        ld -= 0.1f * opinionOfThem[overlordID].v;
        ld += Mathf.Max(0, diploTech - overlord.diploTech) * 5f;
        ld += GetTotalDev() * 0.25f * (1f + libDesireFromDevForSubjects.v) * type.LibertyDesireFromDevelopment;
        ld += (overlord.libDesireInSubjects.v + type.LibertyDesireFlat) * 100f;
        ld += libertyDesireTemp.v;
        libertyDesire = ld;
    }
    public bool CanCoreTile(TileData tile)
    {
        if(tile == null) { return false; }
        List<TileData> neighbors = tile.GetNeighborTiles();
        foreach(var neighbor in neighbors) 
        {
            if (neighbor.civID == CivID || subjects.Contains(neighbor.civID))
            { return true; }
        }
        if (tile.isCoastal && MinimumDistBetween(this, tile.pos) <= coringRange.v)
        {
            civCoastalTiles.Sort((x, y) => TileData.evenr_distance(x.pos, tile.pos).CompareTo(TileData.evenr_distance(y.pos, tile.pos)));
            foreach (var coastal in civCoastalTiles) 
            {
                if (TileData.evenr_distance(coastal.pos, tile.pos) > coringRange.v) { return false; }
                int dist = Pathfinding.CoringDistance(coastal.pos, tile.pos);
                if(dist == -1) { continue; }
                return  dist <= coringRange.v;
            }
        }
        return false;
    }
    public int MinimumDistTo(Civilisation target)
    {
        List<Vector3Int> theirTiles = target.GetAllCivTiles();
        List<Vector3Int> ourTiles = GetAllCivTiles();
        int min = int.MaxValue;
        foreach(var tile in theirTiles)
        {
            foreach(var til2 in ourTiles)
            {
                int dist = TileData.evenr_distance(tile, til2);
                if (dist < min)
                {
                    min = dist;
                }
            }
        }
        return min;
    }
    public static int MinimumDistBetween(Civilisation target,Vector3Int tile)
    {
        List<Vector3Int> theirTiles = target.GetAllCivTiles();
        int min = int.MaxValue;
        foreach (var til2 in theirTiles)
        {
            int dist = TileData.evenr_distance(tile, til2);
            if (dist < min)
            {
                min = dist;
            }           
        }
        return min;
    }
    public List<int> GetPossibleRivals()
    {
        float ourScore = GetTotalDev() * (adminTech + diploTech + milTech);
        List<int> rivalsList = new List<int>();
        foreach (var civ in Game.main.civs)
        {
            if(civ.isActive() && civ != this && !rivals.Contains(civ.CivID) && TileData.evenr_distance(capitalPos,civ.capitalPos) < 20)
            {
                if (!allies.Contains(civ.CivID) && !subjects.Contains(civ.CivID) && overlordID != civ.CivID)
                {
                    float score = civ.GetTotalDev() * (civ.adminTech + civ.diploTech + civ.milTech);
                    if (score < ourScore * 1.4f && score > ourScore * 0.6f)
                    {
                        rivalsList.Add(civ.CivID);
                    }
                }
            }
        }
        return rivalsList;
    }
    public void UpdateStartingResources()
    {
        TileData capitalTile = Map.main.GetTile(capitalPos);
        tradeRegions[Map.main.tradeRegions[capitalTile.tradeRegion].id] = true;
        if (capitalTile.status == 0)
        {
            capitalTile.status = 2;
            controlCentres.Add(capitalPos, 2);
            capitalTile.UpdateStatusModifiers();
        }
        focus = -1;
        focusCD = 0;
        for(int i = 0; i < adminTech + 1; i++)
        {
            Map.main.TechA[i].TakeTech(CivID);
        }
        for (int i = 0; i < diploTech + 1; i++)
        {
            Map.main.TechD[i].TakeTech(CivID);
        }
        for (int i = 0; i < milTech + 1; i++)
        {
            Map.main.TechM[i].TakeTech(CivID);
        }       
        ruler.age.Activate();
        ruler.Activate();
        ruler.civID = CivID;
        heir.civID = CivID;
        float coinsIncome = (1f + taxIncome.v) * (1f + taxEfficiency.v);
        float ForceLimit = 0f;
        for (int i = 0; i < maximumAdvisors.v; i++)
        {
            RefillAdvisors();
        }
        advisorA = new Advisor();
        advisorD = new Advisor();
        advisorM = new Advisor();
        ideaGroups = new IdeaGroupData[8];
        UpdateGovernmentRank();
        RefreshForceLimit();
        Religion r = Map.main.religions[religion];
        if (r != null)
        {
            for (int i = 0; i < r.effects.Length; i++)
            {
                ApplyCivModifier(r.effects[i].name, r.effects[i].amount, r.name, r.effects[i].type);
            }
            if (religion == 2 )
            {
                religiousPoints = -1;
            }
        }
        GovernmentUI.AddGovernment(this, government);
        ApplyCivModifier(nationalIdeas.traditions[0].name, nationalIdeas.traditions[0].amount, "Tradition one",nationalIdeas.traditions[0].type);
        ApplyCivModifier(nationalIdeas.traditions[1].name, nationalIdeas.traditions[1].amount, "Tradition two",nationalIdeas.traditions[1].type);
        AddPulseEvents();
        for(int i = 0; i < Game.main.civs.Count; i++)
        {
            if(i == CivID) { opinionOfThem[i] = new Stat(200f, "Self", true);continue; }
            Civilisation civ = Game.main.civs[i];
                
        }
        if(overlordID > -1)
        {
            SetLibertyDesire();
        }
        borders.Add(GameObject.Instantiate(Map.main.civBorderPrefab,Map.main.borderTransform).GetComponent<LineRenderer>());
        countryNames.Add(GameObject.Instantiate(Map.main.civNamePrefab,UIManager.main.worldCanvasText).GetComponent<TextMeshProUGUI>());
        SetupBorderLine();
        UpdateDiplomaticCapacity();
        var tiles = GetAllCivTileDatas();
        foreach (var tileData in tiles)
        {
            tileData.SetMaxControl();
            tileData.avaliablePopulation = Mathf.Min(tileData.avaliableMaxPopulation, (int)(tileData.population * tileData.control / 100f), tileData.avaliablePopulation);
            coinsIncome += tileData.GetDailyProductionValue();
            coinsIncome += tileData.GetDailyTax();
            ForceLimit += tileData.GetForceLimit();
            tileData.UpdateUnrestModifiers();
            if (tileData.greatProject != null && tileData.greatProject.tier > 0)
            {
                if (tileData.greatProject.CanUse(this))
                {
                    tileData.greatProject.AddProject(this);
                    if (tileData.greatProject.Name == "Great Temples of Control")
                    {
                        if (tileData.greatProject.tier > 0 && tileData.religion == 3)
                        {
                            isMarshLeader = true;
                        }
                    }
                }
            }
        }
        adminPower += 100 + 16 * ruler.adminSkill;
        diploPower += 100 + 16 * ruler.diploSkill;
        milPower += 100 + 16 * ruler.milSkill;
        governmentPower = 100f;
        coins += coinsIncome * 16;
        startingEcon = coinsIncome;
        startTiles = tiles.Count;
        AddArmyTradition(ruler.milSkill * 5f + monthlyTradition.v * 6);
        SetAILevels();
    }
    public void SetAILevels()
    {
        
        AIAggressiveness = Mathf.Clamp(((ruler.milSkill + ruler.adminSkill + ruler.diploSkill) * 5f + UnityEngine.Random.Range(-40f, 40f)) , 0f,100f) * Game.main.AI_MAX_AGGRESSIVENESS/100f;
        AIAdministrative = Mathf.Clamp(ruler.adminSkill * 16f + UnityEngine.Random.Range(1f, 40f), 1f, 100f);
        AIDiplomatic = Mathf.Clamp(ruler.diploSkill * 16f + UnityEngine.Random.Range(1f, 40f), 1f, 100f);
        AIMilitary = Mathf.Clamp(ruler.milSkill * 16f + UnityEngine.Random.Range(1f, 40f), 1f, 100f);
        float total = AIAdministrative + AIDiplomatic + AIMilitary;
        AIAdministrative /= total;
        AIDiplomatic /= total;
        AIMilitary /= total;
    }
    public void UpdateGovernmentRank()
    {
        if(governmentRank >= 2)
        {
            return;
        }
        if(governmentRank == 0 && GetTotalDev() >= 300)
        {
            governmentRank = 1;
            governingCapacityMax.UpdateModifier("Government Rank", 200, EffectType.Flat);
            dailyControl.UpdateModifier("Government Rank", 0.025f, EffectType.Flat);
            diplomaticCapacityMax.UpdateModifier("Government Rank", 75f, EffectType.Flat);
            maxSettlements.UpdateModifier("Government Rank", 1f, EffectType.Flat);
            promoteSettlementCost.UpdateModifier("Government Rank", -0.08f, EffectType.Flat);
            maximumDiplomats.UpdateModifier("Government Rank", 1, EffectType.Flat);
        }
        if(governmentRank == 1 && GetTotalDev() >= 1000)
        {
            governmentRank = 2;
            governingCapacityMax.UpdateModifier("Government Rank", 400, EffectType.Flat);
            dailyControl.UpdateModifier("Government Rank", 0.05f, EffectType.Flat);
            diplomaticCapacityMax.UpdateModifier("Government Rank", 150f, EffectType.Flat);
            maxSettlements.UpdateModifier("Government Rank", 2f, EffectType.Flat);
            promoteSettlementCost.UpdateModifier("Government Rank", -0.16f, EffectType.Flat);
            maximumDiplomats.UpdateModifier("Government Rank", 2, EffectType.Flat);
        }

    }
    public void SetupBorderLine()
    {        
        var perims = PerimeterHelper.GetPerimeter(GetAllCivTiles().ToList());
        int index = 0;
        foreach (var perim in perims)
        {
            if(index >= borders.Count) { borders.Add(GameObject.Instantiate(Map.main.civBorderPrefab, Map.main.borderTransform).GetComponent<LineRenderer>()); }
            LineRenderer border = borders[index];
            if(perim.Count == 0) 
            {
                border.gameObject.SetActive(false);
                index++;
                continue;
            }
            border.gameObject.SetActive(true);
            var linePositions = PerimeterHelper.GetLinePositions(perim);

            border.positionCount = linePositions.Count;

            for (var c = 0; c < linePositions.Count; c++)
            {

                border.SetPosition(c, linePositions[c]);
            }
            border.gameObject.SetActive(true);
            Color color = c;
            if (overlordID > -1)
            {
                color = Game.main.civs[overlordID].c;
            }
            color.r *= 0.4f;
            color.g *= 0.4f;
            color.b *= 0.4f;
            border.startColor = color;
            border.endColor = color;
            SetupCountryName(perim,index);
            index++;
        }
        while (index < countryNames.Count)
        {
            LineRenderer border = borders[index];
            TextMeshProUGUI countryName = countryNames[index];
            border.gameObject.SetActive(false);
            countryName.gameObject.SetActive(false);
            index++;
        }
    }
    public void SetupCountryName(List<Vector3Int> area,int index)
    {
        if (index >= countryNames.Count) { countryNames.Add(GameObject.Instantiate(Map.main.civNamePrefab, UIManager.main.worldCanvasText).GetComponent<TextMeshProUGUI>()); }
        TextMeshProUGUI countryName = countryNames[index];
        List<Vector3Int> tiles = area.ToList();
        tiles.Sort((x, y) => PerimeterHelper.NeigborsSameCiv(Map.main.GetTile(y)).CompareTo(PerimeterHelper.NeigborsSameCiv(Map.main.GetTile(x))));
        Vector3Int centreTile = tiles[0];
        tiles.Sort((x, y) => TileData.evenr_distance(x, centreTile).CompareTo(TileData.evenr_distance(y, centreTile)));
        List<Vector3Int> included = new List<Vector3Int>();
        foreach(var tile in tiles)
        {
            int dist = TileData.evenr_distance(tile, centreTile);
            if(dist <= 1)
            {
                included.Add(tile);
                continue;
            }
            Vector3 startWorldPos = Map.main.tileMapManager.tilemap.CellToWorld(centreTile);
            Vector3 endWorldPos = Map.main.tileMapManager.tilemap.CellToWorld(tile);
            Vector3 step = (endWorldPos - startWorldPos)/(float)dist;
            bool valid = true;
            for(int i = 1; i < dist + 1; i++)
            {
                Vector3 pos = startWorldPos + step * i;
                TileData check = Map.main.GetTile(Map.main.tileMapManager.tilemap.WorldToCell(pos));
                if (check.civID != CivID)
                {
                    valid = false;
                    break;
                }                
            }
            if (valid)
            {
                included.Add(tile);
            }
        }
        included.Sort((x, y) => x.x.CompareTo(y.x));
        Vector3Int leftMostTile = included[0];
        Vector3Int rightMostTile = included[included.Count -1];
        included.Sort((x, y) => x.y.CompareTo(y.y));
        Vector3Int lowMostTile = included[0];
        Vector3Int upMostTile = included[included.Count - 1];
        Vector2 leftPos = Map.main.tileMapManager.tilemap.CellToWorld(leftMostTile);
        Vector2 rightPos = Map.main.tileMapManager.tilemap.CellToWorld(rightMostTile);
        Vector2 upPos = Map.main.tileMapManager.tilemap.CellToWorld(upMostTile);
        Vector2 lowPos = Map.main.tileMapManager.tilemap.CellToWorld(lowMostTile);
        float width = Mathf.Abs(leftPos.x - rightPos.x)/4f;
        float height = Mathf.Abs(upPos.y - lowPos.y)/4f;
        Vector3 dir = rightPos - leftPos;
        Vector2 centre = Map.main.tileMapManager.tilemap.CellToWorld(centreTile);
        if (Vector2.Distance(upPos, leftPos) < Vector2.Distance(upPos, rightPos))
        {
            Vector2 topLeft = (upPos + leftPos) / 2f;
            Vector2 bottomRight = (lowPos + rightPos) / 2f; 
            width = Mathf.Abs(topLeft.x - bottomRight.x) / 3f;
            height = Mathf.Abs(topLeft.y - bottomRight.y) / 3f;
            centre = (topLeft + bottomRight) / 2f;
        }
        else
        {
            Vector2 topRight = (upPos + rightPos) / 2f;
            Vector2 bottomLeft = (lowPos + leftPos) / 2f;           
            width = Mathf.Abs(topRight.x - bottomLeft.x) / 3f;
            height = Mathf.Abs(topRight.y - bottomLeft.y) / 3f;
            centre = (topRight + bottomLeft) / 2f;
        }
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(angle > 90) { angle = 90 - angle; }
        if (angle < -90) { angle = angle + 180; }
        RectTransform rect = countryName.GetComponent<RectTransform>();
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        rect.position = centre;
        //rect.sizeDelta = new Vector2(width, height);
        countryName.text = civName;
        countryName.fontSize = Mathf.Max(height,width,0.5f)/3f;
        countryName.gameObject.SetActive(true);
    }
    public void InitCivStats(StatData[] statData)
    {
        stats.Clear();
        for (int i = 0; i < statData.Length; i++)
        {
            StatData data = statData[i];
            Stat stat = new Stat(data.s.bs, "", data.s.f);
            stats.Add(data.name, stat);
        }
        units = new List<UnitType>() { new UnitType("Infantry", 0f, 1, 10f, this), new UnitType("Cavalry", 0f, 2, 20f, this), new UnitType("Artillery", 0f, 2, 30f, this) };
        CacheStats();
    }
    public void CacheStats()
    {
        maxSettlements = GetStat("Max Settlements");
        promoteSettlementCost = GetStat("Promote Settlement Cost");
        governingCapacityMax = GetStat("Governing Capacity");
        governingCostModifier = GetStat("Governing Cost");
        diplomaticCapacityMax = GetStat("Diplomatic Capacity");
        attritionForEnemies = GetStat("Attrition for Enemies");
        landAttrition = GetStat("Land Attrition");
        attackerDiceRoll = GetStat("Attacker Dice Roll");
        defenderDiceRoll = GetStat("Defender Dice Roll");
        generalCombatSkill = GetStat("General Combat Skill");
        generalSiegeSkill = GetStat("General Siege Skill");
        generalManeuverSkill = GetStat("General Maneuver Skill");
        rulerAdminSkill = GetStat("Ruler Admin Skill");
        rulerDiploSkill = GetStat("Ruler Diplo Skill");
        rulerMilSkill = GetStat("Ruler Military Skill");
        reformProgressGrowth = GetStat("Reform Progress Growth");
        militaryTactics = GetStat("Tactics");
        discipline = GetStat("Discipline");
        moraleMax = GetStat("Morale");
        moraleRecovery = GetStat("Morale Recovery");
        reinforceSpeed = GetStat("Reinforce Speed");
        combatWidth = GetStat("Combat Width");
        devCostMod = GetStat("Development Cost Modifier");
        devCost = GetStat("Development Cost");
        fortDefence = GetStat("Fort Defence");
        fortMaintenance = GetStat("Fort Maintenance");
        siegeAbility = GetStat("Siege Ability");
        constructionCost = GetStat("Construction Cost");
        constructionTime = GetStat("Construction Time");
        populationGrowth = GetStat("Population Growth");
        maximumPopulation = GetStat("Maximum Population");
        taxEfficiency = GetStat("Tax Efficiency");
        taxIncome = GetStat("Tax Income");
        productionValue = GetStat("Production Value");
        productionAmount = GetStat("Production Amount");
        dailyControl = GetStat("Daily Control");
        controlDecay = GetStat("Control Decay");
        regimentMaintenanceCost = GetStat("Regiment Maintenance Cost");
        regimentCost = GetStat("Regiment Cost");
        recruitmentTime = GetStat("Recruitment Time");
        movementSpeed = GetStat("Movement Speed");
        forceLimit = GetStat("Force Limit");
        prestigeDecay = GetStat("Prestige Decay");
        monthlyPrestige = GetStat("Monthly Prestige");
        armyTraditionDecay = GetStat("Army Tradition Decay");
        monthlyTradition = GetStat("Monthly Tradition");
        stabilityCost = GetStat("Stability Cost");
        coreCost = GetStat("Core Cost");
        missionaryStrength = GetStat("Missionary Strength");
        maximumAdvisors = GetStat("Maximum Advisors");
        advisorCosts = GetStat("Advisor Cost");
        advisorCostsA = GetStat("Admin Advisor Cost");
        advisorCostsD = GetStat("Diplo Advisor Cost");
        advisorCostsM = GetStat("Military Advisor Cost");
        techCosts = GetStat("Tech Cost");
        techCostsA = GetStat("Admin Tech Cost");
        techCostsD = GetStat("Diplo Tech Cost");
        techCostsM = GetStat("Military Tech Cost");
        ideaCosts = GetStat("Idea Cost");
        globalUnrest = GetStat("Global Unrest");
        trueFaithTolerance = GetStat("True Faith Tolerance");
        infidelIntolerance = GetStat("Infidel Intolerance");
        warScoreCost = GetStat("War Score Cost");
        dipAnnexCost = GetStat("Diplomatic Annexation Cost");
        libDesireFromDevForSubjects = GetStat("Liberty Desire from Subjects Development");
        battlePrestige = GetStat("Prestige from Battles");
        battleTraditon = GetStat("Army Tradition from Battles");
        diploRep = GetStat("Diplo Reputation");
        libDesireInSubjects = GetStat("Liberty Desire in Subjects");
        incomeFromSubjects = GetStat("Income from Subjects");
        improveRelations = GetStat("Improve Relations");
        aggressiveExpansionImpact = GetStat("Aggressive Expansion Impact");
        monthsOfSeperatism = GetStat("Months of Seperatism");
        coringRange = GetStat("Coring Range");
        interestPerMonth = GetStat("Interest");
        minControl = GetStat("Minimum Control");
        tradeValue = GetStat("Trade Value");
        tradePenalty = GetStat("Trade Penalty");
        tradeValPerCiv = GetStat("Trade Value per Civ in Node");
        maximumDiplomats = GetStat("Maximum Diplomats");
        maximumMissionaries = GetStat("Maximum Missionaries");
        for (int i = 0; i < units.Count; i++)
        {
            UnitType unit = units[i];
            unit.baseDamage = GetStat(unit.name + " Damage");
            unit.baseCost = GetStat(unit.name + " Cost");            
        }
    }
    void UpdateDiplomats()
    {
        foreach (var diplomat in deployedDiplomats.ToList())
        {
            if (diplomat.Action == DiplomatAction.Travelling)
            {               
                if (diplomat.Distance > 0)
                {
                    diplomat.Distance--;
                }
                if(diplomat.Distance == 0)
                {
                    deployedDiplomats.Remove(diplomat);
                }
            }
            else if(diplomat.Action == DiplomatAction.Establishing)
            {
                Civilisation target = Game.main.civs[diplomat.targetCivId];
                float relations = 0f;
                if(target.opinionOfThem[CivID].ms.Exists(i=>i.n == "Improved Relations"))
                {
                    relations = target.opinionOfThem[CivID].ms.Find(i => i.n == "Improved Relations").v;
                }
                float increase = (1f + Mathf.Max(0, diploRep.v)) * Mathf.Max(0f,1f+improveRelations.v) * 0.1f;
                target.opinionOfThem[CivID].UpdateModifier("Improved Relations", Mathf.Min(relations + increase, 100f), EffectType.Flat,true);      
                if(relations >= 100f)
                {
                    diplomat.Action = DiplomatAction.Travelling;
                }
            }
            else if (diplomat.Action == DiplomatAction.Spying)
            {                
                float increase = (1f + Mathf.Max(0, diploRep.v)) * Mathf.Max(0f, 1f + improveRelations.v) * 0.1f;
                spyNetwork[diplomat.targetCivId] = Mathf.Min(spyNetwork[diplomat.targetCivId] + increase, 100f);

                if (spyNetwork[diplomat.targetCivId] >= 100f)
                {
                    diplomat.Action = DiplomatAction.Travelling;
                }
            }
        }
    }
    public void Init()
    {
        Game.main.start.AddListener(GameStart);
        Game.main.dayTick.AddListener(DayTick);
        Game.main.hourTick.AddListener(UpdateDiplomats);
        Game.main.tenMinTick.AddListener(TenMinTick);
        Game.main.monthTick.AddListener(MonthTick);       
        truces = new int[Game.main.civs.Count];
        spyNetwork = new float[Game.main.civs.Count];
        opinionOfThem = new Stat[Game.main.civs.Count];
        religiousDevelopment = new int[Map.main.religions.Length]; 
        for (int i = 0; i < Game.main.civs.Count; i++)
        {
            Civilisation civ = Game.main.civs[i];
            opinionOfThem[i] = new Stat(0f, "Opinion", true);
        }
        mercTimers = new int[Map.main.mercenaries.Length];
        if(overlordID > -1)
        {
            Game.main.civs[overlordID].Subjugate(this);           
        }
        tradeRegions = new bool[Map.main.tradeRegions.Count];
    }
    public void BuyGeneral()
    {
        
        int points = UnityEngine.Random.Range(1, 4) + (int)(armyTradition / 20) + (ruler.active? ruler.milSkill/3 : 0);
        General general = new General(Age.zero);
        List<WeightedChoice> choices = new List<WeightedChoice>();
        choices.Add(new WeightedChoice(0, 3));
        choices.Add(new WeightedChoice(1, 1));
        choices.Add(new WeightedChoice(2, 1));
        for (int i = 0; i < points; i++)
        {
            int choice = WeightedChoiceManager.getChoice(choices).choiceID;
            switch (choice)
            {
                case 0:
                    general.combatSkill++;
                    break;
                case 1:
                    general.siegeSkill++;
                    break;
                case 2:
                    general.maneuverSkill++;
                    break;
                default:
                    break;
            }
        }
        if (Game.main.isMultiplayer)
        {
            if(NetworkManager.Singleton.IsServer || Player.myPlayer.myCivID == CivID)
            {
                Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.NewGeneral, general.combatSkill, general.siegeSkill, general.maneuverSkill);
            }
        }
        else
        {
            generals.Add(general);
        }
    }
    public void AddPrestige(float amount)
    {
        prestige = Mathf.Clamp(prestige + amount, -100f, 100f);
        taxEfficiency.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, EffectType.Flat);
        moraleMax.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, EffectType.Additive);
        populationGrowth.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, EffectType.Flat);
        improveRelations.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, EffectType.Flat);
        aggressiveExpansionImpact.UpdateModifier("Prestige", (prestige * -0.1f) / 100f, EffectType.Flat);
    }
    public void AddArmyTradition(float amount)
    {
        armyTradition = Mathf.Clamp(armyTradition + amount, 0f, 100f);
        moraleMax.UpdateModifier("Army Tradition", (armyTradition * 0.25f) / 100f, EffectType.Additive);
        siegeAbility.UpdateModifier("Army Tradition", (armyTradition * 0.05f) / 100f, EffectType.Flat);
        moraleRecovery.UpdateModifier("Army Tradition", (armyTradition * 0.1f) / 100f, EffectType.Flat);
        populationGrowth.UpdateModifier("Army Tradition", (armyTradition * 0.1f) / 100f, EffectType.Flat);
    }
    void MonthTick()
    {
        if(civTiles.Count == 0 || civTileDatas.Count == 0) { return; }
        if(events.Count > 0)
        {
            int index = WeightedChoiceManager.getChoice(events).choiceID;
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    int tileIndex = UnityEngine.Random.Range(0, GetAllCivTileDatas().Count);
                    Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.SendEvent, index, tileIndex);
                }
            }
            else
            {
                if (index != -1)
                {
                    EventData eventData = Map.main.pulseEvents[index];
                    if (eventData.affectsCapital)
                    {
                        eventData.province = Map.main.GetTile(capitalPos);
                    }
                    else if (eventData.affectsRandomProvince)
                    {
                        eventData.province = GetAllCivTileDatas()[UnityEngine.Random.Range(0, GetAllCivTileDatas().Count)];
                    }
                    SendEvent(eventData);
                    eventHistory.Add(Map.main.pulseEvents.ToList().IndexOf(eventData));                  
                }
            }
            AddPulseEvents();
        }
        if (Game.main.isMultiplayer)
        {
            int index = 0;
            foreach (General general in generals.ToList())
            {
                if (!general.active) 
                {
                    Game.main.multiplayerManager.CivActionRpc(CivID, MultiplayerManager.CivActions.GeneralDeath,index);
                }
                else
                {
                    index++;
                }
            }
        }
        else
        {
            generals.RemoveAll(i => !i.active);
        }
            for (int i = 0; i < mercTimers.Length; i++)
            {
                if (mercTimers[i] > 0)
                {
                    mercTimers[i]--;
                }
            }
            if (focusCD > 0)
            {
                focusCD--;
            }
        }
    void AddPulseEvents()
    {
        events.Clear();
        events.Add(new WeightedChoice(-1, 1000));
        for (int i = 0; i < Map.main.pulseEvents.Length;i++)
        {
            if(eventHistory.Count > 0)
            {
                if (eventHistory[eventHistory.Count - 1] == i)
                {
                    continue;
                }
            }
            EventData evt = Map.main.pulseEvents[i];            
            if (evt.CanFire(this))
            {
                events.Add(new WeightedChoice(i, 100));
            }
        }
    }
    public void AddRulerTraits()
    {
        if (!ruler.active) { return; }
        List<Trait> traits = ruler.traits.ToList();
        foreach (var trait in traits)
        {
            List<Effect> effects = trait.effects.ToList();
            foreach (var effect in effects)
            {
                ApplyCivModifier(effect.name, effect.amount, trait.name, effect.type);
            }
        }
    }
    public void RemoveRulerTraits(Ruler dead)
    {
        List<Trait> traits = dead.traits.ToList();
        foreach (var trait in traits)
        {
            List<Effect> effects = trait.effects.ToList();
            foreach (var effect in effects)
            {
                Stat stat = GetStat(effect.name);
                if (stat != null)
                {
                    Modifier mod = stat.ms.Find(i => i.n == trait.name);
                    if (mod != null)
                    {
                        stat.RemoveModifier(mod);
                    }
                } 
            }
        }
    }
    public void RemoveAdvisor(Advisor advisor)
    {
        if(advisor == null || advisor.effects == null || advisor.effects.name == null) { return; }
        Stat stat = GetStat(advisor.effects.name);
        if (stat != null)
        {
            Modifier mod = stat.ms.Find(i => i.n == "Advisor");
            if (mod != null)
            {
                stat.RemoveModifier(mod);
            }
        }   
    }
    public void AssignAdvisor(Advisor advisor)
    {
        coins -= advisor.HireCost(this);
        if(advisor.type == 0)
        {
            if (advisorA.active)
            {
                RemoveAdvisor(advisorA);
            }
            advisorA = advisor;
            advisorsA.Remove(advisor);
        }
        else if (advisor.type == 1)
        {
            if (advisorD.active)
            {
                RemoveAdvisor(advisorD);
            }
            advisorD = advisor;
            advisorsD.Remove(advisor);
        }
        else
        {
            if (advisorM.active)
            {
                RemoveAdvisor(advisorM);
            }
            advisorM = advisor;
            advisorsM.Remove(advisor);
        }
        ApplyCivModifier(advisor.effects.name, advisor.effects.amount, "Advisor", advisor.effects.type);
    }
    void TenMinTick()
    {
        for (int i = 0; i < truces.Length;i++)
        {
            if (truces[i] > 0)
            {
                truces[i]--;
            }
        }
        UpdateGovernmentRank();


        for (int i = 0; i < opinionOfThem.Length;i++)
        {
            Civilisation civ = Game.main.civs[i];
            opinionOfThem[i].d = (1f + civ.improveRelations.v);
        }
        if (maxAE < 0)
        {
            maxAE += (1f + improveRelations.v) / 1440f;
        }

        foreach (var tileData in GetAllCivTileDatas())
        {
            if (tileData.unitTimer > 0 && tileData.unitQueue.Count > 0 && !tileData.occupied && !tileData.underSiege)
            {
                tileData.unitTimer--;
                if (tileData.unitTimer == 0)
                {
                    RecruitData data = tileData.unitQueue[0];                    
                    tileData.unitQueue.RemoveAt(0);
                    if (data.unitType == UnitTypeID.Regiment) {
                        if(avaliablePopulation < 1000) { tileData.unitQueue.Add(data);continue; }
                        if (Game.main.isMultiplayer)
                        {
                            if (NetworkManager.Singleton.IsServer)
                            {
                                Game.main.multiplayerManager.NewUnitRpc(tileData.pos, data.unitID, 0);
                            }
                        }
                        else
                        {
                            tileData.CreateNewArmy(data.unitID);
                        }
                    }
                    else if (data.unitType == UnitTypeID.Boat)
                    {
                        if (avaliablePopulation < 200) { tileData.unitQueue.Add(data); continue; }
                        if (Game.main.isMultiplayer)
                        {
                            if (NetworkManager.Singleton.IsServer)
                            {
                                Game.main.multiplayerManager.NewUnitRpc(tileData.pos, data.unitID, 2);
                            }
                        }
                        else
                        {
                            tileData.CreateNewBoat(data.unitID);
                        }
                    }
                    else if (data.unitType == UnitTypeID.Mercenary)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            if (NetworkManager.Singleton.IsServer)
                            {
                                Game.main.multiplayerManager.NewUnitRpc(tileData.pos, data.unitID, 1);
                            }
                        }
                        else
                        {
                            tileData.CreateMercenaryGroup(data.unitID);
                        }
                    }
                    
                    if (tileData.unitQueue.Count > 0)
                    {
                        tileData.unitTimer = tileData.GetRecruitTime();
                    }
                }
            }            
        }
    }
    public void RefillAdvisors()
    {
        
        foreach (var advisor in advisorsA.ToList())
        {
            if (!advisor.active)
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {                       
                        Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.RemoveFromAdvisorPool, 0, advisorsA.IndexOf(advisor));
                    }
                }
                else
                {
                    advisorsA.Remove(advisor);
                }                
            }
        }
        if(advisorsA.Count < maximumAdvisors.v)
        {
            int index = -1;
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Advisor ad = Advisor.NewRandomAdvisor(0, CivID, ref index);
                    Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.FillAdvisorPool, 0, index, ad.skillLevel, ad.age.y);
                }
            }
            else
            {
                advisorsA.Add(Advisor.NewRandomAdvisor(0, CivID, ref index));
            }
        }
        foreach (var advisor in advisorsD.ToList())
        {
            if (!advisor.active)
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.RemoveFromAdvisorPool, 1, advisorsD.IndexOf(advisor));
                    }
                }
                else
                {
                    advisorsD.Remove(advisor);
                }
            }
        }
        if (advisorsD.Count < maximumAdvisors.v)
        {
            int index = -1;
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Advisor ad = Advisor.NewRandomAdvisor(1, CivID, ref index);
                    Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.FillAdvisorPool, 1, index, ad.skillLevel, ad.age.y);
                }
            }
            else
            {
                advisorsD.Add(Advisor.NewRandomAdvisor(1, CivID, ref index));
            }
        }
        foreach (var advisor in advisorsM.ToList())
        {
            if (!advisor.active)
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.RemoveFromAdvisorPool, 2, advisorsM.IndexOf(advisor));
                    }
                }
                else
                {
                    advisorsM.Remove(advisor);
                }
            }
        }
        if (advisorsM.Count < maximumAdvisors.v)
        {
            int index = -1;
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Advisor ad = Advisor.NewRandomAdvisor(2, CivID, ref index);
                    Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.FillAdvisorPool, 2, index, ad.skillLevel, ad.age.y);
                }
            }
            else
            {
                advisorsM.Add(Advisor.NewRandomAdvisor(2, CivID, ref index));
            }

        }
    }
    public void RefreshForceLimit()
    {
        float ForceLimit = 3f;
        foreach (var tileData in GetAllCivTileDatas())
        {
            ForceLimit += tileData.GetForceLimit();
        }
        forceLimit.ChangeBaseStat(ForceLimit);
    }
    public int GetStabilityCost()
    {
        int baseC = 100;
        return (int)(baseC * (1f + stabilityCost.v));
    }
    public float GetAverageControl()
    {
        if(GetAllCivTileDatas().Count == 0) { return 0f; }
        float control = 0;
        foreach (var tileData in GetAllCivTileDatas())
        {
            control += tileData.control;
        }
        return control / GetAllCivTileDatas().Count;
    }
    public int GetTotalTilePopulation()
    {
        int pop = 0;
        foreach (var tileData in GetAllCivTileDatas())
        {
            pop += tileData.avaliablePopulation;            
        }
        return pop;
    }
    public float GetTotalIncome()
    {
        return TaxIncome() + TradeIncome() + ProductionIncome() + GetSubjectIncome();
    }
    public int GetTotalPopulationGrowth()
    {
        int pop = 0;
        foreach (var tileData in GetAllCivTileDatas())
        {
            if (tileData.avaliablePopulation < tileData.avaliableMaxPopulation)
            {
                pop += (int)(tileData.populationGrowth * tileData.control / 100f);
            }
        }
        return pop;
    }
    public float GetLoanSize()
    {
        return 0.5f * GetTotalDev();
    }
    public int GetMaxLoans()
    {
        float income = (ProductionIncome() + TaxIncome());
        return (int)(income * 30f / (GetLoanSize() * interestPerMonth.v/100f));
    }
    public float GetInterestPayment()
    {
        float cost = 0f;
        foreach(var loan in loans)
        {
            cost += loan.GetInterestValue(this);
        }
        return cost;
    }
    public int GetTotalMaxPopulation()
    {
        int pop = 0;
        foreach (var tileData in GetAllCivTileDatas())
        {
            pop += tileData.avaliableMaxPopulation;
        }
        return pop;
    }
    public float GetBalance()
    {
        float taxV = TaxIncome();
        float prodV = ProductionIncome();
        float armyM = ArmyMaintainance();
        float advisorM = AdvisorMaintainance();
        float fortM = FortMaintenance();
        float interestM = GetInterestPayment();
        return taxV + prodV - armyM - advisorM - fortM - interestM;
    }
    public int AddPopulation(int targetAmount)
    {
        int amount = 0;
        List<TileData> tiles = GetAllCivTileDatas();
        tiles.RemoveAll(i => i.occupied || i.underSiege);
        tiles.RemoveAll(i => i.population == i.maxPopulation);
        if (tiles.Count == 0) { return 0; }
        int amountPerTile = targetAmount / tiles.Count;
        foreach (var tile in tiles)
        {
            int used = Mathf.Min(amountPerTile,tile.maxPopulation - tile.population);
            tile.avaliablePopulation += (int)(used * tile.control/100f);
            tile.population += used;
            amount += used;
        }
        if (amount < targetAmount)
        {
            tiles.Sort((x, y) => (x.avaliablePopulation / (x.avaliableMaxPopulation+1)).CompareTo((y.avaliablePopulation / (y.avaliableMaxPopulation+1))));
            foreach (var tile in tiles)
            {
                int remainder = targetAmount - amount;
                if (amount >= targetAmount) { break; }
                int used = Mathf.Min(remainder,tile.maxPopulation - tile.population);
                tile.avaliablePopulation += (int)(used * tile.control/100f);
                tile.population += used;
                amount += used;
            }
        }
        return amount;
    }
    public int RemovePopulation(int targetAmount)
    {
        int amount = 0;
        List<TileData> tiles = GetAllCivTileDatas();
        tiles.RemoveAll(i => i.occupied || i.underSiege);
        tiles.RemoveAll(i => i.avaliablePopulation <= 0 || i.avaliableMaxPopulation == 0);
        if (tiles.Count == 0) { return 0; }
        int apd = targetAmount / GetTotalDev();
        foreach (var tile in tiles)
        {
            int used = Mathf.Min(apd * tile.totalDev,Mathf.Max(tile.avaliablePopulation - tile.developmentB * 200,0));
            tile.avaliablePopulation -= used;
            tile.population -= used;
            amount += used;
        }        
        if (amount < targetAmount)
        {            
            tiles.Sort((x, y) => (y.population).CompareTo(x.population));
            foreach (var tile in tiles)
            {
                int remainder = targetAmount - amount;
                if (amount >= targetAmount) { break; }
                int used = Mathf.Min(remainder, tile.avaliablePopulation);
                tile.avaliablePopulation -= used;
                tile.population -= used;
                amount += used;
            }
        }
        //Debug.Log("Removed " + amount + " population from " + civName);
        return amount;
    }
    public float ProductionIncome()
    {
        float coinsIncome = 0f;
        foreach (var tileData in GetAllCivTileDatas())
        {         
            coinsIncome += Mathf.Max(0, tileData.GetDailyProductionValue());           
        }
        return coinsIncome ;
    }
    public float TradeIncome()
    {
        float tradeIncome = 0f;
        TileData capital = Map.main.GetTile(capitalPos);
        string capitalRegion = capital.tradeRegion;
        for(int i = 0; i < tradeRegions.Length;i++)
        {
            if (tradeRegions[i])
            {
                TradeRegion region = Map.main.tradeRegions.Values.ToArray()[i];
                tradeIncome += Mathf.Max(0, region.GetTradeIncome(this) * (region.name == capitalRegion ? 1f : 1f - tradePenalty.v));
            }
        }
        return tradeIncome * (1f + tradeValue.v);
    }
    public float TaxIncome()
    {
        float coinsIncome = (1f + taxIncome.v) * (1f + taxEfficiency.v);
        foreach (var tileData in GetAllCivTileDatas())
        {
            coinsIncome += Mathf.Max(0, tileData.GetDailyTax());
        }
        return coinsIncome;
    }
    public float FortMaintenance()
    {
        float fortM = 0f;
        foreach (var tileData in GetAllCivTileDatas())
        {
            if (tileData.hasFort)
            {
                fortM += Mathf.Max(0,(tileData.fortLevel - (tileData.pos == capitalPos ? 1 : 0)) * (1f + tileData.localFortMaintenance.v));
            }
        }
        return fortM * (1f + fortMaintenance.v);
    }
    public float DiplomaticExpenses()
    {
        float expenses = 0f;

        if (overlordID > -1 && libertyDesire < 50f)
        {
            SubjectType type = subjectType > -1 ? Map.main.subjectTypes[subjectType] : Map.main.subjectTypes[0];
            expenses += Mathf.Max(0, GetTotalIncome() * (type.IncomePercentage + Game.main.civs[overlordID].incomeFromSubjects.v));
        }
        return expenses;
    }
    public void AddStability(int amount)
    {
        stability = Mathf.Clamp(stability + amount, -3, 3);
        stabilityCost.UpdateModifier("Stability", stability > 0 ? stability * 0.50f : 0f, EffectType.Flat);
        globalUnrest.UpdateModifier("Stability", stability > 0 ? stability * -1f : stability * -2f, EffectType.Flat);
        dailyControl.UpdateModifier("Stability", stability > 0 ? stability * 0.01f : stability * 0.02f, EffectType.Flat);
        taxEfficiency.UpdateModifier("Stability", stability * 0.05f, EffectType.Flat);
        interestPerMonth.UpdateModifier("Stability", stability < 0 ? stability * -1f : 0f, EffectType.Flat);
    }
    public void DeclareBankruptcy()
    {
        if (loans.Count <= 0) { return; }
        loans.Clear();
        interestPerMonth.AddModifier(new Modifier(0.05f, ModifierType.Flat, "Bankruptcy", 25920));
        moraleMax.AddModifier(new Modifier(-0.9f, ModifierType.Additive, "Bankruptcy", 25920));
        dailyControl.AddModifier(new Modifier(-0.05f, ModifierType.Flat, "Bankruptcy", 25920));
        reinforceSpeed.AddModifier(new Modifier(-0.25f, ModifierType.Flat, "Bankruptcy", 25920));
        populationGrowth.AddModifier(new Modifier(-1f, ModifierType.Flat, "Bankruptcy", 25920));
        techCosts.AddModifier(new Modifier(0.5f, ModifierType.Flat, "Bankruptcy", 25920));
        ideaCosts.AddModifier(new Modifier(0.5f, ModifierType.Flat, "Bankruptcy", 25920));
        coins = 100f;
        AddPrestige(-100f);
        AddStability(-3);
        adminPower = -100;
        diploPower = -100;
        milPower = -100;
        RemoveAdvisor(advisorA);
        advisorA.active = false;
        RemoveAdvisor(advisorD);
        advisorD.active = false;
        RemoveAdvisor(advisorM);
        advisorM.active = false;
        foreach (var tileData in GetAllCivTileDatas())
        {
            tileData.localUnrest.TryRemoveModifier("Recent Uprising");
            tileData.buildQueue.Clear();
            tileData.unitQueue.Clear();
            if (tileData.coreTimer > -1)
            {
                tileData.coreTimer = -1;
            }
        }
    }
    public void TakeLoan()
    {
        if (loans.Count < GetMaxLoans())
        {
            Loan newLoan = new Loan(GetLoanSize());
            coins += newLoan.value;
            loans.Add(newLoan);
        }
        else
        {
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Game.main.multiplayerManager.CivActionRpc(CivID, MultiplayerManager.CivActions.Bankruptcy, 0);
                }
            }
            else
            {
                DeclareBankruptcy();
            }
        }
    }
    public float ArmyMaintainance()
    {
        float armyCosts = 0f;
        foreach(var army in armies)
        {
            foreach(var regiment in army.regiments)
            {
                float mult = regiment.mercenary ? (0.5f + Game.main.gameTime.y * 0.25f): 1f;
                armyCosts += mult * units[regiment.type].baseCost.v * (float)regiment.size / (float)regiment.maxSize * 0.25f / 12f;
            }
        }
        if (TotalMaxArmySize() / 1000f > forceLimit.v)
        {
            float increase = (forceLimit.v + (TotalMaxArmySize() / 1000f - forceLimit.v) * 2) / forceLimit.v;
            armyCosts *= increase;
        }
        armyCosts *= (1f + regimentCost.v);
        armyCosts *= (1f + regimentMaintenanceCost.v);
        return armyCosts;
    }
    public float AdvisorMaintainance()
    {
        float advisorCosts = 0f;
        if (advisorA!= null && advisorA.active) { advisorCosts += advisorA.Salary(this); }
        if (advisorD != null && advisorD.active) { advisorCosts += advisorD.Salary(this); }
        if (advisorM != null && advisorM.active) { advisorCosts += advisorM.Salary(this); }
        return advisorCosts;
    }
    public float AquatismFervorDaily()
    {
        float points = 0.1f + (stability * 0.1f) - (loans.Count * 0.1f) - (overextension * 0.01f) + (ruler.active ? (ruler.adminSkill + ruler.diploSkill + ruler.milSkill) * 0.02f : 0f)
                 + (advisorA.active ? (advisorA.skillLevel) * 0.02f : 0f) + (advisorD.active ? (advisorD.skillLevel) * 0.02f : 0f) + (advisorM.active ? (advisorM.skillLevel) * 0.02f : 0f);
        points -= religiousPoints * 0.01f;
        return points;
    }
    public float DjinnUnityDaily()
    {
        float points = (GetWars().Count * 2f) + (armyTradition * 0.05f) + (TotalArmySize()/1000f * 0.02f);
        points -= religiousPoints * 0.05f;
        return points;
    }
    public float HungerDaily()
    {
        float points = 0.1f + (civTiles.Count * 0.01f) + (diplomaticCapacity > diplomaticCapacityMax.v ? (diplomaticCapacity - diplomaticCapacityMax.v)/ diplomaticCapacityMax.v * 1f : 0f);
        return points;
    }
    public float DjinnStandingDaily()
    {
        float points = GetWars().Count > 0f? 3f : -3f;
        points -= religiousPoints * 0.03f;
        return points;
    }
    public void UpdateReligion()
    {
        if(religion == 0)
        {
            float increase = AquatismFervorDaily();
            religiousPoints = Mathf.Clamp(religiousPoints + increase, 0, 100);
            for (int i = 0; i < Map.main.religions[0].religiousMechanicEffects.Length; i++)
            {
                Effect effect = Map.main.religions[0].religiousMechanicEffects[i];
                bool active = religiousPoints > (i * 100 / Map.main.religions[0].religiousMechanicEffects.Length);
                GetStat(effect.name).UpdateModifier("Aquatism Fervor", active ? effect.amount : 0f, effect.type);
            }
            if (increase < 0 && CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.aquatismDropping;
                notification.description = "You are losing Aquatism Fervor";
                NotificationsUI.AddNotification(notification);
            }
        }
        else if (religion == 1)
        {
            religiousPoints = Mathf.Clamp(religiousPoints + DjinnUnityDaily(), 0, 100);
            Effect effect = Map.main.religions[1].religiousMechanicEffects[0];          
            GetStat(effect.name).UpdateModifier("Djinn Unity",  effect.amount *(religiousPoints/100f), effect.type);
            if (religiousPoints >= 50 && CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.djinnFavor;
                notification.description = "You can use religious mechanic";
                NotificationsUI.AddNotification(notification);
            }
        }
        else if (religion == 2)
        {
            if (religiousPoints == -1 && CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.pickDeity;
                notification.description = "You can use religious mechanic";
                NotificationsUI.AddNotification(notification);
            }
        }
        else if (religion == 3)
        {
            religiousPoints = Mathf.Clamp(religiousPoints + HungerDaily(), 0, 100);
            Effect cd = Map.main.religions[3].religiousMechanicEffects[0];
            Effect tv = Map.main.religions[3].religiousMechanicEffects[1];
            Effect gu = Map.main.religions[3].religiousMechanicEffects[2];
            float cdVal = religiousPoints > 50f ? (religiousPoints - 50f) / 50f  * cd.amount: (religiousPoints - 50f) / 500f * cd.amount;
            GetStat(cd.name).UpdateModifier("Hunger", cdVal, cd.type);

            float tvVal = religiousPoints > 50f ? (religiousPoints - 50f) / 50f * tv.amount : (religiousPoints - 50f) / 500f * tv.amount;
            GetStat(tv.name).UpdateModifier("Hunger", tvVal, tv.type);

            float guVal = religiousPoints > 50f ? (religiousPoints - 50f) / 50f * gu.amount : (religiousPoints - 50f) / 500f * gu.amount;
            GetStat(gu.name).UpdateModifier("Hunger", guVal, gu.type);

            if (religiousPoints > 30 && CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.highHunger;
                notification.description = "You have high hunger";
                NotificationsUI.AddNotification(notification);
            }
        }
        else if (religion == 4)
        {
            religiousPoints = Mathf.Clamp(religiousPoints + DjinnStandingDaily(), -100, 100);
            Effect peace1 = Map.main.religions[4].religiousMechanicEffects[2];
            Effect peace2 = Map.main.religions[4].religiousMechanicEffects[3];

            Effect war1 = Map.main.religions[4].religiousMechanicEffects[0];
            Effect war2 = Map.main.religions[4].religiousMechanicEffects[1];

            float peaceVal1 = religiousPoints >= 0? 0f : -religiousPoints/100f * peace1.amount;
            GetStat(peace1.name).UpdateModifier("Djinn Standing", peaceVal1, peace1.type);
            float peaceVal2 = religiousPoints >= 0 ? 0f : -religiousPoints/100f * peace2.amount;
            GetStat(peace2.name).UpdateModifier("Djinn Standing", peaceVal2, peace2.type);

            float warval1 = religiousPoints <= 0 ? 0f : religiousPoints/100f * war1.amount;
            GetStat(war1.name).UpdateModifier("Djinn Standing", warval1, war1.type);
            float warval2 = religiousPoints <= 0 ? 0f : religiousPoints/100f * war2.amount;
            GetStat(war2.name).UpdateModifier("Djinn Standing", warval2, war2.type);
        }
    }
    public void Rebirth()
    {
        if (!countryNames[0].gameObject.activeSelf)
        {
            Game.main.dayTick.AddListener(DayTick);
            Game.main.tenMinTick.AddListener(TenMinTick);
            SetupBorderLine();
            opinionOfThem = new Stat[Game.main.civs.Count];
            for (int i = 0; i < Game.main.civs.Count; i++)
            {
                Civilisation civ = Game.main.civs[i];
                opinionOfThem[i] = new Stat(0f, "Opinion", true);
            }
        }
    }
    public float GetMonthlyPrestigeChange()
    {
        float change = monthlyPrestige.v;
        change += -prestige * prestigeDecay.v;
        return change;
    }
    public float GetMonthlyTraditionChange()
    {
        float change = monthlyTradition.v;
        change += -armyTradition * armyTraditionDecay.v;
        return change;
    }
    public void DayTick()
    {
        revanchism = Mathf.Clamp(revanchism - 0.005f, 0f, 1f);
        if(revanchism > 0f)
        {
            taxEfficiency.UpdateModifier("Revanchism", revanchism * 0.5f, EffectType.Flat);
            fortDefence.UpdateModifier("Revanchism", revanchism * 0.25f, EffectType.Flat);
            populationGrowth.UpdateModifier("Revanchism", revanchism * 0.5f, EffectType.Flat);
            monthlyTradition.UpdateModifier("Revanchism", revanchism * 1f, EffectType.Flat);
            globalUnrest.UpdateModifier("Revanchism", revanchism * -5f, EffectType.Flat);
            interestPerMonth.UpdateModifier("Revanchism", revanchism * -1f, EffectType.Flat);
            taxIncome.UpdateModifier("Revanchism", revanchism * 2f, EffectType.Flat);
        }
        for (int i = 0; i < Game.main.civs.Count; i++)
        { 
            Civilisation civ = Game.main.civs[i];
            opinionOfThem[i].UpdateModifier("Religion",civ.religion == religion ? 20 : -20, EffectType.Flat);            
        }
        if (CivID == Player.myPlayer.myCivID)
        {
            NotificationsUI.main.ClearNotifications();
        }
        if(civTiles.Count == 0)
        {
            Game.main.dayTick.RemoveListener(DayTick);
            Game.main.tenMinTick.RemoveListener(TenMinTick);
            countryNames[0].gameObject.SetActive(false);
            borders.ForEach(i =>i.gameObject.SetActive(false));
            GameObject.Destroy(capitalIndicator);
            if(overlordID > -1 && Game.main.civs[overlordID].subjects.Contains(CivID))
            {
                Game.main.civs[overlordID].subjects.Remove(CivID);
                overlordID = -1;
            }
            foreach (var army in armies.ToList())
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        army.GetComponent<NetworkArmy>().ExitEnterRpc(false, army.pos);
                        GameObject.Destroy(army.gameObject);                        
                    }
                }
                else 
                { 
                    army.OnExitTile(army.tile);                
                    GameObject.Destroy(army.gameObject);
                }           
            }
            foreach (var fleet in fleets.ToList())
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        fleet.GetComponent<NetworkFleet>().ExitEnterRpc(false, fleet.pos);
                        GameObject.Destroy(fleet.gameObject);
                    }
                }
                else
                {
                    fleet.OnExitTile(fleet.tile);
                    GameObject.Destroy(fleet.gameObject);
                }
            }
            allies.ToList().ForEach(i => BreakAlliance(i));
            foreach (var war in GetWars())
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        if (war.attackerCiv == this || war.defenderCiv == this)
                        {
                            if (war.networkWar != null)
                            {
                                war.networkWar.EndWarRpc();                               
                            }
                        }
                        else
                        {
                            if (war.networkWar != null)
                            {
                                war.networkWar.EndWarRpc(CivID);                                
                            }
                        }
                    }
                }
                else
                {
                    if (war.attackerCiv == this || war.defenderCiv == this)
                    {
                        war.EndWar();
                    }
                    else
                    {
                        war.LeaveWar(CivID);
                    }
                }
            }
            return;
        }
        UpdateDiplomaticCapacity();
        foreach (var rival in rivals)
        {
            if(rival > -1)
            {
                Civilisation rivalCiv = Game.main.civs[rival]; 
                opinionOfThem[rival].UpdateModifierDuration("Rival", -100, 1729, EffectType.Flat);
                rivalCiv.opinionOfThem[CivID].UpdateModifierDuration("Rivals Us", -50, 1729, EffectType.Flat);
                foreach (var ally in rivalCiv.allies)
                {
                    opinionOfThem[ally].UpdateModifierDuration("Ally of Rival",-25, 1729,EffectType.Flat);
                }
                foreach (var riv in rivalCiv.rivals)
                {
                    if (riv > -1)
                    {
                        opinionOfThem[riv].UpdateModifierDuration("Rival of Rival", 20, 1729, EffectType.Flat);
                    }
                }
                foreach (var riv in rivalCiv.atWarWith)
                {
                    opinionOfThem[riv].UpdateModifierDuration("At war with Rival", 10, 1729, EffectType.Flat);
                }
            }
        }
        AddPrestige(GetMonthlyPrestigeChange() * 1f / 30f);
        AddArmyTradition(GetMonthlyTraditionChange() * 1f/30f);
        UpdateReligion();
        if (overlordID > -1)
        {
            SetLibertyDesire();
            if(integrating && libertyDesire < 50f)
            {
                Civilisation overlord = Game.main.civs[overlordID];
                if (overlord.diploPower > 0 && (int)overlord.diploRep.v > -2)
                {
                    annexationProgress += 2 + (int)overlord.diploRep.v;
                    overlord.diploPower -= 2 + (int)overlord.diploRep.v;
                    if(annexationProgress >= GetIntegrationCost(overlord))
                    {
                        foreach(var tileData in GetAllCivTileDatas())
                        {
                            if (Game.main.isMultiplayer)
                            {
                                if (NetworkManager.Singleton.IsServer)
                                {
                                    Game.main.multiplayerManager.TileActionRpc(tileData.pos, MultiplayerManager.TileActions.OccupyIntegrated, overlordID);
                                }
                            }
                            else
                            {
                                tileData.TransferOccupation(overlordID, true);
                            }                     
                            overlord.AddPrestige(0.25f * tileData.totalDev);
                        }
                        RemoveSubjugation();
                        updateBorders = true;
                        overlord.updateBorders = true;
                        return;
                    }
                }
            }
        }
        RefillAdvisors();
        overextension = 0f;
        isMarshLeader = false;
        religiousUnity = 0f;
        religiousDevelopment = new int[Map.main.religions.Length];
        int totalDev = GetTotalDev();
        foreach(var tileData in GetAllCivTileDatas())
        {
            if(tileData.civID != CivID) { continue; }
            religiousDevelopment[tileData.religion] += tileData.totalDev;
            if(tileData.religion == religion)
            {
                tileData.localUnrest.UpdateModifier("Religion", -trueFaithTolerance.v, EffectType.Flat);
                religiousUnity += tileData.totalDev;
                
            }
            else
            {
                tileData.localUnrest.UpdateModifier("Religion", infidelIntolerance.v, EffectType.Flat);
            }
            tileData.SetMaxControl();
            if(tileData.greatProject != null)
            {
                GreatProject gp = tileData.greatProject;
                if(gp.Name == "Great Temples of Control")
                {
                    if(gp.tier > 0 && gp.CanUse(this) && tileData.religion == 3)
                    {
                        isMarshLeader = true;
                    }
                }
                if (gp.isBuilding)
                {
                    gp.buildTimer++;
                    if (gp.buildTimer >= gp.GetTime(tileData, this))
                    {
                        gp.isBuilding = false;
                        gp.buildTimer = 0;
                        gp.Upgrade(this);
                    }
                }
                else if (gp.tier < 3 && coins >= gp.GetCost(tileData,this) && gp.CanUse(this))
                {
                    if (CivID == Player.myPlayer.myCivID)
                    {
                        Notification notification = NotificationsUI.main.greatProj;
                        notification.description = "You can upgrade a great project";
                        notification.province = tileData;
                        NotificationsUI.AddNotification(notification);
                    }
                    else if (Game.main.Started && !isPlayer && GetBalance() > 0)
                    {
                        float cost = gp.GetCost(tileData, this);
                        if (coins >= cost)
                        {
                            if (Game.main.isMultiplayer)
                            {
                                Game.main.multiplayerManager.TileActionRpc(tileData.pos, MultiplayerManager.TileActions.UpgradeGreatProj, CivID);
                            }
                            else
                            {
                                gp.isBuilding = true;
                                coins -= cost;
                            }
                        }
                    }
                }
            }
            if (tileData.isConverting && !tileData.occupied)
            {
                tileData.conversionProgress += Mathf.Max(0,missionaryStrength.v - tileData.GetConvertResistance())/100f;
                if (tileData.conversionProgress >= 1 || tileData.religion == religion)
                {
                    tileData.religion = religion;
                    tileData.isConverting = false;
                    tileData.conversionProgress = 0;
                    deployedMissionaries.Remove(tileData.pos);
                    tileData.SetMaxControl();
                    if (tileData.localUnrest.ms.Exists(i => i.n == "Converting Religion"))
                    {
                        tileData.localUnrest.RemoveModifier(tileData.localUnrest.ms.Find(i => i.n == "Converting Religion"));
                    }
                }
            }
            if(tileData.religion != religion)
            {
                if (tileData.isConverting)
                {
                    if (!tileData.localUnrest.ms.Exists(i => i.n == "Converting Religion"))
                    {
                        tileData.localUnrest.UpdateModifier("Converting Religion",6,EffectType.Flat);
                    }
                }
                else
                {
                    if (tileData.localUnrest.ms.Exists(i => i.n == "Converting Religion"))
                    {
                        tileData.localUnrest.RemoveModifier(tileData.localUnrest.ms.Find(i => i.n == "Converting Religion"));
                    }
                }
            }
            if (tileData.coreTimer > -1 && !tileData.occupied && !atWarWith.Exists(i=>tileData.cores.Contains(i)))
            {
                tileData.coreTimer--;
                if(tileData.coreTimer == 0)
                {
                    tileData.cores.Add(CivID);
                    tileData.coreTimer = -1;
                    tileData.SetMaxControl();
                    tileData.control = Mathf.Clamp(tileData.control + 15f, 0f, tileData.maxControl);
                    if (tileData.localUnrest.ms.Exists(i => i.n == "Not a core"))
                    {
                        tileData.localUnrest.RemoveModifier(tileData.localUnrest.ms.Find(i => i.n == "Not a core"));
                    }
                }
            }
            if (!tileData.hasCore)
            {                          
                overextension += tileData.totalDev * 0.8f;     
                if(!tileData.localUnrest.ms.Exists(i=>i.n == "Not a core"))
                {
                    tileData.localUnrest.AddModifier(new Modifier(6, ModifierType.Flat, "Not a core"));
                }
                if (tileData.coreTimer == -1 && CivID == Player.myPlayer.myCivID)
                {
                    Notification notification = NotificationsUI.main.nonCoreProvinces;
                    notification.description = "You have provinces that need coring";
                    NotificationsUI.AddNotification(notification);
                }
            }
            if (!tileData.occupied)
            {
                tileData.seperatism = Mathf.Clamp(tileData.seperatism - 1, 0, 360);
                tileData.control = Mathf.Clamp(tileData.control + tileData.dailyControl.v + dailyControl.v, 0f, tileData.maxControl);
                tileData.population = Mathf.Max(0,Mathf.Min(tileData.maxPopulation, tileData.population + tileData.populationGrowth));
                tileData.avaliablePopulation = Mathf.Max(0, Mathf.Min(tileData.avaliableMaxPopulation, tileData.population, tileData.avaliablePopulation + (int)(tileData.populationGrowth * tileData.control / 100f + tileData.population * (tileData.dailyControl.v + dailyControl.v) / 100f)));
                tileData.rebelHeldTime = 0;
            }
            else
            {
                tileData.population = Mathf.Clamp((int)(tileData.population * 0.99f),0, tileData.maxPopulation);
                tileData.avaliablePopulation = Mathf.Clamp((int)(tileData.avaliablePopulation * 0.99f), 0, tileData.avaliableMaxPopulation);
                if(tileData.occupiedByID == -1)
                {
                    tileData.rebelHeldTime++;
                    if(tileData.rebelHeldTime >= 30 && atWarWith.Count == 0)
                    {
                        tileData.BreakToRebels();
                    }
                }
                else
                {
                    tileData.rebelHeldTime = 0;
                    tileData.heldByID = -1;
                    tileData.heldByType =-1;
                }
            }
            tileData.localUnrest.UpdateModifier("Control", tileData.control > 50 ? (tileData.control - 50f) * -0.02f : (50f - tileData.control) * 0.04f, EffectType.Flat);
            tileData.localUnrest.UpdateModifier("Seperatism", tileData.seperatism/24f, EffectType.Flat);
            tileData.UpdateUnrestModifiers();
            if(tileData.buildTimer > 0 && tileData.buildQueue.Count > 0 && !tileData.occupied && !tileData.underSiege)
            {
                tileData.buildTimer--;
                if(tileData.buildTimer == 0)
                {
                    tileData.Build(tileData.buildQueue.First());
                    tileData.buildQueue.RemoveAt(0);
                    if(tileData.buildQueue.Count > 0)
                    {
                        Building building = Map.main.Buildings[tileData.buildQueue.First()];
                        tileData.buildTimer = (int)building.GetTime(tileData, this);
                    }
                }
            }
        }
        religiousUnity = Mathf.Clamp((float)religiousUnity / (float)totalDev,0f,1f);
        stabilityCost.UpdateModifier("Religious Unity", (1f - religiousUnity), EffectType.Flat);
        globalUnrest.UpdateModifier("Religious Unity", (1f - religiousUnity) * 3f, EffectType.Flat);
        monthlyPrestige.UpdateModifier("Religious Unity", religiousUnity >= 1f ? 1f : 0f, EffectType.Flat);
        stabilityCost.UpdateModifier("Overextension", (overextension * 0.5f) / 100f, EffectType.Flat);
        dailyControl.UpdateModifier("Overextension", (-overextension * 0.05f) / 100f, EffectType.Flat);
        improveRelations.UpdateModifier("Overextension", (-overextension * 0.5f) / 100f, EffectType.Flat);
        globalUnrest.UpdateModifier("Overextension", (overextension * 5f) / 100f, EffectType.Flat);
        if (ruler.active)
        {
            adminPower = Mathf.Min(999,adminPower + 3 + ruler.adminSkill + (advisorA.active ? advisorA.skillLevel : 0) + (focus > -1 ? ( focus == 0 ? 2: -1) : 0));
            diploPower = Mathf.Min(999,diploPower + 3 + ruler.diploSkill + (advisorD.active ? advisorD.skillLevel : 0) + (focus > -1 ? (focus == 1 ? 2 : -1) : 0));
            milPower = Mathf.Min(999,milPower + 3 + ruler.milSkill + (advisorM.active ? advisorM.skillLevel : 0) + (focus > -1 ? (focus == 2 ? 2 : -1) : 0));
        }
        reformProgress += 0.5f * GetAverageControl() / 100f * (1f + reformProgressGrowth.v);
        float income = GetTotalIncome();

        coins += income;
        RefreshForceLimit();
        coins -= AdvisorMaintainance();
        coins -= ArmyMaintainance();
        coins -= FortMaintenance();
        coins -= GetInterestPayment();
        coins -= DiplomaticExpenses();
        int loops = 0;
        while (coins < 0 && loops < 100)
        {
            TakeLoan();
            loops++;
        }        
        if (!advisorA.active)
        {
            RemoveAdvisor(advisorA);
            if (CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.missingAdvisor;
                notification.description = "You can hire an administrative advisor";
                NotificationsUI.AddNotification(notification);
            }
        }
        if (!advisorD.active)
        { 
            RemoveAdvisor(advisorD);
            if (CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.missingAdvisor;
                notification.description = "You can hire a diplomatic advisor";
                NotificationsUI.AddNotification(notification);
            }
        }
        if (!advisorM.active)
        {
            RemoveAdvisor(advisorM);
            if (CivID == Player.myPlayer.myCivID)
            {
                Notification notification = NotificationsUI.main.missingAdvisor;
                notification.description = "You can hire a military advisor";
                NotificationsUI.AddNotification(notification);
            }
        }
        if (!heir.active)
        {
            if (UnityEngine.Random.Range(0f, 100f) < 5f)
            {
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Ruler newHeir = Ruler.NewHeir((int)rulerAdminSkill.v, (int)rulerDiploSkill.v, (int)rulerMilSkill.v, CivID);
                        Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.NewHeir, newHeir.adminSkill, newHeir.diploSkill, newHeir.milSkill);
                    }                  
                }
                else
                {
                    heir = Ruler.NewHeir((int)rulerAdminSkill.v, (int)rulerDiploSkill.v, (int)rulerMilSkill.v, CivID);
                }
            }
            else if (CivID == Player.myPlayer.myCivID)            
            {
                Notification notification = NotificationsUI.main.noHeir;
                notification.description = "If your ruler dies now then your country will descend into madness";
                NotificationsUI.AddNotification(notification);
            }
            if(!heir.active && !ruler.active && !isPlayer)
            {
                List<Civilisation> possibleOverlords = new List<Civilisation>();
                if (subjects.Count > 0)
                {
                    foreach(var subject in subjects.ToList())
                    {
                        Civilisation subCiv = Game.main.civs[subject];
                        subCiv.RemoveSubjugation();
                        if (!possibleOverlords.Contains(subCiv) && subCiv.overlordID == -1)
                        {
                            possibleOverlords.Add(subCiv);
                        }
                    }
                }
                if (allies.Count > 0)
                {
                    foreach (var ally in allies.ToList())
                    {
                        Civilisation allyCiv = Game.main.civs[ally];
                        BreakAlliance(ally);
                        if (!possibleOverlords.Contains(allyCiv) && allyCiv.overlordID == -1)
                        {
                            possibleOverlords.Add(allyCiv);
                        }
                    }
                }
                foreach( var neighbour in civNeighbours)
                {
                    Civilisation neighbourCiv = Game.main.civs[neighbour];
                    if (!possibleOverlords.Contains(neighbourCiv) && neighbourCiv.overlordID == -1)
                    {
                        possibleOverlords.Add(neighbourCiv);
                    }
                }
                if (possibleOverlords.Count > 0 && GetWars().Count == 0)
                {
                    possibleOverlords.Sort((x, y) => (y.diplomaticCapacityMax.v - y.diplomaticCapacity).CompareTo(x.diplomaticCapacityMax.v - x.diplomaticCapacity));
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Game.main.multiplayerManager.CivActionRpc(possibleOverlords[0].CivID, MultiplayerManager.CivActions.Subjugate, CivID);
                        }
                    }
                    else
                    {
                        possibleOverlords[0].Subjugate(this);
                    }
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Ruler newHeir = Ruler.NewHeir((int)rulerAdminSkill.v - 1, (int)rulerDiploSkill.v - 1, (int)rulerMilSkill.v - 1, CivID, possibleOverlords[0].CivID);
                            Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.NewHeir, newHeir.adminSkill, newHeir.diploSkill, newHeir.milSkill, possibleOverlords[0].CivID);
                        }
                    }
                    else
                    {
                        heir = Ruler.NewHeir((int)rulerAdminSkill.v - 1, (int)rulerDiploSkill.v - 1, (int)rulerMilSkill.v - 1, CivID, possibleOverlords[0].CivID);
                    }                    
                }
                else
                {
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Ruler newHeir = Ruler.NewHeir(-6, -6, -6, CivID);
                            Game.main.multiplayerManager.CivExtraActionRpc(CivID, MultiplayerManager.CivExtraActions.NewHeir, newHeir.adminSkill, newHeir.diploSkill, newHeir.milSkill);
                        }
                    }
                    else
                    {
                        heir = Ruler.NewHeir(-6, -6, -6, CivID);
                    }                    
                }
            }
        }
        else if (!ruler.active)
        {
            
            ruler = new Ruler(heir);
            AddRulerTraits();
            SetAILevels();
            AddStability(-1);
            if (religion == 2 && religiousPoints > -1)
            {
                Effect old = Map.main.religions[2].religiousMechanicEffects[(int)religiousPoints];
                RemoveCivModifier(old.name, "Deity");
                religiousPoints = -1;
            }
            heir.Kill();           
        }
        if (CivID == Player.myPlayer.myCivID)
        {
            if(adminPower > 900 || diploPower > 900 || milPower > 900)
            {
                Notification notification = NotificationsUI.main.fullMonarchPoints;
                notification.description = "You cannot store more than 999 of any monarch point";
                NotificationsUI.AddNotification(notification);
            }
            if (stability < 0)
            {
                Notification notification = NotificationsUI.main.lowStability;
                notification.description = "You have low stability";
                NotificationsUI.AddNotification(notification);
            }
            if (loans.Count > 0)
            {
                Notification notification = NotificationsUI.main.loans;
                notification.description = "You have " + loans.Count + " loans.";
                NotificationsUI.AddNotification(notification);
            }
            if (moraleMax.ms.Exists(i => i.n == "Bankruptcy"))
            {
                Notification notification = NotificationsUI.main.bankruptcy;
                notification.description = "You are Bankrupt";
                NotificationsUI.AddNotification(notification);
            }
            if (reformProgress >= reforms.Count * 40 + 40)
            {
                Notification notification = NotificationsUI.main.canTakeReform;
                notification.description = "You can take a new reform";
                NotificationsUI.AddNotification(notification);
            }
            if (Map.main.TechA.Length > adminTech + 1)
            {
                Tech tech = Map.main.TechA[adminTech + 1];
                int cost = TechnologyUI.GetTechCost(tech, this);
                if (adminPower >= cost)
                {
                    Notification notification = NotificationsUI.main.canTakeTech;
                    notification.description = "You can take the next administrative technology";
                    NotificationsUI.AddNotification(notification);
                }
            }
            if (Map.main.TechD.Length > diploTech + 1)
            {
                Tech tech = Map.main.TechD[diploTech + 1];
                int cost = TechnologyUI.GetTechCost(tech, this);
                if (diploPower >= cost)
                {
                    Notification notification = NotificationsUI.main.canTakeTech;
                    notification.description = "You can take the next diplomatic technology";
                    NotificationsUI.AddNotification(notification);
                }
            }
            if (Map.main.TechM.Length > milTech + 1)
            {
                Tech tech = Map.main.TechM[milTech + 1];
                int cost = TechnologyUI.GetTechCost(tech, this);
                if (milPower >= cost)
                {
                    Notification notification = NotificationsUI.main.canTakeTech;
                    notification.description = "You can take the next military technology";
                    NotificationsUI.AddNotification(notification);
                }
            }
            for(int i = 0; i <  ideaGroups.Length;i++)
            {
                IdeaGroupData idea = ideaGroups[i];
                if (idea != null)
                {
                    if (idea.active && idea.unlockedLevel < 7)
                    {
                        int points = idea.type == 0 ? adminPower : idea.type == 1 ? diploPower : milPower;
                        if (points >= IdeasUI.GetIdeaCost(this))
                        {
                            Notification notification = NotificationsUI.main.canTakeIdea;
                            notification.description = "You can take an idea";
                            NotificationsUI.AddNotification(notification);
                        }
                    }
                    else if(!idea.active && unlockedIdeaGroupSlots >= i)
                    {
                        Notification notification = NotificationsUI.main.canTakeIdea;
                        notification.description = "You can take a new idea group";
                        NotificationsUI.AddNotification(notification);
                    }
                }               
            }
            if (governingCapacity > governingCapacityMax.v)
            {
                float percent = (governingCapacity - governingCapacityMax.v) / governingCapacityMax.v;
                Notification notification = NotificationsUI.main.overGovCap;
                notification.description = "You are " + Mathf.Round(percent * 100f) + "% over your Governing Capacity Maximum";
                NotificationsUI.AddNotification(notification);
            }
            if (diplomaticCapacity > diplomaticCapacityMax.v)
            {
                float percent = (diplomaticCapacity - diplomaticCapacityMax.v) / diplomaticCapacityMax.v;
                Notification notification = NotificationsUI.main.overDipCap;
                notification.description = "You are " + Mathf.Round(percent * 100f) + "% over your Diplomatic Capacity Maximum";
                NotificationsUI.AddNotification(notification);
            }
            for (int i = 0; i < missions.Length; i++)
            {
                if (!missionProgress[i])
                {
                    Mission mission = missions[i];
                    if (mission.CanTake(this))
                    {
                        Notification notification = NotificationsUI.main.canTakeMission;
                        notification.description = "You can Complete a Mission";
                        NotificationsUI.AddNotification(notification);
                        break;
                    }
                }
            }
        }

    }
    public float TotalMilStrength()
    {
        float milStrength = 0f;
        armies.ForEach(i => milStrength+= i.ArmyStrength());
        return milStrength;
    }
    public float TotalMaxArmySize()
    {
        float armySize = 0f;
        armies.ForEach(i => armySize += i.ArmyMaxSize());
        return armySize;
    }
    public float TotalArmySize()
    {
        float armySize = 0f;
        armies.ForEach(i => armySize += i.ArmySize());
        return armySize;
    }
    public void DoAE(PeaceDeal peaceDeal , bool mainTarget = true)
    {
        foreach (var province in peaceDeal.provinces)
        {
            int index = peaceDeal.provinces.IndexOf(province);
            int civToID = peaceDeal.civTo[index];
            Civilisation civTo = Game.main.civs[civToID];
            TileData tile = Map.main.GetTile(province);
            ApplyAE(tile, civTo, GetBaseAE(tile, civTo, 0.6f, mainTarget ? 1f : 1.5f),peaceDeal.fullAnnexation);
        }
        if (peaceDeal.subjugation)
        {
            List<Vector3Int> remaining = GetAllCivTiles().FindAll(i => !peaceDeal.provinces.Contains(i));
            foreach (var province in remaining)
            {
                TileData tile = Map.main.GetTile(province);
                ApplyAE(tile, peaceDeal.taker, GetBaseAE(tile, peaceDeal.taker, 0.5f, mainTarget ? 1f : 1.5f));
            }
        }
    }
    public float GetBaseAE(TileData province, Civilisation target, float peaceTermModifier , float nonCBModifier)
    {
        float ae = 0f;
        ae += Mathf.Min(30, province.totalDev) * peaceTermModifier;
        ae *= (1f + target.aggressiveExpansionImpact.v);
        ae *= nonCBModifier;
        return ae;
    }
    public void ApplyAE(TileData province,Civilisation target,float ae,bool fullAnnex = false)
    {
        foreach(var civ in Game.main.civs)
        {
            if(!civ.isActive() || (civ == target && fullAnnex)) { continue; }
            float religionMod = civ.religion == province.religion ? 0.5f : -0.5f;
            float infidelMod = civ.religion == province.civ.religion && target.religion != civ.religion ? 0.5f : 0;
            float distMod = 1f / (1f + MinimumDistBetween(civ, province.pos) / 2f);
            float allyMod = civ.allies.Contains(target.CivID)? 2f/3f : 1f;
            float subjectMod = target.subjects.Contains(civ.CivID) ? 0.1f : 1f;
            float realAE = ae * (1f + religionMod + infidelMod + distMod) * allyMod * subjectMod;
            civ.opinionOfThem[target.CivID].IncreaseModifier("Aggressive Expansion", -realAE, EffectType.Flat,true);
            float civae = civ.opinionOfThem[target.CivID].ms.Find(i => i.n == "Aggressive Expansion").v;
            if (civae < maxAE)
            {
                maxAE = civae;
            }
        }
    }
    public void AcceptPeaceDeal(PeaceDeal peaceDeal,bool mainTarget = true)
    {
        int winnerID = peaceDeal.taker.CivID;
        Civilisation winnerCiv = Game.main.civs[winnerID];
        winnerCiv.truces[CivID] = (int)(peaceDeal.warScore * 75f + 1000f);
        truces[winnerID] = (int)(peaceDeal.warScore * 75f + 1000f);
        if(peaceDeal.numLoans > 0)
        {          
            winnerCiv.coins += GetLoanSize() * peaceDeal.numLoans;
            coins -= GetLoanSize() * peaceDeal.numLoans;
           
        }
        List<TileData> peaceTiles = peaceDeal.provinces.ConvertAll(i => Map.main.GetTile(i));
        foreach (var tile in peaceTiles)
        {
            int index = peaceDeal.provinces.IndexOf(tile.pos);
            int civToID = peaceDeal.civTo[index];
            Civilisation civFrom = Game.main.civs[tile.civID];
            Civilisation civTo = Game.main.civs[civToID];
            civTo.AddPrestige(0.25f * tile.totalDev);
            AddPrestige(-0.25f * tile.totalDev);
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.Occupy, civToID);
                }
            }
            else
            {
                tile.TransferOccupation(civToID);
            }
            revanchism = Mathf.Clamp(revanchism + tile.GetWarScore(civToID)/100f, 0f, 1f);
            if(tile.greatProject != null)
            {
                GreatProject gp = tile.greatProject;
                if (gp.tier > 0)
                {
                    gp.RemoveProject(this);
                    tile.greatProject.tier--;
                    if(gp.tier > 0)
                    {
                        if (gp.CanUse(civTo))
                        {
                            gp.AddProject(civTo);
                        }
                    }
                }
            }
            if (!tile.hasCore)
            {
                tile.seperatism += (int)(30f * civTo.monthsOfSeperatism.v);
                tile.control = Mathf.Min(25f,tile.control);
            }
            else
            {
                tile.control = Mathf.Max(100f, tile.control + 50f);
            }
            if (civFrom.capitalPos == tile.pos)
            {
                tile.fortLevel = Mathf.Max(tile.fortLevel-1,0);
                civFrom.NewCapital(peaceDeal.provinces);
            }
            civTo.updateBorders = true;
            civFrom.updateBorders = true;
            opinionOfThem[winnerID].AddModifier(new Modifier(peaceDeal.provinces.Count * -5, 1, "Took our Land", decay: true));
        }
        if(peaceDeal.fullAnnexation && subjects.Count > 0)
        {
            foreach(var subject in subjects.ConvertAll(i => Game.main.civs[i]))
            {
                subject.overlordID = -1;
                if (Game.main.isMultiplayer)
                {
                    if (NetworkManager.Singleton.IsServer)
                    {
                        Game.main.multiplayerManager.CivActionRpc(winnerCiv.CivID, MultiplayerManager.CivActions.Subjugate, subject.CivID);
                    }
                }
                else
                {
                    winnerCiv.Subjugate(subject);
                }
            }
            subjects.Clear();
        }
        if (peaceDeal.subjugation)
        {
            if (Game.main.isMultiplayer)
            {
                if (NetworkManager.Singleton.IsServer)
                {
                    Game.main.multiplayerManager.CivActionRpc(winnerCiv.CivID, MultiplayerManager.CivActions.Subjugate, CivID);
                }
            }
            else
            {
                winnerCiv.Subjugate(this);
            }
            winnerCiv.AddPrestige(0.2f * GetTotalDev());
            opinionOfThem[winnerID].AddModifier(new Modifier(-100, 1, "Forced Subject", decay: true));
        }
        opinionOfThem[winnerID].AddModifier(new Modifier(-20,1,"Was at war",decay:true));
        DoAE(peaceDeal, mainTarget);     
    }
    public void MoveCapitalToSaveGame(Vector3Int pos)
    {
        if(Map.main.GetTile(pos) == null) { return; }
        capitalPos = pos;
        Map.main.GetTile(capitalPos).fortLevel++;
        if (capitalIndicator != null)
        {
            capitalIndicator.transform.position = Map.main.GetTile(pos).worldPos();
        }
        else
        {
            capitalIndicator = GameObject.Instantiate(Map.main.capitalIndicatorPrefab, Map.main.GetTile(pos).worldPos(), Quaternion.identity, Map.main.capitalTransform);
        }
    }
    public void NewCapital(List<Vector3Int> notAllowed)
    {
        if(notAllowed.Count == GetAllCivTiles().Count) { return; }
        List<Vector3Int> possible = GetAllCivTiles();
        possible.RemoveAll(i => notAllowed.Contains(i));
        if(possible.Count == 0) { return; }
        Vector3Int best = possible[0];
        float score = -1000;
        foreach(var pos in possible)
        {
            float tileScore = 0;
            TileData tile = Map.main.GetTile(pos);
            tileScore += tile.totalDev;
            tileScore += tile.hasFort ? tile.fortLevel * 5 : 0;
            if(tileScore > score)
            {
                best = pos;
                score = tileScore;
            }
        }
        capitalPos = best;
        Map.main.GetTile(capitalPos).fortLevel++;
        if (capitalIndicator != null)
        {
            capitalIndicator.transform.position = Map.main.GetTile(best).worldPos();
        }
        else
        {
            capitalIndicator = GameObject.Instantiate(Map.main.capitalIndicatorPrefab, Map.main.GetTile(best).worldPos(), Quaternion.identity, Map.main.capitalTransform);
        }
    }
    public List<War> GetWars()
    {
        List<War> results = new List<War>();
        results = Game.main.ongoingWars.FindAll(i => i.Involving(CivID));
        return results;
    }
    public bool isActive()
    {
        if(capitalPos == Vector3Int.zero) { return false; }
        return Map.main.GetTile(capitalPos).civID == CivID;
    }

    public List<Vector3Int> GetAllCivTiles()
    {
        return civTiles.ToList();
    }
    public List<TileData> GetAllCivTileDatas()
    {
        List<TileData> data = civTileDatas.ToList();
        data.RemoveAll(i => i.civID != CivID);
        return data;
    }
    public float GetTotalWarScore(int forCivID)
    {
        float totalWS = 0;
        var tiles = GetAllCivTileDatas();
        foreach (var prov in tiles)
        {
            totalWS += prov.GetWarScore(forCivID);
        }
        return totalWS;
    }
    public int GetTotalDev()
    {
        int totalDev = 0;
        var tiles = GetAllCivTileDatas();
        foreach(var prov in tiles)
        {
            totalDev += prov.totalDev;
        }
        return totalDev;
    }
    public void SetRival(int rivalid)
    {
        for (int i = 0; i < 3; i++)
        {
            if (rivals[i] == -1)
            {
                rivals[i] = rivalid;
                opinionOfThem[rivals[i]].AddModifier(new Modifier(-100, ModifierType.Flat, "Rival"));
                Game.main.civs[rivals[i]].opinionOfThem[CivID].AddModifier(new Modifier(-50, ModifierType.Flat, "Rivals Us"));
                return;
            }
        }
    }
    public void GameStart()
    {
        if(Player.myPlayer.myCivID == CivID) { isPlayer = true; }
        if (!isPlayer && rivals.Contains(-1))
        {
            List<int> possible = GetPossibleRivals();
            possible.Sort((x, y) => TileData.evenr_distance(capitalPos, Game.main.civs[x].capitalPos).CompareTo(TileData.evenr_distance(capitalPos, Game.main.civs[y].capitalPos)));
            for (int i = 0; i < 3; i++)
            {
                if (possible.Count > 0)
                {
                    if (Game.main.isMultiplayer)
                    {
                        if (NetworkManager.Singleton.IsServer)
                        {
                            Game.main.multiplayerManager.CivActionRpc(CivID, MultiplayerManager.CivActions.SetRival, possible[0]);
                            possible.RemoveAt(0);
                        }
                    }
                    else
                    {
                        SetRival(possible[0]);
                        possible.RemoveAt(0);
                    }
                }
            }
        }
    }
    public float GetSubjectIncome()
    {
        float income = 0;
        if(subjects.Count > 0)
        {
            foreach(var subject in subjects)
            {
                Civilisation sub = Game.main.civs[subject];
                SubjectType type = sub.subjectType > -1 ? Map.main.subjectTypes[sub.subjectType] : Map.main.subjectTypes[0];
                if (sub.libertyDesire < 50f)
                {
                    income += sub.GetTotalIncome() * (type.IncomePercentage + incomeFromSubjects.v);
                }
            }
        }
        return income;
    }
    public void Subjugate(Civilisation target)
    {
        if (target == null || target == this || (target.overlordID != CivID && target.overlordID > -1)) { return; }
        subjects.Add(target.CivID);
        opinionOfThem[target.CivID].AddModifier(new Modifier(50, 1, "Subject"));
        target.opinionOfThem[CivID].AddModifier(new Modifier(50, 1, "Subject"));
        target.overlordID = CivID;
        if (Game.main.Started)
        {
            if (isMarshLeader)
            {
                target.subjectType = 4;
            }
            else if (government != 3)
            {
                target.subjectType = 0;
            }
            else
            {
                target.subjectType = 3;
            }
        }
        
        SubjectType type = target.subjectType > -1 ? Map.main.subjectTypes[target.subjectType] : Map.main.subjectTypes[0];
        if(type.SubjectEffects.Length > 0)
        {
            foreach(var effect in type.SubjectEffects)
            {
                target.ApplyCivModifier(effect.name, effect.amount, type.SubjectTypeName, effect.type);
            }
        }
        if (type.OverlordEffects.Length > 0)
        {
            foreach (var effect in type.OverlordEffects)
            {
                ApplyCivModifier(effect.name, effect.amount, target.civName, effect.type);
            }
        }
        target.ApplyCivModifier("Force Limit", -3f, "Subject", EffectType.Base);
        target.ApplyCivModifier("Tax Income", -0.5f, "Subject", EffectType.Flat);
        target.ApplyCivModifier("Development Cost", 0.5f, "Subject", EffectType.Flat);
        target.allies.ToList().ForEach(i => target.BreakAlliance(i));
        target.subjects.ToList().ForEach(i => Game.main.civs[i].RemoveSubjugation());
        target.rivals = new int[3] {-1,-1,-1};
        UpdateDiplomaticCapacity();
        target.UpdateDiplomaticCapacity();
        List<War> wars = target.GetWars();
        foreach (War war in wars)
        {
            if(war.attackerCiv == target || war.defenderCiv == target)
            {
                war.TakeOverLeadership(this, war.attackerCiv == target);
            }
            else
            {
                war.LeaveWar(target.CivID);
            }
        }
    }
    public void RemoveSubjugation()
    {
        if(overlordID == -1) { return; }
        Civilisation target = Game.main.civs[overlordID];
        overlordID = -1;
        opinionOfThem[target.CivID].TryRemoveModifier("Subject");
        target.opinionOfThem[CivID].TryRemoveModifier("Subject");
        SubjectType type = subjectType > -1 ? Map.main.subjectTypes[subjectType] : Map.main.subjectTypes[0];
        if (type.SubjectEffects.Length > 0)
        {
            foreach (var effect in type.SubjectEffects)
            {
                RemoveCivModifier(effect.name,type.SubjectTypeName);
            }
        }
        if (type.OverlordEffects.Length > 0)
        {
            foreach (var effect in type.OverlordEffects)
            {
                target.RemoveCivModifier(effect.name, civName);
            }
        }
        RemoveCivModifier("Force Limit", "Subject");
        RemoveCivModifier("Tax Income", "Subject");
        RemoveCivModifier("Development Cost", "Subject");
        target.subjects.Remove(CivID);
    }
    public War DeclareWar(int targetID,Vector3Int warGoal,CasusBelli casusBelli)
    {
        if(Game.main.gameTime.totalTicks() < 6 * 24 * 7) { return null; }
        if(atWarWith.Contains(targetID) || targetID == -1 || targetID == CivID || truces[targetID] > 0 || allies.Contains(targetID) || militaryAccess.Contains(targetID)) { return null; }
        Civilisation target = Game.main.civs[targetID];
        bool independence = targetID == overlordID && libertyDesire >= 50f;
        if(overlordID > -1 && !independence) { return null; }
        while (target.overlordID > -1)
        {
            target = Game.main.civs[target.overlordID];
        }
        if (atWarWith.Contains(target.CivID) || 
                target.CivID == CivID ||
                truces[target.CivID] > 0) 
        { return null; }
        if (religion == 1)
        {
            religiousPoints = Mathf.Clamp(religiousPoints + 10f, 0, 100);
            Effect effect = Map.main.religions[1].religiousMechanicEffects[0];
            GetStat(effect.name).UpdateModifier("Djinn Unity", effect.amount * (religiousPoints / 100f), effect.type);
        }
        List<Civilisation> allyCivs = allies.ConvertAll(i => Game.main.civs[i]);
        List<Civilisation> enemyAllyCivs = target.allies.ConvertAll(i => Game.main.civs[i]);
       
        atWarWith.Add(target.CivID);
        target.atWarWith.Add(CivID);
        War war = new War(this, target);
        if (subjects.Count > 0)
        {
            foreach (var subject in subjects)
            {
                war.JoinWar(Game.main.civs[subject], true);
            }
        }
        if (target.subjects.Count > 0)
        {

            foreach (var subject in target.subjects.ToList())
            {
                Civilisation sub = Game.main.civs[subject];
                if (subject == CivID || allies.Contains(subject))
                {
                    if (sub.overlordID > -1 && sub.libertyDesire >= 50f)
                    {
                        if (Game.main.isMultiplayer)
                        {
                            if (NetworkManager.Singleton.IsServer)
                            {
                                Game.main.multiplayerManager.CivActionRpc(subject, MultiplayerManager.CivActions.RemoveSubjugation, -1);
                            }
                        }
                        sub.RemoveSubjugation();
                        war.JoinWar(sub, true);
                    }
                }
                else
                {
                    war.JoinWar(sub, false);
                }
            }
        }
        war.warGoal = warGoal;
        war.casusBelli = casusBelli;
        foreach (var civ in enemyAllyCivs.ToList())
        {
            if(civ.overlordID > -1) { continue; }
            if (civ.CallToArms(this, target, true))
            {
                if (!civ.isPlayer)
                {
                    war.JoinWar(civ, false);
                }
                else if (Player.myPlayer.myCivID == civ.CivID)
                {
                    SendPlayerCallToArms(war, false);
                }
            }
            else
            {
                civ.BreakAlliance(target.CivID);
            }
            
        }
        foreach (var civ in allyCivs.ToList())
        {           
            if (civ.CallToArms(target, this, false))
            {
                if (!civ.isPlayer)
                {
                    war.JoinWar(civ, true);
                }
                else if (Player.myPlayer.myCivID == civ.CivID)
                {
                    SendPlayerCallToArms(war, true);
                }
            }           
        }
        return war;
    }
    void SendPlayerCallToArms(War war, bool asAttackerAlly)
    {
        PlayerCTAUI playerCTAUI = GameObject.Instantiate(UIManager.main.playerCTAPrefab,UIManager.main.transform).GetComponent<PlayerCTAUI>();
        playerCTAUI.war = war;
        playerCTAUI.joinAsAttackerAlly = asAttackerAlly;
        playerCTAUI.SetupDescription();
        Game.main.paused = true;
    }
    public bool CallToArms(Civilisation target, Civilisation ally, bool defensive = false)
    {
        float choice = DeclareWarPanelUI.CallToArms(target, ally, this, defensive);
        if(ally.CivID == Player.myPlayer.myCivID || target.CivID == Player.myPlayer.myCivID)
        {
        }
        return choice > 0;
    }
    public void BreakAlliance(int targetID)
    {
        if (!allies.Contains(targetID) || targetID == -1 || targetID == CivID) { return; }
        Civilisation target = Game.main.civs[targetID];
        allies.Remove(targetID);
        Game.main.civs[targetID].allies.Remove(CivID);
        opinionOfThem[targetID].TryRemoveModifier("Ally");
        Game.main.civs[targetID].opinionOfThem[CivID].TryRemoveModifier("Ally");        
        Game.main.civs[targetID].opinionOfThem[CivID].AddModifier(new Modifier(-50, ModifierType.Flat, "Broke Alliance", decay:true));
        truces[targetID] = 6 * 24 * 30;
    }
    public void OfferAlliance(int targetID)
    {
        if (allies.Contains(targetID) || targetID == -1 || targetID == CivID || truces[targetID] > 0 || avaliableDiplomats <= 0) { return; }
        Civilisation target = Game.main.civs[targetID];
        if (target.AllianceOffer(this))
        {
            allies.Add(targetID);
            Game.main.civs[targetID].allies.Add(CivID);
            opinionOfThem[targetID].AddModifier(new Modifier(50, ModifierType.Flat, "Ally"));
            Game.main.civs[targetID].opinionOfThem[CivID].AddModifier(new Modifier(50, ModifierType.Flat, "Ally"));
            UpdateDiplomaticCapacity();
            target.UpdateDiplomaticCapacity();
            deployedDiplomats.Add(new DiplomatStatus(target, this));
        }
    }
    public bool AllianceOffer(Civilisation fromCiv)
    {
        float choice = 0.25f * opinionOfThem[fromCiv.CivID].v;
        choice += 5f * fromCiv.diploRep.v;
        choice += Mathf.Clamp(50f * ((1f + fromCiv.TotalMilStrength()) / (1f + TotalMilStrength()) - 1f), -20f, 20f);
        choice -= Mathf.Max(0,MinimumDistTo(fromCiv) - 10);
        choice += fromCiv.atWarWith.Count > 0 ? -1000 : 0;
        choice += (fromCiv.overlordID == CivID) ? -1000 : 0;
        choice += (overlordID == fromCiv.CivID) ? -1000 : 0;
        choice += (overlordID > -1 && libertyDesire < 50f) ? -1000 : 0;
        choice += ((diplomaticCapacity + 25 + fromCiv.governingCapacity * 0.5f) > diplomaticCapacityMax.v? ((diplomaticCapacity +25 + fromCiv.governingCapacity *0.5f - diplomaticCapacityMax.v)/ diplomaticCapacityMax.v) * -100f : 0f);
        return choice > 0;
    }
    public void RemoveAccess(int targetID)
    {
        if (!militaryAccess.Contains(targetID) || targetID == -1 || targetID == CivID || avaliableDiplomats <= 0) { return; }
        Civilisation target = Game.main.civs[targetID];
        militaryAccess.Remove(targetID);
        UpdateDiplomaticCapacity();
        deployedDiplomats.Add(new DiplomatStatus(target, this));
    }
    public void AccessRequest(int targetID)
    {
        if (militaryAccess.Contains(targetID) || targetID == -1 || targetID == CivID || atWarWith.Contains(targetID) || avaliableDiplomats <= 0) { return; }
        Civilisation target = Game.main.civs[targetID];
        if (target.AccessOffer(this))
        {
            militaryAccess.Add(targetID);
            UpdateDiplomaticCapacity();
            deployedDiplomats.Add(new DiplomatStatus(target, this));
        }
    }
    public bool AccessOffer(Civilisation fromCiv)
    {
        float choice = 0.2f * opinionOfThem[fromCiv.CivID].v;
        choice += 3f * fromCiv.diploRep.v;
        choice += Mathf.Clamp(50f * ((1f+fromCiv.TotalMilStrength()) / (1f+TotalMilStrength()) -1f),-100f,100f);
        choice += fromCiv.allies.Contains(CivID) ? 50f : 0f;
        choice += fromCiv.atWarWith.Exists(i=>rivals.Contains(i)) ? 50f : 0f;
        return choice > 0;
    }

    public Vector3Int SafeProvince()
    {
        Vector3Int retreat = capitalPos;
        List<Vector3Int> possible = GetAllCivTiles();
        List<Vector3Int> enemyPos = new List<Vector3Int>();
        for (int i = 0; i < atWarWith.Count; i++)
        {
            Civilisation civ2 = Game.main.civs[atWarWith[i]];
            //possible.AddRange(civ2.GetAllCivTiles());
            for (int j = 0; j < civ2.armies.Count; j++)
            {
                Vector3Int pos = civ2.armies[j].pos;
                if (!enemyPos.Contains(pos))
                {
                    enemyPos.Add(pos);
                }
            }
        }
        List<float> score = new List<float>();
        foreach (var tile in possible)
        {
            TileData tileData = Map.main.GetTile(tile);
            int dist = TileData.evenr_distance(capitalPos, tile);
            float tileScore = dist < 10 ? dist * 10 : 50;
            if (tileData.civID == CivID)
            {
                foreach (var enemy in enemyPos)
                {
                    dist = TileData.evenr_distance(enemy, tile);
                    if (dist < 2)
                    {
                        tileScore += -100;
                    }
                    else
                    {
                        tileScore += Mathf.Pow(dist, 2);
                    }
                }
                if (tileData.occupied || tileData.underSiege)
                {
                    score.Add(-1000);
                    continue;
                }

                tileScore += tileData.hasZOC ? 2 : 0;
                tileScore += tileData.hasFort ? 5 : 0;
                tileScore += tileData.unitQueue.Count * -10f;
            }
            else
            {
                tileScore -= 100;
            }
            tileScore += tile == capitalPos ? 10 : 0;
            tileScore += tileData.totalDev;
            score.Add(tileScore);
        }
        float bestScoreSoFar = -1000;
        for (int i = 0; i < score.Count; i++)
        {
            if (score[i] > bestScoreSoFar)
            {
                retreat = possible[i];
                bestScoreSoFar = score[i];
            }
        }
        return retreat;
    }
    public void SendEvent(EventData eventData)
    {
        if (isPlayer)
        {
            if (Player.myPlayer.myCivID == CivID)
            {
                if (eventData.affectsRandomProvince)
                {
                    List<TileData> tiles = GetAllCivTileDatas();
                    eventData.province = tiles[UnityEngine.Random.Range(0, tiles.Count)];
                }
                else if (eventData.affectsCapital)
                {
                    eventData.province = Map.main.GetTile(capitalPos);
                }
                GameObject obj = GameObject.Instantiate(UIManager.main.eventPrefab, UIManager.main.eventTransform);
                obj.GetComponent<EventManager>().eventData = eventData;
                Game.main.paused = true;
            }
        }
        else
        {
            EventManager.TakeOption(eventData, eventData.optionEffects[0], this);
        }
    }
    public Stat GetStat(string name)
    {
        Stat stat = null;
        if (stats.TryGetValue(name,out stat)) {
            return stat;
        }
        else
        {
            Debug.Log("Stat " + name + " Not Found");
            return null;
        }       
    }
    public void ChangeReligion(int newReligion,bool costStab = true)
    {
        Religion r = Map.main.religions[religion];
        if (r != null)
        {
            for (int i = 0; i < r.effects.Length; i++)
            {
                RemoveCivModifier(r.effects[i].name, r.name);
            }
            if (religion == 2)
            {
                religiousPoints = -1;
            }
        }
        if (religion == 0)
        {
            for (int i = 0; i < Map.main.religions[0].religiousMechanicEffects.Length; i++)
            {
                Effect effect = Map.main.religions[0].religiousMechanicEffects[i];
                RemoveCivModifier(effect.name, "Aquatism Fervor");
            }
        }
        else if (religion == 1)
        {
            Effect effect = Map.main.religions[1].religiousMechanicEffects[0];
            RemoveCivModifier(effect.name, "Djinn Unity");
        }
        else if (religion == 2)
        {
            if (religiousPoints > -1)
            {
                Effect old = Map.main.religions[2].religiousMechanicEffects[(int)religiousPoints];
                RemoveCivModifier(old.name, "Deity");
            }
        }
        else if (religion == 3)
        {
            Effect cd = Map.main.religions[3].religiousMechanicEffects[0];
            Effect tv = Map.main.religions[3].religiousMechanicEffects[1];
            Effect gu = Map.main.religions[3].religiousMechanicEffects[2];
            RemoveCivModifier(cd.name, "Hunger");
            RemoveCivModifier(tv.name, "Hunger");
            RemoveCivModifier(gu.name, "Hunger");
        }
        religion = newReligion;
        r = Map.main.religions[religion];
        if (r != null)
        {
            for (int i = 0; i < r.effects.Length; i++)
            {
                ApplyCivModifier(r.effects[i].name, r.effects[i].amount, r.name, r.effects[i].type);
            }
            if (religion == 2)
            {
                religiousPoints = -1;
            }
        }
        if (costStab)
        {
            AddStability(-3);
        }
    }
    public void ApplyCivModifier(string modifierName, float strength,string reason,EffectType modifierType = EffectType.Flat,int time = -1)
    {
        if (modifierType == EffectType.Other)
        {
            ApplyCivBonusNonModifier(modifierName);
        }
        else
        {
            Stat stat = GetStat(modifierName);
            if (stat != null)
            {
                stat.AddModifier(new Modifier(strength, (int)modifierType, reason, time));
            }
        }
    }
    public void RemoveCivModifier(string statname, string modname)
    {
        Stat stat = GetStat(statname);
        if (stat != null)
        {
            if (stat.ms.Exists(item => item.n == modname))
            {
                stat.RemoveModifier(stat.ms.Find(item => item.n == modname));
            }
        }
    }
    public void ClearReforms(bool wasTribe = false)
    {
        if (!wasTribe)
        {
            float amount = 0f;
            for(int i = 0; i < reforms.Count; i++)
            {
                amount += 40 + 40 * i;
            }        
            reformProgress += amount;
        }
        else
        {
            reformProgress = 120;
        }
        reforms = new List<int>();
    }
    public void ApplyCivBonusNonModifier(string modfierName)
    {
        switch (modfierName)
        {
            case "Allows Holy Wars":
                canHolyWar = true;
                break;
            case "Change to Monarchy Rule":              
                GovernmentUI.RemoveGovernment(this, government);
                government = 0;
                GovernmentUI.AddGovernment(this, 0,false);
                AddStability(-3);
                break;
            case "Change to Baron Rule":
                GovernmentUI.RemoveGovernment(this, government);
                government = 1;
                GovernmentUI.AddGovernment(this, 1, false);
                AddStability(-3);
                break;
            case "Change to Military Rule":
                GovernmentUI.RemoveGovernment(this, government);
                government = 2;
                GovernmentUI.AddGovernment(this, 2, false);
                AddStability(-3);
                break;
            case "Change to Crusader":
                GovernmentUI.RemoveGovernment(this, government);
                government = 4;
                GovernmentUI.AddGovernment(this, 4, false);
                AddStability(-3);
                break;
            default:
                break;
        }
    }
}
