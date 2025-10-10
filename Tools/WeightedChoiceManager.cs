using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WeightedChoiceManager
{
    public static WeightedChoice getChoice(List<WeightedChoice> list)
    {
        int TotalWeight = 0;
        list.ForEach(item => TotalWeight += item.weight);
        int choice = UnityEngine.Random.Range(0, TotalWeight);
        WeightedChoice selected = new WeightedChoice(-1,0);
        foreach (WeightedChoice item in list)
        {
            if(choice < item.weight)
            {
                selected = item;
                break;
            }
            choice -= item.weight;
        }
        return selected;
    }

    public static List<T> Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0,n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }
}
