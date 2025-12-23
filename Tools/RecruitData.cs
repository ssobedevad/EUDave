using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class RecruitData
{
    public int unitID;
    public UnitTypeID unitType;
    
    public string GetUnitName(Civilisation civ)
    {
        try
        {
            if (unitType == UnitTypeID.Regiment)
            {
                return civ.units[unitID].name;
            }
            else if (unitType == UnitTypeID.Boat)
            {
                return civ.boats[unitID].name;
            }
            else if (unitType == UnitTypeID.Mercenary)
            {
                return Map.main.mercenaries[unitID].Name;
            }
            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }

    public RecruitData() 
    {
        
    }

    public RecruitData(int unitID,UnitTypeID type)
    {
        this.unitID = unitID;
        this.unitType = type;
    }

}
public enum UnitTypeID
{
    Regiment,
    Boat,
    Mercenary,
}
