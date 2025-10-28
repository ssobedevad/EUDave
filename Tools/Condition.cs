using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Condition
{
    public string conditionName;
    public int conditionID;
    public float conditionAmount;
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
                if(civ.GetStat(conditionName).value >= conditionAmount)
                {
                    return true;
                }
            }
            else if (conditionID == ConditionID.Government)
            {
                if (civ.government == (int)conditionAmount)
                {
                    return true;
                }
            }
            else if (conditionID == ConditionID.Religion)
            {
                if (civ.religion == (int)conditionAmount)
                {
                    return true;
                }
            }
        }
        return false;
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
                text = "Requires Stat: " + civ.GetStat(conditionName).name + " >= " + conditionAmount;                
            }
            else if (conditionID == ConditionID.Government)
            {
                text = "Requires Government: " + Map.main.governmentTypes[(int)conditionAmount].name;
            }
            else if (conditionID == ConditionID.Religion)
            {
                text = "Requires Religion: " + Map.main.religions[(int)conditionAmount].name;
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
}
