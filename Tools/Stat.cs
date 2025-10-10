using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stat
{
    public float baseStat;
    public float value;
    public string name;
    public List<Modifier> modifiers = new List<Modifier>();
    bool tempActive;
    public bool isFlat;
    public Stat(float BaseStat,string Name,bool IsFlat = false)
    {
        baseStat = BaseStat;
        name = Name;
        value = baseStat;       
        isFlat = IsFlat;
    }
    public override string ToString()
    {
        List<Modifier> bases = modifiers.FindAll(i => i.type == ModifierType.Base && i.value != 0);
        List<Modifier> additive = modifiers.FindAll(i => i.type == ModifierType.Additive && i.value != 0);
        List<Modifier> flat = modifiers.FindAll(i => i.type == ModifierType.Flat && i.value != 0);
        List<Modifier> multiplicative = modifiers.FindAll(i => i.type == ModifierType.Multiplicative && i.value != 0);
        float totalB = 0f;
        if (bases.Count > 0)
        {
            
            foreach (Modifier modifier in bases)
            {
                totalB += modifier.value;
                
            }
        }
        float baseStatNew = baseStat + totalB;
        string Start = baseStatNew > 0 ? "Base: " + baseStatNew + "\n" : "";
        if (additive.Count > 0)
        {
            float total = 0f;
            foreach(Modifier modifier in additive)
            {
                total += modifier.value;
                Start += modifier.name + ":" + Modifier.ToString(modifier.value, this,true) + "\n";
            }
        }
        if (flat.Count > 0)
        {
            float total = 0f;
            foreach (Modifier modifier in flat)
            {
                total += modifier.value;
                Start += modifier.name + ":" + Modifier.ToString(modifier.value,this) + "\n";                
            }
        }
        if (multiplicative.Count > 0)
        {
            Start += "Multiplicative: \n";
            float total = 1f;
            foreach (Modifier modifier in multiplicative)
            {
                total *= modifier.value;
                Start += modifier.name + ":" + Modifier.ToString(modifier.value, this, true) + "\n";
            }
        }
        if(Start.Length == 0) { return "None"; }
        return Start;
    }
    public void SetValue()
    {
        value = baseStat;
        float Add = 0f;
        float Flat = 0f;
        float Mult = 1f;
        float baseBonus = 0f;
        foreach (var modifier in modifiers)
        {
            if (modifier.type == ModifierType.Base)
            {
                baseBonus += modifier.value;
            }
            else if (modifier.type == ModifierType.Flat)
            {
                Flat += modifier.value;
            }
            else if (modifier.type == ModifierType.Additive)
            {
                Add += modifier.value;
            }
            else if (modifier.type == ModifierType.Multiplicative)
            {
                Mult *= modifier.value;
            }
        }
        value += baseBonus;
        value += Add * (baseStat + baseBonus) + Flat;
        value *= Mult;
    }
    public void ChangeBaseStat(float NewStat)
    {
        baseStat = NewStat;
        SetValue();
    }
    public void AddModifier(Modifier modifier)
    {
        modifiers.Add(modifier);
        SetValue();
        if(modifier.duration != -1 && !tempActive)
        {
            Game.main.tenMinTick.AddListener(TickTempModifiers);
            tempActive = true;
        }
    }
    void TickTempModifiers()
    {
        bool hasTemp = false;
        foreach(var modifier in modifiers.ToList())
        {
            if(modifier.duration > 0)
            {
                modifier.duration--;
                if(modifier.duration == 0)
                {
                    RemoveModifier(modifier);
                }
                else
                {
                    hasTemp = true;
                }
            }
        }
        if (!hasTemp)
        {
            Game.main.tenMinTick.RemoveListener(TickTempModifiers);
            tempActive = false;
        }
    }
    public void RemoveModifier(Modifier modifier)
    {
        modifiers.Remove(modifier);
        SetValue();
    }
    public void TryRemoveModifier(string modifierName)
    {
        Modifier modifier = modifiers.Find(i => i.name == modifierName);
        if (modifier != null)
        {
            modifiers.Remove(modifier);
            SetValue();
        }
    }
    public void UpdateModifier(string name,  float value, int type = 0)
    {
        Modifier modifier;
        if(modifiers.Exists(i=>i.name == name))
        {
            modifier = modifiers.Find(i => i.name == name);
            modifier.value = value;
        }
        else
        {
            modifier = new Modifier(value, type,name);
            AddModifier(modifier);
        }
        SetValue();
    }
}
