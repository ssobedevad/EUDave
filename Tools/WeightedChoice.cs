using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WeightedChoice
{
    public int choiceID;
    public string choiceName;
    public int weight; 

    public WeightedChoice(int ChoiceID  = 0, int Weight = 0,string ChoiceName = "")
    {
        choiceID = ChoiceID;
        weight = Weight;
        choiceName = ChoiceName;
    }

}
