using MessagePack;
using System;
using UnityEngine;

[MessagePackObject(keyAsPropertyName: true)]
[Serializable]
public class Boat
{
    public int type;
    public int civID;
    public float hullStrengthMax;
    public float hullStrength;
    public float supplyMax;
    public float supply;
    public int cannons;
    public int sailors;
    public int maxSailors;
    public int width;
    public int flankingRange;
    public bool inBatle;
    public bool mercenary;
    public Boat()
    {

    }
    public Boat(int CivID, int Type = 0, bool merc = false)
    {
        sailors = 100;
        type = Type;
        civID = CivID;
        mercenary = merc;
        if (civID > -1)
        {
            Civilisation civ = Game.main.civs[civID];
            cannons = (int)civ.boats[type].cannons.v;
            supplyMax = civ.boats[type].supply.v;
            hullStrengthMax = civ.boats[type].hullStrength.v;
            hullStrength = hullStrengthMax;
            flankingRange = civ.boats[type].flankingRange;
            maxSailors = (int)civ.boats[type].maxSailors.v;
            width = civ.boats[type].width;
            sailors = maxSailors/2;
        }
        inBatle = false;
    }
    public Boat(Boat clone)
    {
        if (clone == null)
        {
            civID = -1;
            type = -1;
            inBatle = false;
            mercenary = false;
            sailors = 100;
            hullStrength = 0;
            hullStrengthMax = 0;
            supply = 0;
            supplyMax = 0;
            cannons = 0;
            maxSailors = 100;
        }
        else
        {
            civID = clone.civID;
            type = clone.type;
            inBatle = clone.inBatle;
            mercenary = clone.mercenary;
            sailors = clone.sailors;
            maxSailors = clone.maxSailors;
            hullStrength = clone.hullStrength;
            hullStrengthMax = clone.hullStrengthMax;
            supply = clone.supply;
            supplyMax = clone.supplyMax;
            cannons = clone.cannons;
            flankingRange = clone.flankingRange;
        }
    }
    public void TakeHullDamage(float damage)
    {      
        hullStrength = Mathf.Clamp(hullStrength - damage,0,hullStrengthMax);
        if (hullStrength <= 0)
        {
            TakeSailorDamage(maxSailors / (3 * width));
        }
    }
    public void TakeSailorDamage(int damage)
    {
        sailors = Mathf.Clamp(sailors - damage,0,maxSailors);
    }

    public void RefillSailors()
    {
        Civilisation civ = Game.main.civs[civID];
        int reinforce = (int)(20 * (1f + civ.reinforceSpeed.v));
        int targetAmount = Mathf.Min(reinforce, maxSailors - sailors);
        int realAmount = civ.RemovePopulation(targetAmount);
        //Debug.Log(targetAmount + " / " + realAmount);
        sailors += realAmount;
    }
}
