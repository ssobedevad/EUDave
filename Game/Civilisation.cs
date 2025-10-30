using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.Tilemaps;

[Serializable]
public class Civilisation
{
    public Color c;
    public string civName;
    public Vector3Int capitalPos;
    public int CivID; 
    public bool isPlayer;
    public GameObject capitalIndicator;
    public List<Vector3Int> visited = new List<Vector3Int>();
    public List<Vector3Int> explorePos = new List<Vector3Int>();
    public LineRenderer border;
    public TextMeshProUGUI countryName;
    public List<Army> armies = new List<Army>();
    public List<Boat> boats = new List<Boat>();
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
    public float religiousPoints;
    public float libertyDesire = 0f;
    public float religiousUnity = 1f;
    public Stat libertyDesireTemp = new Stat(0f, "Liberty Desire Temp");
    public int annexationProgress = 0;
    public bool integrating = false;
    public int focus = -1;
    public int focusCD = 0;
    public float reformProgress;
    public float governingCapacity;
    public float coins;
    public float prestige;
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
    public List<int> unlockedBuildings = new List<int>();
    public NationalIdeas nationalIdeas;
    public IdeaGroupData[] ideaGroups = new IdeaGroupData[8];
    public int totalIdeas = 0;
    public int unlockedIdeaGroupSlots = 0;
    public Dictionary<string,RebelFaction> rebelFactions = new Dictionary<string,RebelFaction>();
    public List<Loan> loans = new List<Loan>();
    public float armyTradition;
    public float overextension;
    public List<string> techUnlocks = new List<string>();
    public List<int> reforms = new List<int>();
    public Stat governingCapacityMax = new Stat(200f, "Governing Capacity",true);
    public Stat attritionForEnemies = new Stat(0f, "Attrition For Enemies", true);
    public Stat landAttrition = new Stat(0f, "Land Attrition");
    public Stat attackerDiceRoll = new Stat(0f, "Attacker Dice Roll", true);
    public Stat defenderDiceRoll = new Stat(0f, "Defender Dice Roll", true);
    public Stat generalMeleeSkill = new Stat(0f, "General Melee Skill", true);
    public Stat generalFlankingSkill = new Stat(0f, "General Flanking Skill", true);
    public Stat generalRangedSkill = new Stat(0f, "General Ranged Skill", true);
    public Stat generalSiegeSkill = new Stat(0f, "General Siege Skill", true);
    public Stat generalManeuverSkill = new Stat(0f, "General Maneuver Skill", true);
    public Stat rulerAdminSkill = new Stat(0f, "Ruler Admin Skill", true);
    public Stat rulerDiploSkill = new Stat(0f, "Ruler Diplo Skill", true);
    public Stat rulerMilSkill = new Stat(0f, "Ruler Mil Skill", true);
    public Stat reformProgressGrowth = new Stat(0f, "Reform Progress Growth");
    public Stat militaryTactics = new Stat(0.5f,"Military Tactics",true);
    public Stat discipline = new Stat(1f, "Discipline");
    public Stat moraleMax = new Stat(0f, "Maximum Morale",true);
    public Stat moraleRecovery = new Stat(0f, "Morale Recovery");
    public Stat reinforceSpeed = new Stat(0f, "Reinforce Speed");
    public List<UnitType> units = new List<UnitType>() { new UnitType("Infantry", 0f, 0f, 0f,1,10f), new UnitType("Cavalry", 0f, 0f, 0f,2,20f), new UnitType("Artillery", 0f, 0f, 0f,2,30f) };
    public Stat combatWidth = new Stat(15f, "Combat Width", true);
    public Stat flankingSlots = new Stat(2f, "Flanking Slots", true);
    public Stat devCostMod = new Stat(0f, "Development Cost Modifier");
    public Stat devCost = new Stat(0f, "Development Cost");
    public Stat fortDefence = new Stat(0f, "Fort Defence");
    public Stat fortMaintenance = new Stat(0f, "Fort Maintenance");
    public Stat siegeAbility = new Stat(0f, "Siege Ability");
    public Stat constructionCost = new Stat(0f, "Construction Cost");
    public Stat constructionTime = new Stat(0f, "Construction Time");
    public Stat populationGrowth = new Stat(0f, "Population Growth");
    public Stat maximumPopulation = new Stat(0f, "Maximum Population");
    public Stat taxEfficiency = new Stat(0f, "Tax Efficiency");
    public Stat taxIncome = new Stat(0f, "Tax Income",true);
    public Stat productionValue = new Stat(0f, "Production Value");
    public Stat productionAmount = new Stat(0f, "Production Amount");
    public Stat dailyControl = new Stat(0f, "Daily Control", true);
    public Stat controlDecay = new Stat(0f, "Control Decay");
    public Stat regimentMaintenanceCost = new Stat(0f, "Regiment Maintainance Cost");
    public Stat regimentCost = new Stat(0f, "Regiment Cost");
    public Stat recruitmentTime = new Stat(0f, "Recruitment Time");
    public Stat movementSpeed = new Stat(0f, "Movement Speed");
    public Stat forceLimit = new Stat(3f, "Land Force Limit");
    public Stat prestigeDecay = new Stat(0.05f, "Prestige Decay");
    public Stat monthlyPrestige = new Stat(0f, "Monthly Prestige", true);
    public Stat armyTraditionDecay = new Stat(0.05f, "Army Tradition Decay");
    public Stat monthlyTradition = new Stat(0f, "Monthly Tradition", true);
    public Stat stabilityCost = new Stat(0f, "Stability Cost");
    public Stat coreCost = new Stat(0f, "Core Creation Cost");
    public Stat conversionCost = new Stat(0f, "Religious Conversion Cost");
    public Stat maximumAdvisors = new Stat(3f, "Maximum Advisors",true);
    public Stat advisorCosts = new Stat(0f, "Advisor Cost");
    public Stat advisorCostsA = new Stat(0f, "Administrative Advisor Cost");
    public Stat advisorCostsD = new Stat(0f, "Diplomatic Advisor Cost");
    public Stat advisorCostsM = new Stat(0f, "Military Advisor Cost");
    public Stat techCosts = new Stat(0f, "Tech Cost");
    public Stat techCostsA = new Stat(0f, "Administrative Tech Cost");
    public Stat techCostsD = new Stat(0f, "Diplomatic Tech Cost");
    public Stat techCostsM = new Stat(0f, "Military Tech Cost");
    public Stat ideaCosts = new Stat(0f, "Idea Cost");
    public Stat globalUnrest = new Stat(0f, "Global Unrest", true);
    public Stat trueFaithTolerance = new Stat(2f, "Tolerance of the True Faith", true);
    public Stat infidelIntolerance = new Stat(2f, "Intolerance of Religious Infidels", true);
    public Stat warScoreCost = new Stat(0f, "War Score Cost");
    public Stat dipAnnexCost = new Stat(0f, "Diplomatic Annexation Cost");
    public Stat libDesireFromDev = new Stat(0f, "Liberty Desire from Subjects Development");
    public Stat battlePrestige = new Stat(0f, "Prestige From Battles");
    public Stat battleTraditon = new Stat(0f, "Army Tradition From Battles");
    public Stat diploRep = new Stat(0f, "Diplomatic Reputation",true);
    public Stat libDesire = new Stat(0f, "Liberty Desire in Subjects");
    public Stat incomeFromSubjects = new Stat(0f, "Income from Subjects");
    public Stat diploRelations = new Stat(4f, "Diplomatic Relations", true);
    public Stat improveRelations = new Stat(0f, "Improve Relations");
    public Stat aggressiveExpansionImpact = new Stat(0f, "Aggressive Expansion Impact");
    public Stat monthsOfSeperatism = new Stat(12f, "Months of Seperatism",true);
    public Stat coringRange = new Stat(0f, "Coring Range",true);
    public Stat interestPerMonth = new Stat(4f, "Interest",true);
    public Stat minControl = new Stat(0f, "Minimum Control", true);
    public Stat tradeValue = new Stat(0f, "Trade Value");
    public Stat tradePenalty = new Stat(0.5f, "Trade Penalty");
    public Stat tradeValPerCiv = new Stat(0.1f, "Trade Value per Civ in Node");
    public bool canHolyWar;
    public List<Vector3Int> civTiles = new List<Vector3Int>();
    public List<Vector3Int> claims = new List<Vector3Int>();
    public List<Vector3Int> cores = new List<Vector3Int>();
    public List<string> tradeRegions = new List<string>();
    public List<TileData> civTileDatas = new List<TileData>();
    public List<TileData> civCoastalTiles = new List<TileData>();
    public List<int> civNeighbours = new List<int>();
    public int[] truces;
    public Stat[] opinionOfThem;
    public bool hasUpdatedStartingResources = false;
    public bool updateBorders;
    public List<WeightedChoice> events = new List<WeightedChoice>();
    public int avaliablePopulation;
    public int remainingDiploRelations => (int)diploRelations.value - allies.Count - subjects.Count - militaryAccess.Count;
    public int GetIntegrationCost(Civilisation overlord)
    {
        int baseCost = 8 * GetTotalDev();
        return (int)(baseCost * (1f + overlord.dipAnnexCost.value));
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
        float ld = 0;
        ld +=  (TotalMilStrength() + 1)/(overlord.TotalMilStrength() + 1) * 75f;
        foreach(var ally in allies)
        {
            ld += (Game.main.civs[ally].TotalMilStrength() + 1)/ (overlord.TotalMilStrength() + 1) * 75f;
        }
        ld += (GetTotalIncome() + 1) / (overlord.GetTotalIncome() + 1) * 75f;
        ld -= 3f * overlord.diploRep.value;
        ld -= 0.1f * opinionOfThem[overlordID].value;
        ld += Mathf.Max(0, diploTech - overlord.diploTech) * 5f;
        ld += GetTotalDev() * 0.25f * (1f + libDesireFromDev.value);
        ld += overlord.libDesire.value * 100f;
        ld += libertyDesireTemp.value;
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
        if (tile.isCoastal && MinimumDistBetween(this, tile.pos) <= coringRange.value)
        {
            civCoastalTiles.Sort((x, y) => TileData.evenr_distance(x.pos, tile.pos).CompareTo(TileData.evenr_distance(y.pos, tile.pos)));
            foreach (var coastal in civCoastalTiles) 
            {
                if (TileData.evenr_distance(coastal.pos, tile.pos) > coringRange.value) { return false; }
                int dist = Pathfinding.CoringDistance(coastal.pos, tile.pos);
                if(dist == -1) { continue; }
                return  dist <= coringRange.value;
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
        float coinsIncome = (1f + taxIncome.value) * (1f + taxEfficiency.value);
        float ForceLimit = 0f;
        foreach (var tileData in GetAllCivTileDatas())
        {
            tileData.SetMaxControl();
            tileData.avaliablePopulation = Mathf.Min(tileData.avaliableMaxPopulation, (int)(tileData.population * tileData.control / 100f), tileData.avaliablePopulation);
            coinsIncome += tileData.GetDailyProductionValue();
            coinsIncome += tileData.GetDailyTax();
            ForceLimit += tileData.GetForceLimit();
            if (tileData.greatProject != null && tileData.greatProject.tier > 0)
            {
                if (tileData.greatProject.CanUse(this))
                {
                    tileData.greatProject.AddProject(this);
                }

            }
        }
        for (int i = 0; i < maximumAdvisors.value; i++)
        {
            RefillAdvisors();
        }
        advisorA = new Advisor();
        advisorD = new Advisor();
        advisorM = new Advisor();
        ideaGroups = new IdeaGroupData[8];
        adminPower += 100 + 16 * ruler.adminSkill;
        diploPower += 100 + 16 * ruler.diploSkill;
        milPower += 100 + 16 * ruler.milSkill;
        coins += coinsIncome * 16;
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
        border = GameObject.Instantiate(Map.main.civBorderPrefab,Map.main.indicatorTransform).GetComponent<LineRenderer>();
        countryName = GameObject.Instantiate(Map.main.civNamePrefab,UIManager.main.worldCanvas).GetComponent<TextMeshProUGUI>();
        SetupBorderLine();
        SetupCountryName();
        AddArmyTradition(ruler.milSkill * 5f + monthlyTradition.value * 6);
    }
    public void SetupBorderLine()
    {
        var perim = PerimeterHelper.GetPerimeter(GetAllCivTiles());

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
        color.r *= 0.6f;
        color.g *= 0.6f;
        color.b *= 0.6f;
        border.startColor = color;
        border.endColor = color;
    }
    public void SetupCountryName()
    {
        List<Vector3Int> tiles = GetAllCivTiles();
        List<Vector3Int> included = new List<Vector3Int>();
        included.Add(capitalPos);
        Queue<Vector3Int> possible = new Queue<Vector3Int>();
        possible.Enqueue(capitalPos);
        while (possible.Count > 0)
        {
            Vector3Int pos = possible.Dequeue();
            List<Vector3Int> neighbors = Map.main.GetTile(pos).GetNeighbors().ToList();
            neighbors.RemoveAll(i => !tiles.Contains(i));
            if (neighbors.Count > 0)
            {
                foreach (var nb in neighbors)
                {
                    if (!included.Contains(nb))
                    {
                        included.Add(nb);                      
                        if (neighbors.Count > 3)
                        {
                            possible.Enqueue(nb);
                        }
                    }
                }
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
        Vector2 centre = Map.main.tileMapManager.tilemap.CellToWorld(capitalPos);
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
        rect.sizeDelta = new Vector2(width,height);
        countryName.text = civName;
        countryName.gameObject.SetActive(true);
    }
    public void Init()
    {
        Game.main.start.AddListener(GameStart);
        Game.main.dayTick.AddListener(DayTick);
        Game.main.tenMinTick.AddListener(TenMinTick);
        Game.main.monthTick.AddListener(MonthTick);
        InitExploration();
        truces = new int[Game.main.civs.Count];
        opinionOfThem = new Stat[Game.main.civs.Count];
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
    }
    public void BuyGeneral()
    {
        int points = UnityEngine.Random.Range(1, 7) + (int)(armyTradition / 10) + (ruler.active? ruler.milSkill/3 : 0);
        General general = new General(Age.zero);
        List<WeightedChoice> choices = new List<WeightedChoice>();
        choices.Add(new WeightedChoice(0, 3));
        choices.Add(new WeightedChoice(1, 3));
        choices.Add(new WeightedChoice(2, 3));
        choices.Add(new WeightedChoice(3, 1));
        choices.Add(new WeightedChoice(4, 1));
        int basePoints = points / 5;
        general.meleeSkill = basePoints + (int)generalMeleeSkill.value;
        general.flankingSkill = basePoints + (int)generalFlankingSkill.value;
        general.rangedSkill = basePoints + (int)generalRangedSkill.value;
        general.siegeSkill = basePoints + (int)generalSiegeSkill.value;
        general.maneuverSkill = basePoints + (int)generalManeuverSkill.value;
        points -= (basePoints * 5);
        for (int i = 0; i < points; i++)
        {
            int choice = WeightedChoiceManager.getChoice(choices).choiceID;
            switch (choice)
            {
                case 0:
                    general.meleeSkill++;
                    break;
                case 1:
                    general.flankingSkill++;
                    break;
                case 2:
                    general.rangedSkill++;
                    break;
                case 3:
                    general.siegeSkill++;
                    break;
                case 4:
                    general.maneuverSkill++;
                    break;
                default:
                    break;
            }
        }
        generals.Add(general);
    }
    public void AddPrestige(float amount)
    {
        prestige = Mathf.Clamp(prestige + amount, -100f, 100f);
        taxEfficiency.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 1);
        moraleMax.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 0);
        populationGrowth.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 1);
        taxEfficiency.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 1);
    }
    public void AddArmyTradition(float amount)
    {
        armyTradition = Mathf.Clamp(armyTradition + amount, 0f, 100f);

        moraleMax.UpdateModifier("Army Tradition", (armyTradition * 0.25f) / 100f, 0);
        siegeAbility.UpdateModifier("Army Tradition", (armyTradition * 0.05f) / 100f, 1);
        moraleRecovery.UpdateModifier("Army Tradition", (armyTradition * 0.1f) / 100f, 1);
        populationGrowth.UpdateModifier("Army Tradition", (armyTradition * 0.1f) / 100f, 1);
    }
    void MonthTick()
    {
        if(civTiles.Count == 0 || civTileDatas.Count == 0) { return; }
        if(events.Count > 0)
        {
            int index = WeightedChoiceManager.getChoice(events).choiceID;
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
                AddPulseEvents();
            }
        }
        generals.RemoveAll(i => !i.active);
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
    public void RemoveAdvisor(Advisor advisor)
    {
        Stat stat = GetStat(advisor.effect);
        if (stat != null)
        {
            Modifier mod = stat.modifiers.Find(i => i.name == "Advisor");
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
        ApplyCivModifier(advisor.effect, advisor.effectStrength, "Advisor",advisor.effectType);
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



        for (int i = 0; i < opinionOfThem.Length;i++)
        {
            Civilisation civ = Game.main.civs[i];
            opinionOfThem[i].decayAmount = (1f + civ.improveRelations.value);
        }


        foreach (var tileData in GetAllCivTileDatas())
        {
            if (tileData.mercenaryTimer > 0 && tileData.mercenaryQueue.Count > 0 && !tileData.occupied && !tileData.underSiege)
            {
                tileData.mercenaryTimer--;
                if (tileData.mercenaryTimer == 0)
                {
                    MercenaryGroup merc = Map.main.mercenaries[tileData.mercenaryQueue[0]];
                    tileData.CreateMercenaryGroup(tileData.mercenaryQueue[0]);
                    tileData.mercenaryQueue.RemoveAt(0);
                    if (tileData.mercenaryQueue.Count > 0)
                    {
                        tileData.mercenaryTimer = tileData.GetRecruitTime();
                    }
                }
            }
            if (avaliablePopulation < 1000) { break; }
            if (tileData.recruitTimer > 0 && tileData.recruitQueue.Count > 0 && !tileData.occupied && !tileData.underSiege)
            {
                tileData.recruitTimer--;
                if (tileData.recruitTimer == 0)
                {
                    tileData.CreateNewArmy(tileData.recruitQueue[0]);
                    tileData.recruitQueue.RemoveAt(0);
                    if (tileData.recruitQueue.Count > 0)
                    {
                        tileData.recruitTimer = tileData.GetRecruitTime();
                    }
                }
            }
            
        }
    }
    public void RefillAdvisors()
    {
        foreach(var advisor in advisorsA.ToList())
        {
            if (!advisor.active)
            {
                advisorsA.Remove(advisor);
            }
        }
        if(advisorsA.Count < maximumAdvisors.value)
        {
            advisorsA.Add(Advisor.NewRandomAdvisor(0, CivID));
        }
        foreach (var advisor in advisorsD.ToList())
        {
            if (!advisor.active)
            {
                advisorsD.Remove(advisor);
            }
        }
        if (advisorsD.Count < maximumAdvisors.value)
        {
            advisorsD.Add(Advisor.NewRandomAdvisor(1, CivID));
        }
        foreach (var advisor in advisorsM.ToList())
        {
            if (!advisor.active)
            {
                advisorsM.Remove(advisor);
            }
        }
        if (advisorsM.Count < maximumAdvisors.value)
        {
            advisorsM.Add(Advisor.NewRandomAdvisor(2, CivID));
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
        return (int)(baseC * (1f + stabilityCost.value));
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
        return (int)(income * 30f / (GetLoanSize() * interestPerMonth.value/100f));
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
        tiles.RemoveAll(i => i.avaliablePopulation == 0 || i.avaliableMaxPopulation == 0);
        if (tiles.Count == 0) { return 0; }
        int amountPerTile = targetAmount / tiles.Count;
        foreach (var tile in tiles)
        {
            int used = Mathf.Min(amountPerTile, tile.avaliablePopulation);
            tile.avaliablePopulation -= used;
            tile.population -= used;
            amount += used;
        }        
        if (amount < targetAmount)
        {            
            tiles.Sort((x, y) => (y.avaliablePopulation / y.avaliableMaxPopulation).CompareTo((x.avaliablePopulation / x.avaliableMaxPopulation)));
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
        foreach(var region in tradeRegions)
        {
            tradeIncome += Mathf.Max(0, Map.main.tradeRegions[region].GetTradeIncome(this) * (region == capitalRegion ? 1f : 1f-tradePenalty.value));
        }
        return tradeIncome * (1f + tradeValue.value);
    }
    public float TaxIncome()
    {
        float coinsIncome = (1f + taxIncome.value) * (1f + taxEfficiency.value);
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
                fortM += Mathf.Max(0,(tileData.fortLevel - (tileData.pos == capitalPos ? 1 : 0)) * (1f + tileData.localFortMaintenance.value));
            }
        }
        return fortM * (1f + fortMaintenance.value);
    }
    public float DiplomaticExpenses()
    {
        float expenses = 0f;
        if (overlordID > -1 && libertyDesire < 50f)
        {
            expenses += Mathf.Max(0, GetTotalIncome() * (0.1f + Game.main.civs[overlordID].incomeFromSubjects.value));
        }
        return expenses;
    }
    public void AddStability(int amount)
    {
        stability = Mathf.Clamp(stability + amount, -3, 3);
        stabilityCost.UpdateModifier("Stability", stability > 0 ? stability * 0.50f : 0f, 1);
        globalUnrest.UpdateModifier("Stability", stability > 0 ? stability * -1f : stability * -2f, 1);
        dailyControl.UpdateModifier("Stability", stability > 0 ? stability * 0.01f : stability * 0.02f, 1);
        taxEfficiency.UpdateModifier("Stability", stability * 0.05f, 1);
        interestPerMonth.UpdateModifier("Stability", stability < 0 ? stability * -1f : 0f, 1);
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
            tileData.recruitQueue.Clear();
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
            DeclareBankruptcy();
        }
    }
    public float ArmyMaintainance()
    {
        float armyCosts = 0f;
        foreach(var army in armies)
        {
            foreach(var regiment in army.regiments)
            {
                float mult = regiment.mercenary ? (0.5f + Game.main.gameTime.years * 0.25f): 1f;
                armyCosts += mult * units[regiment.type].baseCost * (float)regiment.size / (float)regiment.maxSize * 0.25f / 12f;
            }
        }
        if (TotalMaxArmySize() / 1000f > forceLimit.value)
        {
            float increase = (forceLimit.value + (TotalMaxArmySize() / 1000f - forceLimit.value) * 2) / forceLimit.value;
            armyCosts *= increase;
        }
        armyCosts *= (1f + regimentCost.value);
        armyCosts *= (1f + regimentMaintenanceCost.value);
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
        float points = 0.1f + (civTiles.Count * 0.01f) + (remainingDiploRelations < 0 ? remainingDiploRelations * -1f : 0f);
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
    }
    public void Rebirth()
    {
        if (!countryName.gameObject.activeSelf)
        {
            Game.main.dayTick.AddListener(DayTick);
            Game.main.tenMinTick.AddListener(TenMinTick);
            countryName.gameObject.SetActive(true);
            border.gameObject.SetActive(true);
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
        float change = monthlyPrestige.value;
        change += -prestige * prestigeDecay.value;
        return change;
    }
    public float GetMonthlyTraditionChange()
    {
        float change = monthlyTradition.value;
        change += -armyTradition * armyTraditionDecay.value;
        return change;
    }
    public void DayTick()
    {
        for (int i = 0; i < Game.main.civs.Count; i++)
        { 
            Civilisation civ = Game.main.civs[i];
            opinionOfThem[i].UpdateModifier("Religion",civ.religion == religion ? 20 : -20, ModifierType.Flat);            
        }
        if (CivID == Player.myPlayer.myCivID)
        {
            NotificationsUI.main.ClearNotifications();
        }
        if(civTiles.Count == 0)
        {
            Game.main.dayTick.RemoveListener(DayTick);
            Game.main.tenMinTick.RemoveListener(TenMinTick);
            countryName.gameObject.SetActive(false);
            border.gameObject.SetActive(false);
            GameObject.Destroy(capitalIndicator);
            foreach (var army in armies.ToList())
            {
                GameObject.Destroy(army.gameObject);
                armies.Remove(army);
            }
            allies.ToList().ForEach(i => BreakAlliance(i));
            foreach (var war in GetWars())
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
            return;
        }
        foreach(var rival in rivals)
        {
            if(rival > -1)
            {
                Civilisation rivalCiv = Game.main.civs[rival]; 
                opinionOfThem[rival].UpdateModifierDuration("Rival", -100, 1729, ModifierType.Flat);
                rivalCiv.opinionOfThem[CivID].UpdateModifierDuration("Rivals Us", -50, 1729, ModifierType.Flat);
                foreach (var ally in rivalCiv.allies)
                {
                    opinionOfThem[ally].UpdateModifierDuration("Ally of Rival",-25, 1729,1);
                }
                foreach (var riv in rivalCiv.rivals)
                {
                    if (riv > -1)
                    {
                        opinionOfThem[riv].UpdateModifierDuration("Rival of Rival", 20, 1729, 1);
                    }
                }
                foreach (var riv in rivalCiv.atWarWith)
                {
                    opinionOfThem[riv].UpdateModifierDuration("At war with Rival", 10, 1729, 1);
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
                if (overlord.diploPower > 0 && (int)overlord.diploRep.value > -2)
                {
                    annexationProgress += 2 + (int)overlord.diploRep.value;
                    overlord.diploPower -= 2 + (int)overlord.diploRep.value;
                    if(annexationProgress >= GetIntegrationCost(overlord))
                    {
                        foreach(var tileData in GetAllCivTileDatas())
                        {
                            tileData.civID = overlordID;
                            if (!tileData.cores.Contains(overlordID))
                            {
                                tileData.cores.Add(overlordID);
                            }
                            overlord.AddPrestige(0.25f * tileData.totalDev);
                            tileData.buildQueue.Clear();
                            tileData.recruitQueue.Clear();
                            tileData.mercenaryQueue.Clear();
                            tileData.coreTimer = -1;
                            tileData.religionTimer = -1;
                            tileData.buildTimer = -1;
                            tileData.recruitTimer = -1;
                            tileData.mercenaryTimer = -1;
                            tileData.SetMaxControl();
                            tileData.control = Mathf.Min(75f, tileData.control,tileData.maxControl);
                        }
                        overlord.subjects.Remove(CivID);
                        overlordID = -1;
                        updateBorders = true;
                        overlord.updateBorders = true;
                        return;
                    }
                }
            }
        }
        RefillAdvisors();
        overextension = 0f;
        religiousUnity = 0f;
        int totalDev = GetTotalDev();
        foreach(var tileData in GetAllCivTileDatas())
        {
            if(tileData.religion == religion)
            {
                tileData.localUnrest.UpdateModifier("Religion", -trueFaithTolerance.value, 1);
                religiousUnity += tileData.totalDev;
            }
            else
            {
                tileData.localUnrest.UpdateModifier("Religion", infidelIntolerance.value, 1);
            }
            tileData.SetMaxControl();
            if(tileData.greatProject != null)
            {
                GreatProject gp = tileData.greatProject;
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
                else if (gp.tier < 3 && CivID == Player.myPlayer.myCivID && coins >= gp.GetCost(tileData,this) && gp.CanUse(this))
                {
                    Notification notification = NotificationsUI.main.greatProj;
                    notification.description = "You can upgrade a great project";
                    notification.province = tileData;
                    NotificationsUI.AddNotification(notification);
                }
            }
            if (tileData.religionTimer > -1 && !tileData.occupied && tileData.control >= tileData.GetConvertControl())
            {
                tileData.religionTimer--;
                if (tileData.religionTimer == 0)
                {
                    tileData.religion = religion;
                    tileData.religionTimer = -1;
                    tileData.SetMaxControl();
                    if (tileData.localUnrest.modifiers.Exists(i => i.name == "Converting Religion"))
                    {
                        tileData.localUnrest.RemoveModifier(tileData.localUnrest.modifiers.Find(i => i.name == "Converting Religion"));
                    }
                }
            }
            if(tileData.religion != religion && tileData.religionTimer > -1)
            {               
                if (!tileData.localUnrest.modifiers.Exists(i => i.name == "Converting Religion"))
                {
                    tileData.localUnrest.AddModifier(new Modifier(6, ModifierType.Flat, "Converting Religion"));
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
                    if (tileData.localUnrest.modifiers.Exists(i => i.name == "Not a core"))
                    {
                        tileData.localUnrest.RemoveModifier(tileData.localUnrest.modifiers.Find(i => i.name == "Not a core"));
                    }
                }
            }
            if (!tileData.hasCore)
            {                          
                overextension += tileData.totalDev * 0.8f;     
                if(!tileData.localUnrest.modifiers.Exists(i=>i.name == "Not a core"))
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
                tileData.control = Mathf.Clamp(tileData.control + tileData.dailyControl.value + dailyControl.value, 0f, tileData.maxControl);
                tileData.population = Mathf.Max(0,Mathf.Min(tileData.maxPopulation, tileData.population + tileData.populationGrowth));
                tileData.avaliablePopulation = Mathf.Max(0, Mathf.Min(tileData.avaliableMaxPopulation, tileData.population, tileData.avaliablePopulation + (int)(tileData.populationGrowth * tileData.control / 100f + tileData.population * (tileData.dailyControl.value + dailyControl.value) / 100f)));
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
            tileData.localUnrest.UpdateModifier("Control", tileData.control > 50 ? (tileData.control - 50f) * -0.02f : (50f - tileData.control) * 0.04f, 1);
            tileData.localUnrest.UpdateModifier("Seperatism", tileData.seperatism/24f, 1);
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
            if (tileData.unrest > 0)
            {
                if(rebelFactions.ContainsKey(tileData.region))
                {
                    if (!rebelFactions[tileData.region].provinces.Contains(tileData.pos))
                    {
                        rebelFactions[tileData.region].provinces.Add(tileData.pos);
                    }
                }
                else
                {
                    rebelFactions.Add(tileData.region,new RebelFaction(tileData.pos));
                }
            }
        }
        religiousUnity = Mathf.Clamp((float)religiousUnity / (float)totalDev,0f,1f);
        stabilityCost.UpdateModifier("Religious Unity", (1f - religiousUnity), 1);
        globalUnrest.UpdateModifier("Religious Unity", (1f - religiousUnity) * 3f, 1);
        monthlyPrestige.UpdateModifier("Religious Unity", religiousUnity >= 1f ? 1f : 0f, 1);
        stabilityCost.UpdateModifier("Overextension", (overextension * 0.5f) / 100f, 1);
        dailyControl.UpdateModifier("Overextension", (-overextension * 0.05f) / 100f, 1);
        improveRelations.UpdateModifier("Overextension", (-overextension * 0.5f) / 100f, 1);
        globalUnrest.UpdateModifier("Overextension", (overextension * 5f) / 100f, 1);
        foreach (var rebelFaction in rebelFactions.Values.ToList())
        {
            rebelFaction.Update();
        }
        if (ruler.active)
        {
            adminPower = Mathf.Min(999,adminPower + 3 + ruler.adminSkill + (advisorA.active ? advisorA.skillLevel : 0) + (focus > -1 ? ( focus == 0 ? 2: -1) : 0));
            diploPower = Mathf.Min(999,diploPower + 3 + ruler.diploSkill + (advisorD.active ? advisorD.skillLevel : 0) + (focus > -1 ? (focus == 1 ? 2 : -1) : 0));
            milPower = Mathf.Min(999,milPower + 3 + ruler.milSkill + (advisorM.active ? advisorM.skillLevel : 0) + (focus > -1 ? (focus == 2 ? 2 : -1) : 0));
        }
        reformProgress += 0.5f * GetAverageControl() / 100f * (1f + reformProgressGrowth.value);
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
                heir = Ruler.NewHeir((int)rulerAdminSkill.value, (int)rulerDiploSkill.value, (int)rulerMilSkill.value, CivID);
            }
            else if (CivID == Player.myPlayer.myCivID)            
            {
                Notification notification = NotificationsUI.main.noHeir;
                notification.description = "If your ruler dies now then your country will descend into madness";
                NotificationsUI.AddNotification(notification);
            }
        }
        else if (!ruler.active)
        {
            ruler = new Ruler(heir);
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
            if (moraleMax.modifiers.Exists(i => i.name == "Bankruptcy"))
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
                    else if(!idea.active && unlockedIdeaGroupSlots > i)
                    {
                        Notification notification = NotificationsUI.main.canTakeIdea;
                        notification.description = "You can take a new idea group";
                        NotificationsUI.AddNotification(notification);
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
            ApplyAE(tile, civTo, GetBaseAE(tile, civTo, 0.6f, mainTarget ? 1f : 1.5f));
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
        ae *= (1f + target.aggressiveExpansionImpact.value);
        ae *= nonCBModifier;
        return ae;
    }
    public void ApplyAE(TileData province,Civilisation target,float ae)
    {
        foreach(var civ in Game.main.civs)
        {
            if(!civ.isActive() || civ == target) { continue; }
            float religionMod = civ.religion == province.religion ? 0.5f : -0.5f;
            float infidelMod = civ.religion == province.civ.religion && target.religion != civ.religion ? 0.5f : 0;
            float distMod = 1f / (1f + MinimumDistBetween(civ, province.pos) / 2f);
            float allyMod = civ.allies.Contains(target.CivID)? 2f/3f : 1f;
            float subjectMod = target.subjects.Contains(civ.CivID) ? 0.1f : 1f;
            float realAE = ae * (1f + religionMod + infidelMod + distMod) * allyMod * subjectMod;
            civ.opinionOfThem[target.CivID].IncreaseModifier("Aggressive Expansion", -realAE, 1,true);
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
            tile.civID = civToID;
            tile.buildQueue.Clear();
            tile.recruitQueue.Clear();
            tile.mercenaryQueue.Clear();
            tile.coreTimer = -1;
            tile.religionTimer = -1;
            tile.buildTimer = -1;
            tile.recruitTimer = -1;
            tile.mercenaryTimer = -1;
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
                tile.seperatism += (int)(30f * civTo.monthsOfSeperatism.value);
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
            civFrom.opinionOfThem[winnerID].AddModifier(new Modifier(peaceDeal.provinces.Count * -5, 1, "Took our Land", decay: true));
        }
        if(peaceDeal.fullAnnexation && subjects.Count > 0)
        {
            foreach(var subject in subjects.ConvertAll(i => Game.main.civs[i]))
            {
                subject.overlordID = -1;
                winnerCiv.Subjugate(subject);
            }
            subjects.Clear();
        }
        if (peaceDeal.subjugation)
        {
            winnerCiv.Subjugate(this);
            winnerCiv.AddPrestige(0.2f * GetTotalDev());
            opinionOfThem[winnerID].AddModifier(new Modifier(-100, 1, "Forced Subject", decay: true));
        }
        opinionOfThem[winnerID].AddModifier(new Modifier(-20,1,"Was at war",decay:true));
        DoAE(peaceDeal, mainTarget);     
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
            capitalIndicator = GameObject.Instantiate(Map.main.capitalIndicatorPrefab, Map.main.GetTile(best).worldPos(), Quaternion.identity);
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
    public void GameStart()
    {
        if(Player.myPlayer.myCivID == CivID) { isPlayer = true; }
        if (!isPlayer && rivals.Contains(-1))
        {
            List<int> possible = GetPossibleRivals();
            possible.Sort((x, y) => TileData.evenr_distance(capitalPos, Game.main.civs[x].capitalPos).CompareTo(TileData.evenr_distance(capitalPos, Game.main.civs[y].capitalPos)));
            for (int i = 0; i < 3; i++)
            {
                if (rivals[i] == -1 && possible.Count > 0)
                {
                    rivals[i] = possible.First();
                    possible.RemoveAt(0);
                    opinionOfThem[rivals[i]].AddModifier(new Modifier(-100, ModifierType.Flat, "Rival"));
                    Game.main.civs[rivals[i]].opinionOfThem[CivID].AddModifier(new Modifier(-50, ModifierType.Flat, "Rivals Us"));
                }
            }
        }
    }
    public void DiscoverTile(TileData tile)
    {
        DiscoverResource(tile);
        
        if (!isPlayer)
        {
           
        }
        if (Player.myPlayer.myCivID == CivID)
        {
            Map.main.tileMapManager.tilemapUnknown.SetTileFlags(tile.pos, TileFlags.None);
            Map.main.tileMapManager.tilemapUnknown.SetColor(tile.pos, Color.clear);
        }
        if (explorePos.Contains(tile.pos)) { explorePos.Remove(tile.pos); }
        if (!visited.Contains(tile.pos))
        {
            visited.Add(tile.pos);           
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
                if (sub.libertyDesire < 50f)
                {
                    income += sub.GetTotalIncome() * (0.1f + incomeFromSubjects.value);
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
        target.ApplyCivModifier("Force Limit", -3f, "Subject", 3);
        target.ApplyCivModifier("Tax Income", -0.5f, "Subject", 1);
        target.ApplyCivModifier("Development Cost", 0.5f, "Subject", 1);
        target.allies.ToList().ForEach(i => target.BreakAlliance(i));
        target.subjects.ToList().ForEach(i => Game.main.civs[i].RemoveSubjugation());
        target.rivals = new int[3] {-1,-1,-1};
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
        RemoveCivModifier("Force Limit", "Subject");
        RemoveCivModifier("Tax Income", "Subject");
        RemoveCivModifier("Development Cost", "Subject");
        target.subjects.Remove(CivID);
    }
    public void DiscoverResource(TileData tileData)
    {       
    }
    public void DeclareWar(int targetID,Vector3Int warGoal,CasusBelli casusBelli)
    {
        if(Game.main.gameTime.totalTicks() < 6 * 24 * 7) { return; }
        if(atWarWith.Contains(targetID) || targetID == -1 || targetID == CivID || truces[targetID] > 0 || allies.Contains(targetID) || militaryAccess.Contains(targetID)) { return; }
        Civilisation target = Game.main.civs[targetID];
        bool independence = targetID == overlordID;
        if(overlordID > -1 && !independence) { return; }
        while (target.overlordID > -1)
        {
            target = Game.main.civs[target.overlordID];
        }
        if (atWarWith.Contains(target.CivID) || 
                target.CivID == CivID ||
                truces[target.CivID] > 0) 
        { return; }
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
            //Debug.Log(civName + " Choice " + choice);
            //Debug.Log(DeclareWarPanelUI.GetPositiveReasons(target, ally, this, defensive));
            //Debug.Log(DeclareWarPanelUI.GetNegativeReasons(target, ally, this, defensive));
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
        if (allies.Contains(targetID) || targetID == -1 || targetID == CivID || truces[targetID] > 0) { return; }
        Civilisation target = Game.main.civs[targetID];
        if (target.AllianceOffer(this))
        {
            allies.Add(targetID);
            Game.main.civs[targetID].allies.Add(CivID);
            opinionOfThem[targetID].AddModifier(new Modifier(50, ModifierType.Flat, "Ally"));
            Game.main.civs[targetID].opinionOfThem[CivID].AddModifier(new Modifier(50, ModifierType.Flat, "Ally"));
        }
    }
    public bool AllianceOffer(Civilisation fromCiv)
    {
        float choice = 0.25f * opinionOfThem[fromCiv.CivID].value;
        choice += 5f * fromCiv.diploRep.value;
        choice += Mathf.Atan(fromCiv.TotalMilStrength() / 1000f - TotalMilStrength() / 1000f) * 12f;
        choice -= Mathf.Max(0,MinimumDistTo(fromCiv) - 10);
        choice += fromCiv.atWarWith.Count > 0 ? -1000 : 0;
        choice += (fromCiv.overlordID == CivID) ? -1000 : 0;
        choice += (overlordID == fromCiv.CivID) ? -1000 : 0;
        choice += (overlordID > -1 && libertyDesire < 50f) ? -1000 : 0;
        choice += remainingDiploRelations <= 0? (remainingDiploRelations-1) * 20f : 0f;
        //Debug.Log("Choice Alliance Offer " + choice);
        return choice > 0;
    }
    void InitExploration()
    {
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
        if (Player.myPlayer.myCivID == CivID && isPlayer)
        {
            if (eventData.affectsRandomProvince)
            {
                List<TileData> tiles = GetAllCivTileDatas();
                eventData.province = tiles[UnityEngine.Random.Range(0,tiles.Count)];
            }
            else if (eventData.affectsCapital)
            {
                eventData.province = Map.main.GetTile(capitalPos);
            }
            GameObject obj = GameObject.Instantiate(UIManager.main.eventPrefab,UIManager.main.eventTransform);
            obj.GetComponent<EventManager>().eventData = eventData;
            Game.main.paused = true;
        }
        else
        {
            EventManager.TakeOption(eventData, eventData.optionEffects[0], this);
        }
    }
    public Stat GetStat(string name)
    {
        switch (name)
        {
            case "Morale":
                return moraleMax;
            case "Discipline":
                return discipline;               
            case "Morale Recovery":
                return moraleRecovery;
            case "Reinforce Speed":
                return reinforceSpeed;
            case "Tactics":
                return militaryTactics;
            case "Combat Width":
                return combatWidth;
            case "Infantry Melee Damage":
                return units[0].meleeDamage;
            case "Infantry Flanking Damage":
                return units[0].flankingDamage;
            case "Infantry Ranged Damage":
                return units[0].rangedDamage;
            case "Cavalry Melee Damage":
                return units[1].meleeDamage;
            case "Cavalry Flanking Damage":
                return units[1].flankingDamage;
            case "Cavalry Ranged Damage":
                return units[1].rangedDamage;
            case "Artillery Melee Damage":
                return units[2].meleeDamage;
            case "Artillery Flanking Damage":
                return units[2].flankingDamage;
            case "Artillery Ranged Damage":
                return units[2].rangedDamage;
            case "Infantry Combat Ability":
                return units[0].combatAbility;
            case "Cavalry Combat Ability":
                return units[1].combatAbility;
            case "Artillery Combat Ability":
                return units[2].combatAbility;
            case "Development Cost":
                return devCost;
            case "Development Cost Modifier":
                return devCostMod;
            case "Force Limit":
                return forceLimit;
            case "Fort Defence":
                return fortDefence;
            case "Fort Maintenance":
                return fortMaintenance;
            case "Siege Ability":
                return siegeAbility;
            case "Construction Cost":
                return constructionCost;
            case "Construction Time":
                return constructionTime;
            case "Population Growth":
                return populationGrowth;
            case "Maximum Population":
                return maximumPopulation;
            case "Tax Efficiency":
                return taxEfficiency;
            case "Tax Income":
                return taxIncome;
            case "Daily Control":
                return dailyControl;
            case "Control Decay":
                return controlDecay;
            case "Production Value":
                return productionValue;
            case "Production Amount":
                return productionAmount;
            case "Movement Speed":
                return movementSpeed;
            case "Recruitment Cost":
                return regimentCost;
            case "Regiment Maintenance Cost":
                return regimentMaintenanceCost;
            case "Recruitment Time":
                return recruitmentTime;
            case "Stability Cost":
                return stabilityCost;
            case "Core Cost":
                return coreCost;
            case "Coring Range":
                return coringRange;
            case "Conversion Cost":
                return conversionCost;
            case "Prestige Decay":
                return prestigeDecay;
            case "Monthly Prestige":
                return monthlyPrestige;
            case "Army Tradition Decay":
                return armyTraditionDecay;
            case "Monthly Tradition":
                return monthlyTradition;
            case "Global Unrest":
                return globalUnrest;
            case "True Faith Tolerance":
                return trueFaithTolerance;
            case "Infidel Intolerance":
                return infidelIntolerance;
            case "War Score Cost":
                return warScoreCost;
            case "Maximum Advisors":
                return maximumAdvisors;
            case "Advisor Cost":
                return advisorCosts;
            case "Admin Advisor Cost":
                return advisorCostsA;
            case "Diplo Advisor Cost":
                return advisorCostsD;
            case "Military Advisor Cost":
                return advisorCostsM;
            case "Idea Cost":
                return ideaCosts;
            case "Tech Cost":
                return techCosts;
            case "Admin Tech Cost":
                return techCostsA;
            case "Diplo Tech Cost":
                return techCostsD;
            case "Military Tech Cost":
                return techCostsM;
            case "Prestige from Battles":
                return battlePrestige;
            case "Army Tradition from Battles":
                return battleTraditon;
            case "Diplo Reputation":
                return diploRep;
            case "Liberty Desire in Subjects":
                return libDesire;
            case "Liberty Desire from Subjects Development":
                return libDesireFromDev;
            case "Income from Subjects":
                return incomeFromSubjects;
            case "Diplomatic Annexation Cost":
                return dipAnnexCost;
            case "Months of Seperatism":
                return monthsOfSeperatism;
            case "Improve Relations":
                return improveRelations;
            case "Aggressive Expansion Impact":
                return aggressiveExpansionImpact;
            case "Diplo Relations":
                return diploRelations;
            case "Interest":
                return interestPerMonth;
            case "Ruler Admin Skill":
                return rulerAdminSkill;
            case "Ruler Diplo Skill":
                return rulerDiploSkill;
            case "Ruler Military Skill":
                return rulerMilSkill;
            case "Reform Progress Growth":
                return reformProgressGrowth;
            case "Trade Value":
                return tradeValue;
            case "Trade Penalty":
                return tradePenalty;
            case "Trade Value per Civ in Node":
                return tradeValPerCiv;
            case "Minimum Control":
                return minControl;
            case "Attacker Dice Roll":
                return attackerDiceRoll;
            case "Defender Dice Roll":
                return defenderDiceRoll;
            case "Attrition for Enemies":
                return attritionForEnemies;
            case "Land Attrition":
                return landAttrition;
            case "General Melee Skill":
                return generalMeleeSkill;
            case "General Flanking Skill":
                return generalFlankingSkill;
            case "General Ranged Skill":
                return generalRangedSkill;
            case "General Siege Skill":
                return generalSiegeSkill;
            case "General Maneuver Skill":
                return generalManeuverSkill;
            case "Governing Capacity":
                return governingCapacityMax;
            default:
                return null;
        }
    }
    public void ApplyCivModifier(string modifierName, float strength,string reason,int modifierType = 1,int time = -1)
    {
        if (modifierType == 4)
        {
            ApplyCivBonusNonModifier(modifierName);
        }
        else
        {
            Stat stat = GetStat(modifierName);
            if (stat != null)
            {
                stat.AddModifier(new Modifier(strength, modifierType, reason, time));
            }
        }
    }
    public void RemoveCivModifier(string statname, string modname)
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
        Debug.Log(reforms.Count);
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
