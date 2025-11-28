using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[Serializable]
public class FleetBattleLine
{
    public int width = 31;
    public BoatData[] boats = new BoatData[31];

    public FleetBattleLine(int combatWidth = 31)
    {
        boats = new BoatData[combatWidth];
        width = combatWidth;
    }
    public FleetBattleLine(FleetBattleLine line)
    {
        width = line.width;
        boats = new BoatData[width];
        for(int i = 0; i < width;i++)
        {
            boats[i] = new BoatData(line.boats[i]);
        }
    }
    public List<Boat> GetBoats()
    {
        List<Boat> currentBoats = new List<Boat>();
        foreach (var boatData in boats)
        {
            if(boatData != null && boatData.segmentID == 0)
            {
                currentBoats.Add(boatData.boat);
            }
        }
        return currentBoats;
    }
    public FleetBattleLine(List<Boat> newboats,int combatWidth)
    {
        width = combatWidth;
        boats = new BoatData[width];
        newboats.Sort((x,y) => x.cannons.CompareTo(y.cannons));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            int index = i;
            if(j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + i - 1;
            if (CanPlaceBoat(newboats[0], index))
            {
                PlaceBoat(newboats[0], index);
                newboats.RemoveAt(0);
            }
        }
    }
    public FleetBattleLine(List<Fleet> newFleets, int combatWidth)
    {
        List<Boat> newboats = new List<Boat>();
        foreach (var fleet in newFleets)
        {
            newboats.AddRange(fleet.boats);
        }
        width = combatWidth;
        boats = new BoatData[width];
        newboats.Sort((x, y) => x.cannons.CompareTo(y.cannons));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            int index = i;
            if (j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + index - 1;
            if (CanPlaceBoat(newboats[0], index))
            {
                PlaceBoat(newboats[0], index);
                newboats.RemoveAt(0);
            }
        }
    }
    public List<Boat> Refillboats(List<Fleet> newFleets, List<Boat> dead, int targetWidth = -1)
    {
        List<Boat> newboats = new List<Boat>();
        foreach (var fleet in newFleets)
        {
            newboats.AddRange(fleet.boats);
        }
        return RefillboatsFront(newboats,dead, targetWidth);
    }
    public int GetUsedWidth()
    {
        int count = 0;
        for(int i = 0; i < width; i++)
        {
            if (boats[i] != null)
            {
                count++;
            }
        }
        return count;
    }
    public List<Boat> RefillboatsFront(List<Boat> newboats,List<Boat> dead,int targetWidth = -1)
    {
        Recentreboats();
        int indent = 0;
        int bonusIndentLeft = 0;
        if(targetWidth > -1)
        {
            indent = (int)Mathf.Floor((width - targetWidth)/2f);
            bonusIndentLeft = targetWidth % 2 == 0 ? 1 : 0;
        }
        List<Boat> used = new List<Boat>();
        newboats.Sort((x, y) => (y.sailors * y.hullStrength).CompareTo(x.sailors * x.hullStrength));
        int i = 0;
        int centre = (width + 1) / 2;
        for (int j = 0; j < width; j++)
        {
            List<Boat> newTransports = newboats.FindAll(i => i.type == 0);
            List<Boat> newSupplyShips = newboats.FindAll(i => i.type == 1);
            List<Boat> newWarships = newboats.FindAll(i => i.type == 2);
            int index = i;
            if (j % 2 == 0) { index *= -1; }
            else { i++; index = i; }
            index = centre + index - 1;
            Boat next = GetNext(newWarships, newSupplyShips, newTransports);
            if (index < indent + bonusIndentLeft || index >= width - indent)
            {
                int distToBattle = index < indent + bonusIndentLeft ? (indent + bonusIndentLeft) - index : index - (width - indent) + 1;
                if (next != null && next.flankingRange < distToBattle)
                {
                    next = GetNext(newWarships, newSupplyShips, newTransports);
                    if (next != null && next.flankingRange < distToBattle) { next = null; }
                }
            }
            if ((boats[index] == null || boats[index].boat.sailors <= 0 || boats[index].boat.hullStrength <= 0))
            {
                if (boats[index] != null && (boats[index].boat.sailors <= 0 || boats[index].boat.hullStrength <= 0))
                {
                    dead.Add(boats[index].boat);
                    RemoveBoat(index);
                }
                if (next != null)
                {
                    if (CanPlaceBoat(next, index))
                    {
                        PlaceBoat(next, index);
                        used.Add(next);
                        newboats.Remove(next);
                    }
                }
                else
                {
                    boats[index] = null;
                }
            }
        }
        return used;
    }    
    Boat GetNext(List<Boat> firstPrio, List<Boat> secondPrio, List<Boat> thirdPrio) 
    {
        return firstPrio.Count > 0 ? firstPrio[0] : secondPrio.Count > 0 ? secondPrio[0] : thirdPrio.Count > 0? thirdPrio[0] : null;
    }
    public void Recentreboats()
    {
        List<BoatData> regimentNew = boats.ToList();
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
        boats = regimentNew.ToArray();
    }
    bool CanPlaceBoat(Boat boat,int pos)
    {
        if (boats[pos] != null) { return false; }
        int w = boat.width;
        if(w + pos >= boats.Length|| w <= 0) { return false; }
        for (int i = 1; i < w; i++)
        {
            if (boats[pos + i] != null) { return false; }
        }
        return true;
    }
    void PlaceBoat(Boat boat, int pos)
    {
        int w = boat.width;
        float cannonMod = (float)boat.cannons / (float)w;
        float cannonTotal = 0;
        int cannonsInPlace = 0;
        for (int i = 0; i < w; i++)
        {
            cannonTotal += cannonMod;
            boats[pos + i] = new BoatData(boat, (cannonTotal >= 1 || (cannonsInPlace < boat.cannons && i == w-1)) ? (int)cannonTotal : 0, i,boat.hullStrength/(float)w,boat.hullStrengthMax/(float)w);
            if(cannonTotal > 1)
            {
                cannonsInPlace += (int)cannonTotal;
                cannonTotal -= (int)cannonTotal;
            }
        }
    }
    void RemoveBoat(int pos)
    {
        BoatData boatData = boats[pos];
        int w = boatData.boat.width;
        for (int i = 0; i < w; i++)
        {
            boats[pos + i - boatData.segmentID] = null;
        }
    }
    [Serializable]
    public class BoatData
    {
        public Boat boat;
        public int cannons;
        public float hullStrength;
        public float hullStrengthMax;
        public int segmentID;

        public BoatData(Boat boat,int cannons,int segmentID,float hullStrength,float hullStrengthMax)
        { 
            this.boat = boat;
            this.cannons = cannons;
            this.segmentID = segmentID;
            this.hullStrength = hullStrength;
            this.hullStrengthMax = hullStrengthMax;
        }

        public BoatData(BoatData boatData)
        {
            if (boatData != null)
            {
                this.boat = boatData.boat;
                this.cannons = boatData.cannons;
                this.segmentID = boatData.segmentID;
                this.hullStrength = boatData.hullStrength;
                this.hullStrengthMax = boatData.hullStrengthMax;
            }
        }
    }
}
