using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static UnityEditor.PlayerSettings;

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
    public List<Army> armies = new List<Army>();
    public List<int> atWarWith = new List<int>();
    public List<int> militaryAccess = new List<int>();
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
    public Stat militaryTactics = new Stat(0.5f,"Military Tactics",true);
    public Stat discipline = new Stat(1f, "Discipline");
    public Stat moraleMax = new Stat(0f, "Maximum Morale");
    public Stat moraleRecovery = new Stat(0f, "Morale Recovery");
    public Stat reinforceSpeed = new Stat(0f, "Reinforce Speed");
    public List<UnitType> units = new List<UnitType>() { new UnitType("Infantry", 0f, 0f, 0f,1,10f), new UnitType("Cavalry", 0f, 0f, 0f,2,20f), new UnitType("Artillery", 0f, 0f, 0f,2,30f) };
    public Stat infantryCombatAbility = new Stat(0f, "Infantry Combat Ability");
    public Stat flankingCombatAbility = new Stat(0f, "Flanking Combat Ability");
    public Stat siegeCombatAbility = new Stat(0f, "Siege Combat Ability");
    public Stat combatWidth = new Stat(15f, "Combat Width", true);
    public Stat flankingSlots = new Stat(2f, "Flanking Slots", true);
    public Stat devCostMod = new Stat(0f, "Development Cost Modifier");
    public Stat devCost = new Stat(0f, "Development Cost");
    public Stat fortDefence = new Stat(0f, "Fort Defence");
    public Stat siegeAbility = new Stat(0f, "Siege Ability");
    public Stat constructionCost = new Stat(0f, "Construction Cost");
    public Stat constructionTime = new Stat(0f, "Construction Time");
    public Stat populationGrowth = new Stat(0f, "Population Growth");
    public Stat maximumPopulation = new Stat(0f, "Maximum Population");
    public Stat taxEfficiency = new Stat(0f, "Tax Efficiency");
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
    public Stat stabilityCost = new Stat(0f, "Stability Cost");
    public Stat coreCost = new Stat(0f, "Core Creation Cost");
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
    public Stat warScoreCost = new Stat(0f, "War Score Cost");
    public Stat battlePrestige = new Stat(0f, "Prestige From Battles");
    public Stat battleTraditon = new Stat(0f, "Army Tradition From Battles");
    public Stat diploRep = new Stat(0f, "Diplomatic Reputation",true);
    public Stat improveRelations = new Stat(0f, "Improve Relations");
    public Stat interestPerMonth = new Stat(0.04f, "Interest");
    public List<Vector3Int> civTiles = new List<Vector3Int>();
    public List<int> civNeighbours = new List<int>();
    public int[] truces;
    public bool hasUpdatedStartingResources = false;
    public List<WeightedChoice> events = new List<WeightedChoice>();
    public void UpdateStartingResources()
    {
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
        float coinsIncome = 1f;
        float ForceLimit = 0f;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            tileData.SetMaxControl();
            tileData.avaliablePopulation = Mathf.Min(tileData.avaliableMaxPopulation, (int)(tileData.population * tileData.control / 100f), tileData.avaliablePopulation);
            coinsIncome += tileData.GetDailyProductionValue();
            coinsIncome += tileData.GetDailyTax();
            ForceLimit += tileData.GetForceLimit();
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
        ApplyCivModifier(nationalIdeas.traditionOne, nationalIdeas.traditonAmountOne, "Tradition one",nationalIdeas.traditionTypeOne);
        ApplyCivModifier(nationalIdeas.traditonTwo, nationalIdeas.traditionAmountTwo, "Tradition two",nationalIdeas.traditonTypeTwo);
        AddPulseEvents();
    }
    public void Init()
    {
        Game.main.start.AddListener(GameStart);
        Game.main.dayTick.AddListener(DayTick);
        Game.main.tenMinTick.AddListener(TenMinTick);
        Game.main.monthTick.AddListener(MonthTick);
        InitExploration();
        truces = new int[Game.main.civs.Count];       
    }
    public void AddPrestige(float amount)
    {
        prestige = Mathf.Clamp(prestige + amount, -100f, 100f);
    }
    public void AddArmyTradition(float amount)
    {
        armyTradition = Mathf.Clamp(armyTradition + amount, 0f, 100f);
    }
    void MonthTick()
    {
        if(civTiles.Count == 0) { return; }
        AddPrestige(-prestige * prestigeDecay.value);
        AddPrestige(monthlyPrestige.value);
        AddArmyTradition( -armyTradition * armyTraditionDecay.value);
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
                    eventData.province = Map.main.GetTile(GetAllCivTiles()[UnityEngine.Random.Range(0, GetAllCivTiles().Count)]);
                }
                SendEvent(eventData);
            }
        }
    }
    void AddPulseEvents()
    {
        events.Clear();
        events.Add(new WeightedChoice(-1, 1000));
        for (int i = 0; i < Map.main.pulseEvents.Length;i++)
        {
            events.Add(new WeightedChoice(i,100));
        }
    }
    void RemoveAdvisor(Advisor advisor)
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
        moraleMax.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 0);
        moraleMax.UpdateModifier("Army Tradition", (armyTradition * 0.25f) / 100f, 0);
        siegeAbility.UpdateModifier("Army Tradition", (armyTradition * 0.1f) / 100f, 1);
        populationGrowth.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 1);
        taxEfficiency.UpdateModifier("Prestige", (prestige * 0.1f) / 100f, 1);
        taxEfficiency.UpdateModifier("Stability", stability * 0.05f, 1);
        stabilityCost.UpdateModifier("Stability",stability > 0 ? stability * 0.50f : 0f, 1);
        stabilityCost.UpdateModifier("Overextension", (overextension * 0.5f)/100f, 1);
        dailyControl.UpdateModifier("Overextension", (-overextension * 0.05f) / 100f, 1);
        globalUnrest.UpdateModifier("Overextension", (overextension * 5f) / 100f, 1);
        interestPerMonth.UpdateModifier("Stability", stability < 0 ? stability * -0.01f : 0f, 1);
        globalUnrest.UpdateModifier("Stability", stability > 0 ? stability * -1f : stability * -2f, 1);
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            if(AvaliablePopulation() < 1000) { break; }
            if (tileData.recruitTimer > 0 && tileData.recruitQueue.Count > 0 && !tileData.occupied && !tileData.underSiege)
            {
                tileData.recruitTimer--;
                if (tileData.recruitTimer == 0)
                {
                    tileData.CreateNewArmy(tileData.recruitQueue.First());
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
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);

            ForceLimit += tileData.GetForceLimit();
        }
        forceLimit.ChangeBaseStat(ForceLimit);
    }
    public int GetStabilityCost()
    {
        int baseC = 100;
        return (int)(baseC * (1f + stabilityCost.value));
    }
    public int GetTotalTilePopulation()
    {
        int pop = 0;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            pop += tileData.avaliablePopulation;            
        }
        return pop;
    }
    public int GetTotalPopulationGrowth()
    {
        int pop = 0;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            pop += tileData.populationGrowth;
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
        return (int)(income * 30f / (GetLoanSize() * interestPerMonth.value));
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
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            pop += tileData.avaliableMaxPopulation;
        }
        return pop;
    }
    public int AddPopulation(int targetAmount)
    {
        int amount = 0;
        List<TileData> tiles = GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
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
            tiles.Sort((x, y) => (x.avaliablePopulation / x.avaliableMaxPopulation).CompareTo((y.avaliablePopulation / y.avaliableMaxPopulation)));
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
        List<TileData> tiles = GetAllCivTiles().ConvertAll(i=> Map.main.GetTile(i));
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
    public int AvaliablePopulation()
    {
        int amount = 0;
        List<TileData> tiles = GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
        tiles.RemoveAll(i => i.occupied || i.underSiege);
        tiles.RemoveAll(i => i.avaliablePopulation == 0 || i.avaliableMaxPopulation == 0);
        if (tiles.Count == 0) { return 0; }
        foreach (var tile in tiles)
        {
            int used = Mathf.Max(0, tile.avaliablePopulation);
            amount += used;
        }
        return amount;
    }
    public float ProductionIncome()
    {
        float coinsIncome = 0f;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);           
            coinsIncome += tileData.GetDailyProductionValue();           
        }
        return coinsIncome ;
    }
    public float TaxIncome()
    {
        float coinsIncome = 1f;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            coinsIncome += tileData.GetDailyTax();
        }
        return coinsIncome;
    }
    public float FortMaintenance()
    {
        float fortM = 0f;
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            if (tileData.hasFort)
            {
                fortM += 1;
            }
        }
        return fortM;
    }
    public void AddStability(int amount)
    {
        stability = Mathf.Clamp(stability + amount, -3, 3);
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
        foreach (var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
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
                armyCosts += units[regiment.type].baseCost * (float)regiment.size / (float)regiment.maxSize * 0.25f / 12f;
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
    public void DayTick()
    {
        if(CivID == Player.myPlayer.myCivID)
        {
            NotificationsUI.main.ClearNotifications();
        }
        if(civTiles.Count == 0)
        {
            Game.main.dayTick.RemoveListener(DayTick);
            Game.main.tenMinTick.RemoveListener(TenMinTick);
            GameObject.Destroy(capitalIndicator);
            foreach (var army in armies.ToList())
            {
                GameObject.Destroy(army.gameObject);
                armies.Remove(army);
            }
            foreach(var war in GetWars())
            {
                war.EndWar();
            }
            return;
        }
        RefillAdvisors();
        overextension = 0f;
        foreach(var tile in GetAllCivTiles())
        {
            TileData tileData = Map.main.GetTile(tile);
            tileData.SetMaxControl();
            if(tileData.coreTimer > -1 && !tileData.occupied && !atWarWith.Exists(i=>tileData.cores.Contains(i)))
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
                    notification.province = tileData;
                    notification.description = "You have provinces that need coring";
                    NotificationsUI.AddNotification(notification);
                }
            }
            if (!tileData.occupied)
            {
                tileData.seperatism = Mathf.Clamp(tileData.seperatism - 1, 0, 360);
                tileData.control = Mathf.Clamp(tileData.control + tileData.dailyControl.value + dailyControl.value, 0f, tileData.maxControl);
                tileData.population = Mathf.Min(tileData.maxPopulation, tileData.population + tileData.populationGrowth);
                tileData.avaliablePopulation = Mathf.Min(tileData.avaliableMaxPopulation, tileData.population, tileData.avaliablePopulation + (int)(tileData.populationGrowth * tileData.control / 100f + tileData.population * (tileData.dailyControl.value + dailyControl.value) / 100f));
            }
            else
            {
                tileData.population = Mathf.Clamp((int)(tileData.population * 0.99f),0, tileData.maxPopulation);
                tileData.avaliablePopulation = Mathf.Clamp((int)(tileData.avaliablePopulation * 0.99f), 0, tileData.avaliableMaxPopulation);
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
                    if (!rebelFactions[tileData.region].provinces.Contains(tile))
                    {
                        rebelFactions[tileData.region].provinces.Add(tile);
                    }
                }
                else
                {
                    rebelFactions.Add(tileData.region,new RebelFaction(tile));
                }
            }
        }
        foreach(var rebelFaction in rebelFactions.Values.ToList())
        {
            rebelFaction.Update();
        }
        if (ruler.active)
        {
            adminPower += 1 + ruler.adminSkill + (advisorA.active ? advisorA.skillLevel : 0);
            diploPower += 1 + ruler.diploSkill + (advisorD.active ? advisorD.skillLevel : 0);
            milPower += 1 + ruler.milSkill + (advisorM.active ? advisorM.skillLevel : 0);
        }
        coins += TaxIncome() + ProductionIncome();
        RefreshForceLimit();
        coins -= AdvisorMaintainance();
        coins -= ArmyMaintainance();
        coins -= FortMaintenance();
        coins -= GetInterestPayment();
        if(coins < 0)
        {
            TakeLoan();
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
                heir = Ruler.NewHeir(0, 0, 0, CivID);
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
            heir.Kill();           
        }
        if (CivID == Player.myPlayer.myCivID)
        {
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
            foreach (var idea in ideaGroups)
            {
                if (idea != null && idea.active && idea.unlockedLevel < 7)
                {
                    int points = idea.type == 0 ? adminPower : idea.type == 1 ? diploPower : milPower;
                    if (points >= IdeasUI.GetIdeaCost(this))
                    {
                        Notification notification = NotificationsUI.main.canTakeIdea;
                        notification.description = "You can take an idea";
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
    public void AcceptPeaceDeal(PeaceDeal peaceDeal)
    {
        int winnerID = peaceDeal.attacker? peaceDeal.war.attackerCiv.CivID : peaceDeal.war.defenderCiv.CivID;
        Civilisation winnerCiv = Game.main.civs[winnerID];
        winnerCiv.truces[CivID] = (int)(peaceDeal.warScore * 75f + 1000f);
        truces[winnerID] = (int)(peaceDeal.warScore * 75f + 1000f);
        foreach (var province in GetAllCivTiles().ToList())
        {
            TileData tile = Map.main.GetTile(province);
            if (peaceDeal.provinces.Contains(province))
            {
                winnerCiv.AddPrestige(0.25f * tile.totalDev);
                AddPrestige(-0.25f * tile.totalDev);
                tile.civID = winnerID;
                tile.buildQueue.Clear();
                tile.recruitQueue.Clear();
                if (!tile.hasCore)
                {
                    tile.seperatism += 360;
                    tile.control = Mathf.Min(10f,tile.control);
                }
                if (capitalPos == province)
                {
                    tile.fortLevel = Mathf.Max(tile.fortLevel-1,0);
                    NewCapital(peaceDeal.provinces);
                }
            }
        }       
    }
    void NewCapital(List<Vector3Int> notAllowed)
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
        capitalIndicator.transform.position = Map.main.GetTile(best).worldPos();
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
    public float GetTotalWarScore(int forCivID)
    {
        float totalWS = 0;
        var tiles = GetAllCivTiles();
        foreach (var tile in tiles)
        {
            TileData prov = Map.main.GetTile(tile);
            totalWS += prov.GetWarScore(forCivID);
        }
        return totalWS;
    }
    public int GetTotalDev()
    {
        int totalDev = 0;
        var tiles = GetAllCivTiles();
        foreach(var tile in tiles)
        {
            TileData prov = Map.main.GetTile(tile);
            totalDev += prov.totalDev;
        }
        return totalDev;
    }
    public void GameStart()
    {
        if(Player.myPlayer.myCivID == CivID) { isPlayer = true; }
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
    public void DiscoverResource(TileData tileData)
    {       
    }
    public void DeclareWar(int targetID)
    {
        if(Game.main.gameTime.totalTicks() < 6 * 24) { return; }
        if(atWarWith.Contains(targetID) || targetID == -1 || targetID == CivID || truces[targetID] > 0) { return; }
        atWarWith.Add(targetID);
        Game.main.civs[targetID].atWarWith.Add(CivID);
        War war = new War(this, Game.main.civs[targetID]);
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
        if (Player.myPlayer.myCivID == CivID)
        {
            if (eventData.affectsRandomProvince)
            {
                List<Vector3Int> tiles = GetAllCivTiles();
                eventData.province = Map.main.GetTile(tiles[UnityEngine.Random.Range(0,tiles.Count)]);
            }
            else if (eventData.affectsCapital)
            {
                eventData.province = Map.main.GetTile(capitalPos);
            }
            GameObject obj = GameObject.Instantiate(UIManager.main.eventPrefab,UIManager.main.eventTransform);
            obj.GetComponent<EventManager>().eventData = eventData;
            Game.main.paused = true;
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
                return infantryCombatAbility;
            case "Flanking Combat Ability":
                return flankingCombatAbility;
            case "Siege Combat Ability":
                return siegeCombatAbility;
            case "Development Cost":
                return devCost;
            case "Development Cost Modifier":
                return devCostMod;
            case "Force Limit":
                return forceLimit;
            case "Fort Defence":
                return fortDefence;
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
            case "Prestige Decay":
                return prestigeDecay;
            case "Monthly Prestige":
                return monthlyPrestige;
            case "Army Tradition Decay":
                return armyTraditionDecay;
            case "Global Unrest":
                return globalUnrest;
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
            case "Improve Relations":
                return improveRelations;
            default:
                return null;
        }
    }
    public void ApplyCivModifier(string modifierName, float strength,string reason,int modifierType = 1,int time = -1)
    {
        Stat stat = GetStat(modifierName);
        if (stat != null)
        {
            stat.AddModifier(new Modifier(strength, modifierType, reason, time));
        }
    }
    public void RemoveCivModifier(string modifierName, string reason)
    {
        Stat stat = GetStat(modifierName);
        if (stat != null)
        {
            if (stat.modifiers.Exists(item => item.name == reason))
            {
                stat.RemoveModifier(stat.modifiers.Find(item => item.name == reason));
            }
        }
    }
}
