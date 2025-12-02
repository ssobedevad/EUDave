using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Condition
{
    public string conditionName;
    public int conditionID;
    public float conditionAmount;
    public bool opposite;
    public bool province;
    public Vector3Int provincePos;

    public bool isMet(Civilisation civ)
    {
        if (province)
        {
            TileData prov = Map.main.GetTile(provincePos);
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
        }
        return opposite ? true : false;
    }    
    public string ToString(Civilisation civ)
    {
        string text = "";
        if (province)
        {
            TileData prov = Map.main.GetTile(provincePos);
        }
        else
        {
            if (conditionID == ConditionID.Stat)
            {
                text = "Requires Stat: " + conditionName + " >= " + conditionAmount;                
            }
            else if (conditionID == ConditionID.Government)
            {
                text = "Requires Government: " + Map.main.governmentTypes[(int)conditionAmount].name;
            }
            else if (conditionID == ConditionID.Religion)
            {
                text = "Requires Religion: " + Map.main.religions[(int)conditionAmount].name;
            }
            else if (conditionID == ConditionID.Resource)
            {
                text = "Requires Resource: " + conditionName + " (" + Mathf.Round(conditionAmount * 100f)/100f + ")";
            }
            else if (conditionID == ConditionID.HadEvent)
            {
                text = "Requires Event: " + Map.main.pulseEvents[(int)conditionAmount].Name;
            }
        }
        return text;
    }
}
public class ConditionID
{
    public static int Stat = 0;
    public static int Government = 1;
    public static int Religion = 2;
    public static int Resource = 3;
    public static int HadEvent = 4;
}
