using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] public class SaveGameCiv
{
    public float AIAggressiveness;
    public float AIAdministrative;
    public float AIDiplomatic;
    public float AIMilitary;

    public Vector3Int capitalPos;

    public List<Vector3Int> controlCentrePos = new List<Vector3Int>();
    public List<int> controlCentreLevel = new List<int>();

    public bool isPlayer;

    public List<SaveGameArmy> armies;
    public List<SaveGameFleet> fleets;

    public List<int> atWarWith;
    public List<int> atWarTogether;
    public List<int> militaryAccess;
    public List<int> allies;
    public List<int> subjects;
    public List<int> eventHistory;

    public int[] rivals;
    public int[] mercTimers;
    public int[] truces;

    public List<General> generals;

    public int overlordID;
    public int annexationProgress;
    public bool integrating;

    public float religiousPoints;
    public float reformProgress;
    public float coins;
    public float prestige;
    public float armyTradition;
    public float revanchism;
    public int stability;
    public int adminPower;
    public int diploPower;
    public int milPower;

    public Ruler ruler;
    public Ruler heir;
    public Advisor advisorA;
    public Advisor advisorD;
    public Advisor advisorM;
    public List<Advisor> advisorsA;
    public List<Advisor> advisorsD;
    public List<Advisor> advisorsM;

    public int adminTech;
    public int diploTech;
    public int milTech;
    public int focus;
    public int focusCD;
    public int religion;
    public int government;
    public int governmentRank;

    public IdeaGroupData[] ideaGroups;

    public List<Loan> loans;
    public List<int> reforms;
    public List<Vector3Int> claims;

    public Stat maxSettlements;
    public Stat promoteSettlementCost;
    public Stat governingCapacityMax;
    public Stat governingCostModifier;
    public Stat diplomaticCapacityMax;
    public Stat attritionForEnemies;
    public Stat landAttrition;
    public Stat attackerDiceRoll;
    public Stat defenderDiceRoll;
    public Stat generalMeleeSkill;
    public Stat generalFlankingSkill;
    public Stat generalRangedSkill;
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
    public Stat flankingSlots;
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
    public Stat conversionCost;
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
    public Stat[] opinionOfThem;

    public SaveGameCiv()
    {

    }
    public void SaveCiv(Civilisation civ)
    {
        AIAggressiveness = civ.AIAggressiveness;
        AIAdministrative = civ.AIAdministrative;
        AIDiplomatic = civ.AIDiplomatic;
        AIMilitary = civ.AIMilitary;

        capitalPos = civ.capitalPos;

        foreach(var controlCentre in civ.controlCentres)
        {
            controlCentrePos.Add(controlCentre.Key);
            controlCentreLevel.Add(controlCentre.Value);
        }

        isPlayer = civ.isPlayer;

        armies = civ.armies.ConvertAll(i => new SaveGameArmy(i));
        fleets = civ.fleets.ConvertAll(i => new SaveGameFleet(i));

        atWarWith = civ.atWarWith;
        atWarTogether = civ.atWarTogether;
        militaryAccess = civ.militaryAccess;
        allies = civ.allies;
        subjects = civ.subjects;
        eventHistory = civ.eventHistory;

        rivals = civ.rivals;
        mercTimers = civ.mercTimers;
        truces = civ.truces;

        generals = civ.generals;

        overlordID = civ.overlordID;
        annexationProgress = civ.annexationProgress;
        integrating = civ.integrating;

        religiousPoints = civ.religiousPoints;
        reformProgress = civ.reformProgress;
        coins = civ.coins;
        prestige = civ.prestige;
        armyTradition = civ.armyTradition;
        revanchism = civ.revanchism;
        stability = civ.stability;
        adminPower = civ.adminPower;
        diploPower = civ.diploPower;
        milPower = civ.milPower;

        ruler = civ.ruler;
        heir = civ.heir;
        advisorA = civ.advisorA;
        advisorD = civ.advisorD;
        advisorM = civ.advisorM;
        advisorsA = civ.advisorsA;
        advisorsD = civ.advisorsD;
        advisorsM = civ.advisorsM;

        adminTech = civ.adminTech;
        diploTech = civ.diploTech;
        milTech = civ.milTech;
        focus = civ.focus;
        focusCD = civ.focus;
        religion = civ.religion;
        government = civ.government;
        governmentRank = civ.governmentRank;

        ideaGroups = civ.ideaGroups;

        loans = civ.loans;
        reforms = civ.reforms;
        claims = civ.claims;

         maxSettlements = civ.maxSettlements;
         promoteSettlementCost = civ.promoteSettlementCost;
         governingCapacityMax = civ.governingCapacityMax;
         governingCostModifier = civ.governingCostModifier;
         diplomaticCapacityMax = civ.diplomaticCapacityMax;
         attritionForEnemies = civ.attritionForEnemies;
         landAttrition = civ.landAttrition;
         attackerDiceRoll = civ.attackerDiceRoll;
         defenderDiceRoll = civ.defenderDiceRoll;
         generalMeleeSkill = civ.generalMeleeSkill;
         generalFlankingSkill = civ.generalFlankingSkill;
         generalRangedSkill = civ.generalRangedSkill;
         generalSiegeSkill = civ.generalSiegeSkill;
         generalManeuverSkill = civ.generalManeuverSkill;
         rulerAdminSkill = civ.rulerAdminSkill;
         rulerDiploSkill = civ.rulerDiploSkill;
         rulerMilSkill = civ.rulerMilSkill;
         reformProgressGrowth = civ.reformProgressGrowth;
         militaryTactics = civ.militaryTactics;
         discipline = civ.discipline;
         moraleMax = civ.moraleMax;
         moraleRecovery = civ.moraleRecovery;
         reinforceSpeed = civ.reinforceSpeed;
         combatWidth = civ.combatWidth;
         flankingSlots = civ.flankingSlots;
         devCostMod = civ.devCostMod;
         devCost = civ.devCost;
         fortDefence = civ.fortDefence;
         fortMaintenance = civ.fortMaintenance;
         siegeAbility = civ.siegeAbility;
         constructionCost = civ.constructionCost;
         constructionTime = civ.constructionTime;
         populationGrowth = civ.populationGrowth;
         maximumPopulation = civ.maximumPopulation;
         taxEfficiency = civ.taxEfficiency;
         taxIncome = civ.taxIncome;
         productionValue = civ.productionValue;
         productionAmount = civ.productionAmount;
         dailyControl = civ.dailyControl;
         controlDecay = civ.controlDecay;
         regimentMaintenanceCost = civ.regimentMaintenanceCost;
         regimentCost = civ.regimentCost;
         recruitmentTime = civ.recruitmentTime;
         movementSpeed = civ.movementSpeed;
         forceLimit = civ.forceLimit;
         prestigeDecay = civ.prestigeDecay;
         monthlyPrestige = civ.monthlyPrestige;
         armyTraditionDecay = civ.armyTraditionDecay;
         monthlyTradition = civ.monthlyTradition;
         stabilityCost = civ.stabilityCost;
         coreCost = civ.coreCost;
         conversionCost = civ.conversionCost;
         maximumAdvisors = civ.maximumAdvisors;
         advisorCosts = civ.advisorCosts;
         advisorCostsA = civ.advisorCostsA;
         advisorCostsD = civ.advisorCostsD;
         advisorCostsM = civ.advisorCostsM;
         techCosts = civ.techCosts;
         techCostsA = civ.techCostsA;
         techCostsD = civ.techCostsD;
         techCostsM = civ.techCostsM;
         ideaCosts = civ.ideaCosts;
         globalUnrest = civ.globalUnrest;
         trueFaithTolerance = civ.trueFaithTolerance;
         infidelIntolerance = civ.infidelIntolerance;
         warScoreCost = civ.warScoreCost;
         dipAnnexCost = civ.dipAnnexCost;
         libDesireFromDevForSubjects = civ.libDesireFromDevForSubjects;
         battlePrestige = civ.battlePrestige;
         battleTraditon = civ.battleTraditon;
         diploRep = civ.diploRep;
         libDesireInSubjects = civ.libDesireInSubjects;
         incomeFromSubjects = civ.incomeFromSubjects;
         improveRelations = civ.improveRelations;
         aggressiveExpansionImpact = civ.aggressiveExpansionImpact;
         monthsOfSeperatism = civ.monthsOfSeperatism;
         coringRange = civ.coringRange;
         interestPerMonth = civ.interestPerMonth;
         minControl = civ.minControl;
         tradeValue = civ.tradeValue;
         tradePenalty = civ.tradePenalty;
         tradeValPerCiv = civ.tradeValPerCiv;
        opinionOfThem = civ.opinionOfThem;
    }
    public void LoadToCiv(Civilisation civ)
    {
        civ.AIAggressiveness = AIAggressiveness;
        civ.AIAdministrative = AIAdministrative;
        civ.AIDiplomatic = AIDiplomatic;
        civ.AIMilitary = AIMilitary;

        civ.capitalPos = capitalPos;
        civ.capitalIndicator.transform.position = Map.main.GetTile(capitalPos).worldPos();

        civ.controlCentres.Clear();
        for(int i = 0; i < controlCentrePos.Count;i++)
        {
            civ.controlCentres.Add(controlCentrePos[i], controlCentreLevel[i]);
        }

        civ.isPlayer = isPlayer;

        foreach(var army in armies)
        {
            if (!army.inBattle)
            {
                army.NewArmy();
            }
        }
        foreach(var fleet in fleets)
        {
            if (!fleet.inBattle)
            {
                fleet.NewFleet();
            }
        }

        civ.atWarWith = atWarWith;
        civ.atWarTogether = atWarTogether;
        civ.militaryAccess = militaryAccess;
        civ.allies = allies;
        civ.subjects = subjects;
        civ.eventHistory = eventHistory;

        civ.rivals = rivals;
        civ.mercTimers = mercTimers;
        civ.truces = truces;

        civ.generals = generals;

        civ.overlordID = overlordID;
        civ.annexationProgress = annexationProgress;
        civ.integrating = integrating;

        civ.religiousPoints = religiousPoints;
        civ.reformProgress = reformProgress;
        civ.coins = coins;
        civ.prestige = prestige;
        civ.armyTradition = armyTradition;
        civ.revanchism = revanchism;
        civ.stability = stability;
        civ.adminPower = adminPower;
        civ.diploPower = diploPower;
        civ.milPower = milPower;

        civ.ruler = ruler;
        civ.heir = heir;
        civ.advisorA = advisorA;
        civ.advisorD = advisorD;
        civ.advisorM = advisorM;
        civ.advisorsA = advisorsA;
        civ.advisorsD = advisorsD;
        civ.advisorsM = advisorsM;

        civ.adminTech = adminTech;
        civ.diploTech = diploTech;
        civ.milTech = milTech;
        civ.focus = focus;
        civ.focusCD = focus;
        civ.religion = religion;
        civ.government = government;
        civ.governmentRank = governmentRank;

        civ.ideaGroups = ideaGroups;

        civ.loans = loans;
        civ.reforms = reforms;
        civ.claims = claims;

        civ.maxSettlements = maxSettlements;
        civ.promoteSettlementCost = promoteSettlementCost;
        civ.governingCapacityMax = governingCapacityMax;
        civ.governingCostModifier = governingCostModifier;
        civ.diplomaticCapacityMax = diplomaticCapacityMax;
        civ.attritionForEnemies = attritionForEnemies;
        civ.landAttrition = landAttrition;
        civ.attackerDiceRoll = attackerDiceRoll;
        civ.defenderDiceRoll = defenderDiceRoll;
        civ.generalMeleeSkill = generalMeleeSkill;
        civ.generalFlankingSkill = generalFlankingSkill;
        civ.generalRangedSkill = generalRangedSkill;
        civ.generalSiegeSkill = generalSiegeSkill;
        civ.generalManeuverSkill = generalManeuverSkill;
        civ.rulerAdminSkill = rulerAdminSkill;
        civ.rulerDiploSkill = rulerDiploSkill;
        civ.rulerMilSkill = rulerMilSkill;
        civ.reformProgressGrowth = reformProgressGrowth;
        civ.militaryTactics = militaryTactics;
        civ.discipline = discipline;
        civ.moraleMax = moraleMax;
        civ.moraleRecovery = moraleRecovery;
        civ.reinforceSpeed = reinforceSpeed;
        civ.combatWidth = combatWidth;
        civ.flankingSlots = flankingSlots;
        civ.devCostMod = devCostMod;
        civ.devCost = devCost;
        civ.fortDefence = fortDefence;
        civ.fortMaintenance = fortMaintenance;
        civ.siegeAbility = siegeAbility;
        civ.constructionCost = constructionCost;
        civ.constructionTime = constructionTime;
        civ.populationGrowth = populationGrowth;
        civ.maximumPopulation = maximumPopulation;
        civ.taxEfficiency = taxEfficiency;
        civ.taxIncome = taxIncome;
        civ.productionValue = productionValue;
        civ.productionAmount = productionAmount;
        civ.dailyControl = dailyControl;
        civ.controlDecay = controlDecay;
        civ.regimentMaintenanceCost = regimentMaintenanceCost;
        civ.regimentCost = regimentCost;
        civ.recruitmentTime = recruitmentTime;
        civ.movementSpeed = movementSpeed;
        civ.forceLimit = forceLimit;
        civ.prestigeDecay = prestigeDecay;
        civ.monthlyPrestige = monthlyPrestige;
        civ.armyTraditionDecay = armyTraditionDecay;
        civ.monthlyTradition = monthlyTradition;
        civ.stabilityCost = stabilityCost;
        civ.coreCost = coreCost;
        civ.conversionCost = conversionCost;
        civ.maximumAdvisors = maximumAdvisors;
        civ.advisorCosts = advisorCosts;
        civ.advisorCostsA = advisorCostsA;
        civ.advisorCostsD = advisorCostsD;
        civ.advisorCostsM = advisorCostsM;
        civ.techCosts = techCosts;
        civ.techCostsA = techCostsA;
        civ.techCostsD = techCostsD;
        civ.techCostsM = techCostsM;
        civ.ideaCosts = ideaCosts;
        civ.globalUnrest = globalUnrest;
        civ.trueFaithTolerance = trueFaithTolerance;
        civ.infidelIntolerance = infidelIntolerance;
        civ.warScoreCost = warScoreCost;
        civ.dipAnnexCost = dipAnnexCost;
        civ.libDesireFromDevForSubjects = libDesireFromDevForSubjects;
        civ.battlePrestige = battlePrestige;
        civ.battleTraditon = battleTraditon;
        civ.diploRep = diploRep;
        civ.libDesireInSubjects = libDesireInSubjects;
        civ.incomeFromSubjects = incomeFromSubjects;
        civ.improveRelations = improveRelations;
        civ.aggressiveExpansionImpact = aggressiveExpansionImpact;
        civ.monthsOfSeperatism = monthsOfSeperatism;
        civ.coringRange = coringRange;
        civ.interestPerMonth = interestPerMonth;
        civ.minControl = minControl;
        civ.tradeValue = tradeValue;
        civ.tradePenalty = tradePenalty;
        civ.tradeValPerCiv = tradeValPerCiv;
        civ.opinionOfThem = opinionOfThem;
    }
}