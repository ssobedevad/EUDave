using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Condition
{
    public string conditionName;
    public ConditionID conditionID;
    public float conditionAmount;
    public bool opposite;
    public bool province;
    public bool isCapital;
    public Vector3Int provincePos;

    public bool isMet(Civilisation civ)
    {
        if (province)
        {
            TileData prov = isCapital ? Map.main.GetTile(civ.capitalPos) : Map.main.GetTile(provincePos);
            if (conditionID == ConditionID.Religion)
            {
                if (prov.religion == (int)conditionAmount)
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.Building)
            {
                if (prov.civID == civ.CivID && prov.buildings.Contains((int)conditionAmount))
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.Resource)
            {
                if (prov.civID == civ.CivID)
                {
                    switch (conditionName)
                    {
                        case "Development":
                            return opposite ? prov.totalDev < conditionAmount : prov.totalDev >= conditionAmount;
                        case "Status":
                            return opposite ? prov.status < conditionAmount : prov.status >= conditionAmount;
                        default:
                            return false;
                    }
                }
            }
            else if (conditionID == ConditionID.Location)
            {
                if (prov.civID == civ.CivID)
                {
                    if((int)conditionAmount == 0)
                    {
                        return opposite ? prov.region != conditionName : prov.region == conditionName;
                    }
                    else if ((int)conditionAmount == 1)
                    {
                        return opposite ? prov.tradeRegion != conditionName : prov.tradeRegion == conditionName;
                    }
                    else if ((int)conditionAmount == 2)
                    {
                        return opposite ? prov.continent != conditionName : prov.continent == conditionName;
                    }                    
                }
            }
        }
        else
        {
            if(conditionID == ConditionID.Stat) 
            {
                if(civ.GetStat(conditionName).v >= conditionAmount)
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.Government)
            {
                if (civ.government == (int)conditionAmount)
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.Religion)
            {
                if (civ.religion == (int)conditionAmount)
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.Resource)
            {
                switch (conditionName)
                {
                    case "Stability":
                        return opposite ? civ.stability < conditionAmount : civ.stability >= conditionAmount;
                    case "Prestige":
                        return opposite ? civ.prestige < conditionAmount : civ.prestige >= conditionAmount;
                    case "Coins":
                        return opposite ? civ.coins < conditionAmount : civ.coins >= conditionAmount;
                    case "Religious Unity":
                        return opposite ? civ.religiousUnity < conditionAmount : civ.religiousUnity >= conditionAmount;
                    case "Admin Power":
                        return opposite ? civ.adminPower < conditionAmount : civ.adminPower >= conditionAmount;
                    case "Diplo Power":
                        return opposite ? civ.diploPower < conditionAmount : civ.diploPower >= conditionAmount;
                    case "Mil Power":
                        return opposite? civ.milPower < conditionAmount : civ.milPower >= conditionAmount;
                    case "Religious Power":
                        return opposite ? civ.religiousPoints < conditionAmount : civ.religiousPoints >= conditionAmount;
                    case "Allies":
                        return opposite ? civ.allies.Count < conditionAmount : civ.allies.Count >= conditionAmount;
                    case "Subjects":
                        return opposite ? civ.subjects.Count < conditionAmount : civ.subjects.Count >= conditionAmount;
                    case "Idea Count":
                        return opposite ? civ.totalIdeas < conditionAmount : civ.totalIdeas >= conditionAmount;
                    case "Admin Tech":
                        return opposite ? civ.adminTech < conditionAmount : civ.adminTech >= conditionAmount;
                    case "Diplo Tech":
                        return opposite ? civ.diploTech < conditionAmount : civ.diploTech >= conditionAmount;
                    case "Mil Tech":
                        return opposite ? civ.milTech < conditionAmount : civ.milTech >= conditionAmount;
                    case "Development":
                        return opposite ? civ.GetTotalDev() < conditionAmount : civ.GetTotalDev() >= conditionAmount;
                    default:
                        return false;
                }
            }
            else if (conditionID == ConditionID.HadEvent)
            {
                if (civ.eventHistory.Contains((int)conditionAmount))
                {
                    return opposite ? false : true;
                }
            }
            else if (conditionID == ConditionID.ReligiousPercent)
            {
                int religion = -1;
                if(int.TryParse(conditionName, out religion))
                {
                    float religiousPercent = (float)civ.religiousDevelopment[religion] / (float)civ.GetTotalDev();
                    return opposite ? religiousPercent < conditionAmount : religiousPercent >= conditionAmount;
                }
                else
                {
                    return true;
                }

            }
            else if (conditionID == ConditionID.CivFact)
            {
                if(conditionName == "Marsh Leader")
                {
                    return opposite ? !civ.isMarshLeader : civ.isMarshLeader;
                }
                return true;
            }
            else if (conditionID == ConditionID.ArmyInfo)
            {
                if (conditionName == "Force Limit Percentage")
                {
                    float percent = (civ.TotalMaxArmySize() / 1000f) / civ.forceLimit.v;
                    return opposite ? percent < conditionAmount : percent >= conditionAmount;
                }
                if (conditionName == "Army Size")
                {
                    float armySize = civ.TotalMaxArmySize() / 1000f;
                    return opposite ? armySize < conditionAmount : armySize >= conditionAmount;
                }
                return true;
            }
            else if (conditionID == ConditionID.EconInfo)
            {
                if (conditionName == "Starting Income Percentage")
                {
                    float percent = civ.GetTotalIncome() / civ.startingEcon;
                    return opposite ? percent < conditionAmount : percent >= conditionAmount;
                }
                else if (conditionName == "Starting Tiles Increase")
                {
                    float tileDiff = civ.GetAllCivTileDatas().Count - civ.startTiles;
                    return opposite ? tileDiff < conditionAmount : tileDiff >= conditionAmount;
                }
                else if (conditionName == "Total Income")
                {
                    float income = civ.GetTotalIncome();
                    return opposite ? income < conditionAmount : income >= conditionAmount;
                }
                else if (conditionName == "Total Provinces")
                {
                    float provs = civ.GetAllCivTileDatas().Count;
                    return opposite ? provs < conditionAmount : provs >= conditionAmount;
                }
                return true;
            }
        }
        return opposite ? true : false;
    }    
    public string ToString(Civilisation civ)
    {
        string text = "";
        if (province)
        {
            TileData prov = isCapital ? Map.main.GetTile(civ.capitalPos) : Map.main.GetTile(provincePos);
            if (conditionID == ConditionID.Religion)
            {               
                text = "Requires Religion In: " + prov.Name + (opposite ? " != " : " == ") + Map.main.religions[(int)conditionAmount].name;               
            }
            if (conditionID == ConditionID.Building)
            {
                text = "Requires Building In: " + prov.Name + (opposite ? " != " : " == ") + Map.main.Buildings[(int)conditionAmount].Name;
            }
            else if (conditionID == ConditionID.Resource)
            {
                text = "Requires Resource "+conditionName+" In: " + prov.Name + (opposite ? " != " : " >= ") +conditionAmount;
            }
            else if (conditionID == ConditionID.Location)
            {
                if (prov.civID == civ.CivID)
                {
                    if ((int)conditionAmount == 0)
                    {
                        text = "Requires Region " + conditionName + " In: " + prov.Name;
                    }
                    else if ((int)conditionAmount == 1)
                    {
                        text = "Requires Trade Region " + conditionName + " In: " + prov.Name;
                    }
                    else if ((int)conditionAmount == 2)
                    {
                        text = "Requires Continent " + conditionName + " In: " + prov.Name;
                    }
                }
            }
        }
        else
        {
            if (conditionID == ConditionID.Stat)
            {
                text = "Requires Stat " + conditionName + (opposite?" < " :" >= ") + conditionAmount;                
            }
            else if (conditionID == ConditionID.Government)
            {
                text = "Requires Government" + (opposite ? " != " : " == ") + Map.main.governmentTypes[(int)conditionAmount].name;
            }
            else if (conditionID == ConditionID.Religion)
            {
                text = "Requires Religion" + (opposite ? " != " : " == ") + Map.main.religions[(int)conditionAmount].name;
            }
            else if (conditionID == ConditionID.Resource)
            {
                text = "Requires Resource " + conditionName +(opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount * 100f)/100f + ")";
            }
            else if (conditionID == ConditionID.HadEvent)
            {
                text = "Requires Event" + (opposite ? " != " : " == ") + Map.main.pulseEvents[(int)conditionAmount].Name;
            }
            else if (conditionID == ConditionID.ReligiousPercent)
            {
                int religion = -1;
                if (int.TryParse(conditionName, out religion))
                {
                    float religiousPercent = (float)civ.religiousDevelopment[religion] / (float)civ.GetTotalDev();
                    text = "Requires Religion "+ Map.main.religions[religion].name +" Percent: " + Mathf.Round(religiousPercent * 100f) + "%" + (opposite ? " < " : " >= ") + Mathf.Round(conditionAmount * 100f) + "%";
                }
                else
                {
                    text = "Invalid Condition";
                }               
            }
            else if (conditionID == ConditionID.CivFact)
            {
                if (conditionName == "Marsh Leader")
                {
                    text = "Requires Marsh Leader Status";
                }                
            }
            else if (conditionID == ConditionID.ArmyInfo)
            {
                if (conditionName == "Force Limit Percentage")
                {
                    text = "Requires " + conditionName + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount * 100f) + "%)";
                }
                else if (conditionName == "Army Size")
                {
                    text = "Requires " + conditionName + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount) + ")";
                }
            }
            else if (conditionID == ConditionID.EconInfo)
            {

                if (conditionName == "Starting Income Percentage")
                {
                    float income = civ.GetTotalIncome();
                    text = "Requires Income (" + Mathf.Round(income * 100f) / 100f + ")"  + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount * civ.startingEcon * 100f)/100f + ")";
                }
                else if (conditionName == "Starting Tiles Increase")
                {
                    float provs = civ.GetAllCivTileDatas().Count;
                    text = "Requires Number of Provinces (" + provs + ")" + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount + civ.startTiles) + ")";
                }
                else if (conditionName == "Total Income")
                {
                    float income = civ.GetTotalIncome();
                    text = "Requires Income (" + Mathf.Round(income * 100f) / 100f + ")"  + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount * 100f) / 100f + ")";
                }
                else if (conditionName == "Total Provinces")
                {
                    float provs = civ.GetAllCivTileDatas().Count;
                    text = "Requires Number of Provinces (" + provs +")" + (opposite ? " < " : " >= ") + " (" + Mathf.Round(conditionAmount) + ")";
                }
            }
        }
        return text;
    }
}
public enum ConditionID
{
    Stat,
    Government,
    Religion,
    Resource,
    HadEvent,
    ReligiousPercent,
    CivFact,
    ArmyInfo,
    EconInfo,
    Building,
    Conquest,
    Location,
}
