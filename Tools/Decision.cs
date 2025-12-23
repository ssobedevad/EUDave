using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Decision
{
    public string name;
    public DecisionEffect effect;
    public int ID;
    public Condition[] conditions;
    public Condition[] appearConditions;

    public bool CanAppear(Civilisation civ)
    {
        foreach(var condition in appearConditions)
        {
            if(!condition.isMet(civ))
            {
                return false;
            }
        }
        return true;
    }

    public bool CanTake(Civilisation civ)
    {
        foreach (var condition in conditions)
        {
            if (!condition.isMet(civ))
            {
                return false;
            }
        }
        return true;
    }

    public void Take(Civilisation civ)
    {
        if(effect == DecisionEffect.ChangeReligion)
        {
            if(civ.religion != ID)
            {
               civ.ChangeReligion(ID,true);
            }
        }
    }

    public string GetHoverText(Civilisation civ) 
    {
        string text = "";
        foreach(var condition in conditions)
        {
            text += condition.ToString(civ) + "\n";
        }
        return text;
    }
}
public enum DecisionEffect
{
    ChangeReligion,
}
