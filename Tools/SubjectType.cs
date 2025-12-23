using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class SubjectType
{
    public string SubjectTypeName;
    public float DiplomaticCapacityFlat;
    public float DiplomaticCapacityFromGoverningCapacity;
    public float IncomePercentage;
    public float LibertyDesireFlat;
    public float LibertyDesireFromDevelopment;
    public float DiploAnnexCostModifier;
    public bool CountsOtherSubjectArmies;
    public bool CountsOtherSubjectsEconomy;
    public bool CountsOwnArmies;
    public bool CountsOwnEconomy;
    public Effect[] SubjectEffects;
    public Effect[] OverlordEffects;
    public int[] possiblePromotions;

    public string GetDescription(Civilisation civ)
    {
        string desc = "Subject Type: " + SubjectTypeName + "\n\n";
        if (DiplomaticCapacityFlat > 0 || DiplomaticCapacityFromGoverningCapacity > 0)
        {
            desc += "Diplomatic Capacity: ";
        }
        else
        {
            desc += "Uses No Diplomatic Capacity\n";
        }
        if (DiplomaticCapacityFlat > 0)
        {
            desc += Mathf.Round(DiplomaticCapacityFlat) + (DiplomaticCapacityFromGoverningCapacity > 0? " + " : "\n\n");
        }
        if (DiplomaticCapacityFromGoverningCapacity > 0)
        {
            desc += Mathf.Round(DiplomaticCapacityFromGoverningCapacity * 100f) + "% of Governing Capacity\n\n";
        }
        if(IncomePercentage > 0)
        {
            desc += "Overlord Taxes: " +Mathf.Round(IncomePercentage * 100f) + "% of Total Income\n\n";
        }
        if (LibertyDesireFlat > 0)
        {
            desc += "Liberty Desire: " + Mathf.Round(LibertyDesireFlat * 100f) + "% Bonus\n";
        }
        if (LibertyDesireFromDevelopment > 0)
        {
            desc += "Liberty Desire From Development Multiplier: " + Mathf.Round(LibertyDesireFromDevelopment * 100f) + "%\n\n";
        }
        if (DiploAnnexCostModifier > 0)
        {
            desc += "Diplomatic Annexation Cost Multiplier: " + Mathf.Round(DiploAnnexCostModifier * 100f) + "%\n\n";
        }
        if (CountsOwnArmies)
        {
            desc += "Liberty Desire Includes Own Armies\n";
        }
        if (CountsOwnEconomy)
        {
            desc += "Liberty Desire Includes Own Economy\n";
        }
        if (CountsOtherSubjectArmies)
        {
            desc += "Liberty Desire Includes Other Subject Armies\n";
        }
        if (CountsOtherSubjectsEconomy)
        {
            desc += "Liberty Desire Includes Other Subject Economy\n";
        }
        if (SubjectEffects.Length > 0)
        {
            desc += "\nThe Following Effects On The Subject:\n";
            foreach (var effect in SubjectEffects)
            {
                desc += effect.GetHoverText(civ);
            }
        }
        if (OverlordEffects.Length > 0)
        {
            desc += "\nThe Following Effects On The Overlord:\n";
            foreach (var effect in OverlordEffects)
            {
                desc += effect.GetHoverText(civ);
            }
        }
        return desc;
    }
}
