using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class BattleLine
{
    public int width = 15;
    public Regiment[] regiments = new Regiment[15];

    public BattleLine(int combatWidth = 15)
    {
        regiments = new Regiment[combatWidth];
    }
    public BattleLine(BattleLine line)
    {
        width = line.width;
        regiments = new Regiment[width];
        for(int i = 0; i < width;i++)
        {
            regiments[i] = new Regiment(line.regiments[i]);
        }
    }
    public BattleLine(List<Regiment> newRegiments,int combatWidth)
    {
        width = combatWidth;
        regiments = new Regiment[width];
        newRegiments.Sort((x,y) => x.size.CompareTo(y.size));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            int index = i;
            if(j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + i - 1;
            regiments[index] = newRegiments[j];
        }
    }
    public BattleLine(List<Army> newArmies, int combatWidth)
    {
        List<Regiment> newRegiments = new List<Regiment>();
        foreach (var army in newArmies)
        {
            newRegiments.AddRange(army.regiments);
        }
        width = combatWidth;
        regiments = new Regiment[width];
        newRegiments.Sort((x, y) => x.size.CompareTo(y.size));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            int index = i;
            if (j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + index - 1;
            regiments[index] = newRegiments[j];
        }
    }
    public List<Regiment> RefillRegiments(List<Army> newArmies, List<Regiment> dead, int targetWidth = -1)
    {
        List<Regiment> newRegiments = new List<Regiment>();
        foreach (var army in newArmies)
        {
            newRegiments.AddRange(army.regiments);
        }
        return RefillRegimentsFront(newRegiments,dead, targetWidth);
    }
    public int GetUsedWidth()
    {
        int count = 0;
        for(int i = 0; i < width; i++)
        {
            if (regiments[i] != null)
            {
                count++;
            }
        }
        return count;
    }
    public List<Regiment> RefillRegimentsFront(List<Regiment> newRegiments,List<Regiment> dead,int targetWidth = -1)
    {
        RecentreRegiments();
        int indent = 0;
        int bonusIndentLeft = 0;
        if(targetWidth > -1)
        {
            indent = (int)Mathf.Floor((width - targetWidth)/2f);
            bonusIndentLeft = targetWidth % 2 == 0 ? 1 : 0;
        }
        List<Regiment> used = new List<Regiment>();
        newRegiments.Sort((x, y) => (y.size * y.morale).CompareTo(x.size * x.morale));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            List<Regiment> newInfantry = newRegiments.FindAll(i => i.type == 0);
            List<Regiment> newCavalrly = newRegiments.FindAll(i => i.type == 1);
            int index = i;
            if (j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + index - 1;
            Regiment next = GetNext(newInfantry, newCavalrly);
            if (index < indent + bonusIndentLeft || index >= width - indent)
            {
                int distToBattle = index < indent + bonusIndentLeft ? (indent + bonusIndentLeft) - index : index - (width - indent) + 1;
                if (next != null && next.flankingRange < distToBattle)
                {
                    next = GetNext(newCavalrly, newInfantry);
                    if (next != null && next.flankingRange < distToBattle) { next = null; }
                }
            }
            if ((regiments[index] == null || regiments[index].size <= 0 || regiments[index].morale <= 0))
            {
                if (regiments[index] != null && (regiments[index].size <= 0 || regiments[index].morale <= 0))
                {
                    dead.Add(regiments[index]);
                }
                if (next != null)
                {
                    regiments[index] = next;
                    used.Add(next);
                    newRegiments.Remove(next);
                }
                else
                {
                    regiments[index] = null;
                }
            }
        }
        return used;
    }
    public List<Regiment> RefillRegimentsBack(List<Regiment> newRegiments, List<Regiment> dead, int targetWidth = -1)
    {
        RecentreRegiments();
        int indent = 0;
        int bonusIndentLeft = 0;
        if (targetWidth > -1)
        {
            indent = (int)Mathf.Floor((width - targetWidth) / 2f);
            bonusIndentLeft = targetWidth % 2 == 0 ? 1 : 0;
        }
        List<Regiment> used = new List<Regiment>();
        newRegiments.Sort((x, y) => (y.size * y.morale).CompareTo(x.size * x.morale));
        List<Regiment> newArtillery = newRegiments.FindAll(i => i.type == 2);
        if(newArtillery.Count == 0) { return used; }
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {            
            int index = i;
            if (j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + index - 1;
            Regiment next = newArtillery.First();
            if (index < indent + bonusIndentLeft || index >= width - indent)
            {
                int distToBattle = index < indent + bonusIndentLeft ? (indent + bonusIndentLeft) - index : index - (width - indent) + 1;
            }
            if ((regiments[index] == null || regiments[index].size <= 0 || regiments[index].morale <= 0))
            {
                if (regiments[index] != null && (regiments[index].size <= 0 || regiments[index].morale <= 0))
                {
                    dead.Add(regiments[index]);
                }
                if (next != null)
                {
                    regiments[index] = next;
                    used.Add(next);
                    newRegiments.Remove(next);
                }
                else
                {
                    regiments[index] = null;
                }
            }
        }
        return used;
    }
    Regiment GetNext(List<Regiment> firstPrio, List<Regiment> secondPrio) 
    {
        return firstPrio.Count > 0 ? firstPrio.First() : secondPrio.Count > 0 ? secondPrio.First() : null;
    }
    public void RecentreRegiments()
    {
        List<Regiment> regimentNew = regiments.ToList();
        regimentNew.RemoveAll(i => i == null);
        bool left = regimentNew.Count % 2 == 0;
        while (regimentNew.Count < width)
        {
            if (left)
            {
                regimentNew.Insert(0, null);
            }
            else
            {
                regimentNew.Add(null);
            }
            left = !left;
        }
        regiments = regimentNew.ToArray();
    }
}
