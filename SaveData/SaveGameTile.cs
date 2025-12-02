using MessagePack;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameTile
{
    public int i;
    public int a, b, c;
    public float co;
    public int r;
    public int il;
    public int p;
    public int ap;
    public int sm;

    public List<int> bs;
    public List<int> cs;
    public int cr;

    public List<int> rq;
    public List<int> fq;
    public List<int> mq;
    public List<int> cq;
    public int rr;
    public int fr;
    public int mr;
    public int br;

    public bool o;
    public int oi;

    public Stat dm;
    public Stat dc;
    public Stat pv;
    public Stat pq;
    public Stat te;
    public Stat gc;
    public Stat gm;
    public Stat ae;
    public Stat mc;
    public Stat cc;
    public Stat ct;
    public Stat ms;
    public Stat rc;
    public Stat rt;
    public Stat cd;
    public Stat ld;
    public Stat fm;
    public Stat pg;
    public Stat mp;
    public Stat fl;
    public Stat ad;
    public Stat lu;

    public SaveGameTile()
    {

    }
    public SaveGameTile(TileData tile)
    {
        i = tile.civID;
        a = tile.developmentA;
        b = tile.developmentB;
        c = tile.developmentC;
        co = tile.control;
        r = tile.religion;
        il = tile.infrastructureLevel;
        p = tile.population;
        ap = tile.avaliablePopulation;
        sm = tile.seperatism;

        bs = tile.buildings;
        cs = tile.cores;
        cr = tile.coreTimer;

        rq = tile.recruitQueue;
        fq = tile.boatQueue;
        mq = tile.mercenaryQueue;
        cq = tile.buildQueue;
        rr = tile.recruitTimer;
        fr = tile.boatTimer;
        mr = tile.mercenaryTimer;
        br = tile.buildTimer;

        o = tile.occupied;
        oi = tile.occupiedByID;

        dm = tile.localDevCostMod;
        dc = tile.localDevCost;
        pv = tile.localProductionValue;
        pq = tile.localProductionQuantity;
        te = tile.localTaxEfficiency;
        gc = tile.localGoverningCost;
        gm = tile.localGoverningCostMod;
        ae = tile.localAttritionForEnemies;
        mc = tile.localMinimumControl;
        cc = tile.localConstructionCost;
        ct = tile.localConstructionTime;
        ms = tile.localMovementSpeed;
        rc = tile.localRecruitmentCost;
        rt = tile.localRecruitmentTime;
        cd = tile.dailyControl;
        ld = tile.localDefensiveness;
        fm = tile.localFortMaintenance;
        pg = tile.localPopulationGrowth;
        mp = tile.localMaxPopulation;
        fl = tile.localForceLimit;
        ad = tile.localAttackerDiceRoll;
        lu = tile.localUnrest;
    }

    public void LoadToTile(TileData data)
    {
        data.civID = i;
        data.developmentA = a;
        data.developmentB = b;
        data.developmentC = c;
        data.control = co;
        data.religion = r;
        data.infrastructureLevel = il;
        data.UpdateInfrastructureModifiers();
        data.population = p;
        data.avaliablePopulation = ap;
        data.seperatism = sm;

        foreach(var buildingId in bs)
        {
            if (!data.buildings.Contains(buildingId))
            {
                Building building = Map.main.Buildings[buildingId];
                if (building.fortLevel > 0)
                {
                    data.fortLevel += building.fortLevel;
                    if (!data.hasFort)
                    {
                        data.fort = GameObject.Instantiate(Map.main.fortPrefab, data.worldPos(), Quaternion.identity, Map.main.fortTransform);
                        data.hasFort = true;
                        data.ApplyZOC();
                    }
                }
                data.buildings.Add(buildingId);
            }
        }

        data.cores = cs;
        data.coreTimer = cr;

        data.recruitQueue = rq;
        data.boatQueue = fq;
        data.mercenaryQueue = mq;
        data.buildQueue = cq;
        data.recruitTimer = rr;
        data.boatTimer = fr;
        data.mercenaryTimer = mr;
        data.buildTimer = br;

        data.occupied = o;
        data.occupiedByID = oi;

        data.localDevCostMod = dm;
        data.localDevCost = dc;
        data.localProductionValue = pv;
        data.localProductionQuantity = pq;
        data.localTaxEfficiency = te;
        data.localGoverningCost = gc;
        data.localGoverningCostMod = gm;
        data.localAttritionForEnemies = ae;
        data.localMinimumControl = mc;
        data.localConstructionCost = cc;
        data.localConstructionTime = ct;
        data.localMovementSpeed = ms;
        data.localRecruitmentCost = rc;
        data.localRecruitmentTime = rt;
        data.dailyControl = cd;
        data.localDefensiveness = ld;
        data.localFortMaintenance = fm;
        data.localPopulationGrowth = pg;
        data.localMaxPopulation = mp;
        data.localForceLimit = fl;
        data.localAttackerDiceRoll = ad;
        data.localUnrest = lu;
    }
}