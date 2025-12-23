using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class Effect
{
    public string name;
    public EffectType type;
    public float amount;
    public int duration = -1;
    public bool isProvince = false;

    public string GetHoverText(Civilisation civ)
    {
        string text = "";
        if (type == EffectType.Other)
        {
            text += name + "\n";
        }
        else
        {
            text += name + " " + Modifier.ToString(amount, civ.GetStat(name),type == EffectType.Multiplicative || type == EffectType.Additive, type == EffectType.Base) + "\n";
        }        
        return text;
    }
    
}
public enum EffectType
{
    Additive,
    Flat,
    Multiplicative,
    Base,
    Other,
}

public enum EffectName
{
    AdminAdvisorCost,
    AdminTechCost,
    AdvisorCost,
    AggressiveExpansionImpact,
    ArmyTraditionDecay,
    ArmyTraditionfromBattles,
    ArtilleryCost,
    ArtilleryDamage,
    AttackerDiceRoll,
    AttritionforEnemies,
    CavalryCost,
    CavalryDamage,
    CombatWidth,
    ConstructionCost,
    ConstructionTime,
    ControlDecay,
    CoreCost,
    CoringRange,
    DailyControl,
    DefenderDiceRoll,
    DevelopmentCost,
    DevelopmentCostModifier,
    DiploAdvisorCost,
    DiploReputation,
    DiploTechCost,
    DiplomaticAnnexationCost,
    DiplomaticCapacity,
    Discipline,
    ForceLimit,
    FortDefence,
    FortMaintenance,
    GeneralCombatSkill,
    GeneralManeuverSkill,
    GeneralSiegeSkill,
    GlobalUnrest,
    GoverningCapacity,
    GoverningCost,
    IdeaCost,
    ImproveRelations,
    IncomefromSubjects,
    InfantryCost,
    InfantryDamage,
    InfidelIntolerance,
    Interest,
    LandAttrition,
    LibertyDesirefromSubjectsDevelopment,
    LibertyDesireinSubjects,
    MaxSettlements,
    MaximumAdvisors,
    MaximumDiplomats,
    MaximumMissionaries,
    MaximumPopulation,
    MilitaryAdvisorCost,
    MilitaryTechCost,
    MinimumControl,
    MissionaryStrength,
    MonthlyPrestige,
    MonthlyTradition,
    MonthsofSeperatism,
    Morale,
    MoraleRecovery,
    MovementSpeed,
    PopulationGrowth,
    PrestigeDecay,
    PrestigefromBattles,
    ProductionAmount,
    ProductionValue,
    PromoteSettlementCost,
    RecruitmentCost,
    RecruitmentTime,
    ReformProgressGrowth,
    RegimentCost,
    RegimentMaintenanceCost,
    ReinforceSpeed,
    RulerAdminSkill,
    RulerDiploSkill,
    RulerMilitarySkill,
    SiegeAbility,
    StabilityCost,
    Tactics,
    TaxEfficiency,
    TaxIncome,
    TechCost,
    TradePenalty,
    TradeValue,
    TradeValueperCivinNode,
    TrueFaithTolerance,
    WarScoreCost,

}

