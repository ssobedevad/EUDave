using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
[System.Serializable] public class SaveGameTile
{
    public int civId;
    public int devA, devB, devC;
    public float control;
    public int religion;
    public int status;
    public int infrastructureLevel;
    public int population;
    public int avaliablePopulation;
    public int seperatism;

    public List<int> buildings;
    public List<int> cores;

    public List<int> recruitQueue;
    public List<int> boatQueue;
    public List<int> mercenaryQueue;
    public List<int> buildQueue;

    public float siegeProgress;
    public bool occupied;
    public int occupiedById;

    public Stat localDevCostMod;
    public Stat localDevCost;
    public Stat localProductionValue;
    public Stat localProductionQuantity;
    public Stat localTaxEfficiency;
    public Stat localGoverningCost;
    public Stat localGoverningCostMod;
    public Stat localAttritionForEnemies;
    public Stat localMinimumControl;
    public Stat localConstructionCost;
    public Stat localConstructionTime;
    public Stat localMovementSpeed;
    public Stat localRecruitmentCost;
    public Stat localRecruitmentTime;
    public Stat dailyControl;
    public Stat localDefensiveness;
    public Stat localFortMaintenance;
    public Stat localPopulationGrowth;
    public Stat localMaxPopulation;
    public Stat localForceLimit;
    public Stat localAttackerDiceRoll;
    public Stat localUnrest;

    public SaveGameTile()
    {

    }
    public SaveGameTile(TileData tile)
    {
        civId = tile.civID;
        devA = tile.developmentA;
        devB = tile.developmentB;
        devC = tile.developmentC;
        control = tile.control;
        religion = tile.religion;
        status = tile.status;
        infrastructureLevel = tile.infrastructureLevel;
        population = tile.population;
        avaliablePopulation = tile.avaliablePopulation;
        seperatism = tile.seperatism;

        buildings = tile.buildings;
        cores = tile.cores;

        recruitQueue = tile.recruitQueue;
        boatQueue = tile.boatQueue;
        mercenaryQueue = tile.mercenaryQueue;
        buildQueue = tile.buildQueue;

        siegeProgress = tile.underSiege ? tile.siege.progress : 0f;
        occupied = tile.occupied;
        occupiedById = tile.occupiedByID;

        localDevCostMod = tile.localDevCostMod;
        localDevCost = tile.localDevCost;
        localProductionValue = tile.localProductionValue;
        localProductionQuantity = tile.localProductionQuantity;
        localTaxEfficiency = tile.localTaxEfficiency;
        localGoverningCost = tile.localGoverningCost;
        localGoverningCostMod = tile.localGoverningCostMod;
        localAttritionForEnemies = tile.localAttritionForEnemies;
        localMinimumControl = tile.localMinimumControl;
        localConstructionCost = tile.localConstructionCost;
        localConstructionTime = tile.localConstructionTime;
        localMovementSpeed = tile.localMovementSpeed;
        localRecruitmentCost = tile.localRecruitmentCost;
        localRecruitmentTime = tile.localRecruitmentTime;
        dailyControl = tile.dailyControl;
        localDefensiveness = tile.localDefensiveness;
        localFortMaintenance = tile.localFortMaintenance;
        localPopulationGrowth = tile.localPopulationGrowth;
        localMaxPopulation = tile.localMaxPopulation;
        localForceLimit = tile.localForceLimit;
        localAttackerDiceRoll = tile.localAttackerDiceRoll;
        localUnrest = tile.localUnrest;
    }

    public void LoadToTile(TileData data)
    {
        data.civID = civId;
        data.developmentA = devA;
        data.developmentB = devB;
        data.developmentC = devC;
        data.control = control;
        data.religion = religion;
        data.status = status;
        data.infrastructureLevel = infrastructureLevel;
        data.population = population;
        data.avaliablePopulation = avaliablePopulation;
        data.seperatism = seperatism;

        foreach(var buildingId in buildings)
        {
            if (!data.buildings.Contains(buildingId))
            {
                Building building = Map.main.Buildings[buildingId];
                if (building.fortLevel > 0)
                {
                    data.fortLevel += building.fortLevel;
                    if (!data.hasFort)
                    {
                        data.fort = GameObject.Instantiate(Map.main.fortPrefab, data.worldPos(), Quaternion.identity, Map.main.fortTransform);
                        data.hasFort = true;
                        data.ApplyZOC();
                    }
                }
                data.buildings.Add(buildingId);
            }
        }

        data.cores = cores;

        data.recruitQueue = recruitQueue;
        data.boatQueue = boatQueue;
        data.mercenaryQueue = mercenaryQueue;
        data.buildQueue = buildQueue;

        if (data.underSiege)
        {
            data.siege.progress = siegeProgress;
        }

        data.occupied = occupied;
        data.occupiedByID = occupiedById;

        data.localDevCostMod = localDevCostMod;
        data.localDevCost = localDevCost;
        data.localProductionValue = localProductionValue;
        data.localProductionQuantity = localProductionQuantity;
        data.localTaxEfficiency = localTaxEfficiency;
        data.localGoverningCost = localGoverningCost;
        data.localGoverningCostMod = localGoverningCostMod;
        data.localAttritionForEnemies = localAttritionForEnemies;
        data.localMinimumControl = localMinimumControl;
        data.localConstructionCost = localConstructionCost;
        data.localConstructionTime = localConstructionTime;
        data.localMovementSpeed = localMovementSpeed;
        data.localRecruitmentCost = localRecruitmentCost;
        data.localRecruitmentTime = localRecruitmentTime;
        data.dailyControl = dailyControl;
        data.localDefensiveness = localDefensiveness;
        data.localFortMaintenance = localFortMaintenance;
        data.localPopulationGrowth = localPopulationGrowth;
        data.localMaxPopulation = localMaxPopulation;
        data.localForceLimit = localForceLimit;
        data.localAttackerDiceRoll = localAttackerDiceRoll;
        data.localUnrest = localUnrest;
    }
}