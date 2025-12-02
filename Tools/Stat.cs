using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class Stat
{
    public float bs;
    public float v;  
    public List<Modifier> ms = new List<Modifier>();
    public bool ta;
    public bool f;
    public float d = 1f;
    public Stat(float BaseStat,string Name,bool IsFlat = false)
    {
        bs = BaseStat;
        v = bs;       
        f = IsFlat;
    }
    public Stat()
    {

    }
    public override string ToString()
    {
        List<Modifier> bases = ms.FindAll(i => i.t == ModifierType.Base && i.v != 0);
        List<Modifier> additive = ms.FindAll(i => i.t == ModifierType.Additive && i.v != 0);
        List<Modifier> flat = ms.FindAll(i => i.t == ModifierType.Flat && i.v != 0);
        List<Modifier> multiplicative = ms.FindAll(i => i.t == ModifierType.Multiplicative && i.v != 0);
        float totalB = 0f;
        if (bases.Count > 0)
        {
            
            foreach (Modifier modifier in bases)
            {
                totalB += modifier.v;
                
            }
        }
        float baseStatNew = bs + totalB;
        string Start = baseStatNew > 0 ? "Base: " + baseStatNew + "\n" : "";
        if (additive.Count > 0)
        {
            float total = 0f;
            foreach(Modifier modifier in additive)
            {
                total += modifier.v;
                Start += modifier.n + ":" + Modifier.ToString(modifier.v, this,true) + "\n";
            }
        }
        if (flat.Count > 0)
        {
            float total = 0f;
            foreach (Modifier modifier in flat)
            {
                total += modifier.v;
                Start += modifier.n + ":" + Modifier.ToString(modifier.v,this) + "\n";                
            }
        }
        if (multiplicative.Count > 0)
        {
            Start += "Multiplicative: \n";
            float total = 1f;
            foreach (Modifier modifier in multiplicative)
            {
                total *= modifier.v;
                Start += modifier.n + ":" + Modifier.ToString(modifier.v, this, true) + "\n";
            }
        }
        if(Start.Length == 0) { return "None"; }
        return Start;
    }
    public void SetValue()
    {
        v = bs;
        float Add = 0f;
        float Flat = 0f;
        float Mult = 1f;
        float baseBonus = 0f;
        foreach (var modifier in ms)
        {
            if (modifier.t == ModifierType.Base)
            {
                baseBonus += modifier.v;
            }
            else if (modifier.t == ModifierType.Flat)
            {
                Flat += modifier.v;
            }
            else if (modifier.t == ModifierType.Additive)
            {
                Add += modifier.v;
            }
            else if (modifier.t == ModifierType.Multiplicative)
            {
                Mult += modifier.v;
            }
        }
        v += baseBonus;
        v += Add * (bs + baseBonus) + Flat;
        v *= Mult;
    }
    public void ChangeBaseStat(float NewStat)
    {
        bs = NewStat;
        SetValue();
    }
    public void AddModifier(Modifier modifier)
    {
        if (!ms.Exists(i => i.n == modifier.n))
        {
            ms.Add(modifier);
            SetValue();
            if ((modifier.d != -1 || modifier.dc) && !ta)
            {
                Game.main.tenMinTick.AddListener(TickTempModifiers);
                ta = true;
            }
        }
        else
        {
            //Debug.Log("Modifier with name " + modifier.name + " already exists on stat " + name);
        }
    }
    public void IncreaseModifier(string name, float byAmount, int type = 0,bool Decay = true)
    {
        Modifier modifier;
        if (ms.Exists(i => i.n == name))
        {
            modifier = ms.Find(i => i.n == name);
            modifier.v += byAmount;
        }
        else
        {
            modifier = new Modifier(byAmount, type, name, decay: Decay);
            AddModifier(modifier);
            if ((modifier.d != -1 || modifier.dc) && !ta)
            {
                Game.main.tenMinTick.AddListener(TickTempModifiers);
                ta = true;
            }
        }
        SetValue();
    }
    void TickTempModifiers()
    {
        bool hasTemp = false;
        foreach(var modifier in ms.ToList())
        {
            if(modifier.d > 0)
            {
                modifier.d--;
                if(modifier.d == 0)
                {
                    RemoveModifier(modifier);
                }
                else
                {
                    hasTemp = true;
                }
            }
            if (modifier.dc)
            {
                if(modifier.v > 0)
                {
                    modifier.v -= 1f / 1440f;
                    if(modifier.v < 0)
                    {
                        RemoveModifier(modifier);
                    }
                }
                else if (modifier.v < 0)
                {
                    modifier.v += d / 1440f;
                    if (modifier.v > 0)
                    {
                        RemoveModifier(modifier);
                    }
                }
                hasTemp = true;
            }
        }
        if (!hasTemp)
        {
            Game.main.tenMinTick.RemoveListener(TickTempModifiers);
            ta = false;
        }
    }
    public void RemoveModifier(Modifier modifier)
    {
        ms.Remove(modifier);
        SetValue();
    }
    public void TryRemoveModifier(string modifierName)
    {
        Modifier modifier = ms.Find(i => i.n == modifierName);
        if (modifier != null)
        {
            ms.Remove(modifier);
            SetValue();
        }
    }
    public void UpdateModifier(string name,  float value, int type = 0)
    {
        Modifier modifier = ms.Find(i => i.n == name);
        try
        {
            modifier.v = value;
        }
        catch
        {
            modifier = new Modifier(value, type, name);
            AddModifier(modifier);           
        }
        SetValue();
    }
    public void UpdateModifierDuration(string name,float value, int duration, int type = 0)
    {
        Modifier modifier;
        if (ms.Exists(i => i.n == name))
        {
            modifier = ms.Find(i => i.n == name);
            modifier.d = duration;
            modifier.v = value;
        }
        else
        {
            modifier = new Modifier(value, type, name, duration);
            AddModifier(modifier);
            if ((modifier.d != -1 || modifier.dc) && !ta)
            {
                Game.main.tenMinTick.AddListener(TickTempModifiers);
                ta = true;
            }
        }
        SetValue();
    }
}
