using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameCiv
{
    public float AIag;
    public float AIad;
    public float AIdp;
    public float AIml;

    public SaveGameVector3Int cpos;

    public List<SaveGameVector3Int> ccp = new List<SaveGameVector3Int>();
    public List<int> ccl = new List<int>();

    public bool ip;

    public List<SaveGameArmy> a;
    public List<SaveGameFleet> f;

    public List<int> aww;
    public List<int> awt;
    public List<int> ma;
    public List<int> al;
    public List<int> sb;
    public List<int> eh;

    public int[] rv;
    public int[] mt;
    public int[] tr;
    public bool[] ms;

    public List<General> g;

    public int oid;
    public int ap;
    public bool i;

    public float rlp;
    public float rfp;
    public float c;
    public float p;
    public float gP;
    public float at;
    public float rev;
    public int s;
    public int aP;
    public int dP;
    public int mP;

    public SaveGameRuler rul;
    public SaveGameRuler heir;
    public SaveGameAdvisor aA;
    public SaveGameAdvisor aD;
    public SaveGameAdvisor aM;
    public List<SaveGameAdvisor> asA;
    public List<SaveGameAdvisor> asM;
    public List<SaveGameAdvisor> asD;

    public int aT;
    public int dT;
    public int mT;
    public int fc;
    public int fcCD;
    public int rel;
    public int gv;
    public int gvR;

    public IdeaGroupData[] igs;

    public List<Loan> l;
    public List<int> rfs;
    public List<SaveGameVector3Int> cs;

    public Stat[] sts;
    public string[] kys;
    public Stat[] oot;

    public SaveGameCiv()
    {

    }
    public void SaveCiv(Civilisation civ)
    {
        AIag = civ.AIAggressiveness;
        AIad = civ.AIAdministrative;
        AIdp = civ.AIDiplomatic;
        AIml = civ.AIMilitary;

        cpos = new SaveGameVector3Int(civ.capitalPos);

        foreach(var controlCentre in civ.controlCentres)
        {
            ccp.Add(new SaveGameVector3Int(controlCentre.Key));
            ccl.Add(controlCentre.Value);
        }

        ip = civ.isPlayer;

        a = civ.armies.ConvertAll(i => new SaveGameArmy(i));
        f = civ.fleets.ConvertAll(i => new SaveGameFleet(i));

        aww = civ.atWarWith;
        awt = civ.atWarTogether;
        ma = civ.militaryAccess;
        al = civ.allies;
        sb = civ.subjects;
        eh = civ.eventHistory;

        rv = civ.rivals;
        mt = civ.mercTimers;
        tr = civ.truces;
        ms = civ.missionProgress;

        g = civ.generals;

        oid = civ.overlordID;
        ap = civ.annexationProgress;
        i = civ.integrating;

        rlp = civ.religiousPoints;
        rfp = civ.reformProgress;
        c = civ.coins;
        p = civ.prestige;
        gP = civ.governmentPower;
        at = civ.armyTradition;
        rev = civ.revanchism;
        s = civ.stability;
        aP = civ.adminPower;
        dP = civ.diploPower;
        mP = civ.milPower;

        rul = new SaveGameRuler(civ.ruler);
        heir = new SaveGameRuler(civ.heir);
        aA = new SaveGameAdvisor(civ.advisorA);
        aD = new SaveGameAdvisor(civ.advisorD);
        aM = new SaveGameAdvisor(civ.advisorM);
        asA = civ.advisorsA.ConvertAll(i => new SaveGameAdvisor(i));
        asM = civ.advisorsD.ConvertAll(i => new SaveGameAdvisor(i));
        asD = civ.advisorsM.ConvertAll(i => new SaveGameAdvisor(i));

        aT = civ.adminTech;
        dT = civ.diploTech;
        mT = civ.milTech;
        fc = civ.focus;
        fcCD = civ.focusCD;
        rel = civ.religion;
        gv = civ.government;
        gvR = civ.governmentRank;

        igs = civ.ideaGroups;

        l = civ.loans;
        rfs = civ.reforms;
        cs = civ.claims.ConvertAll(i => new SaveGameVector3Int(i));

        sts = new Stat[civ.stats.Count];
        kys = new string[civ.stats.Count];

        for(int i = 0; i <  civ.stats.Count; i++)
        {
            sts[i] = civ.stats.Values.ToArray()[i];
            kys[i] = civ.stats.Keys.ToArray()[i];
        }
        oot = civ.opinionOfThem;
    }
    public void LoadToCiv(Civilisation civ)
    {
        civ.AIAggressiveness = AIag;
        civ.AIAdministrative = AIad;
        civ.AIDiplomatic = AIdp;
        civ.AIMilitary = AIml;

        if (Map.main.GetTile(cpos.GetVector3Int()).civID == civ.CivID)
        {
            civ.MoveCapitalToSaveGame(cpos.GetVector3Int());
        }
        else
        {
            GameObject.Destroy(civ.capitalIndicator);
        }
        foreach (var controlcentre in civ.controlCentres) 
        {
            Map.main.GetTile(controlcentre.Key).status = 0;
            Map.main.GetTile(controlcentre.Key).UpdateStatusModifiers();
        }
        civ.controlCentres.Clear();
        if (civ.isActive())
        {
            for (int i = 0; i < ccp.Count; i++)
            {
                civ.controlCentres.Add(ccp[i].GetVector3Int(), ccl[i]);
                Map.main.GetTile(ccp[i].GetVector3Int()).status = ccl[i];
                Map.main.GetTile(ccp[i].GetVector3Int()).UpdateStatusModifiers();
            }
        }

        civ.isPlayer = ip;

        foreach(var army in civ.armies)
        {
            army.OnExitTile(army.tile);
            GameObject.Destroy(army.gameObject);
        }
        foreach (var fleet in civ.fleets)
        {
            fleet.OnExitTile(fleet.tile);
            GameObject.Destroy(fleet.gameObject);
        }
        foreach (var army in a)
        {
            if (!army.b)
            {
                army.NewArmy();
            }
        }
        foreach(var fleet in f)
        {
            if (!fleet.inBattle)
            {
                fleet.NewFleet();
            }
        }

        civ.atWarWith = aww;
        civ.atWarTogether = awt;
        civ.militaryAccess = ma;
        civ.allies = al;
        civ.subjects = sb;
        civ.eventHistory = eh;

        civ.rivals = rv;
        civ.mercTimers = mt;
        civ.truces = tr;

        try
        {
            for (int i = 0; i < ms.Length; i++)
            {
                if (civ.missionProgress.Length > i)
                {
                    civ.missionProgress[i] = ms[i];
                }
            }
        }
        catch
        {

        }

        civ.generals = g;

        civ.overlordID = oid;
        civ.annexationProgress = ap;
        civ.integrating = i;

        civ.religiousPoints = rlp;
        civ.reformProgress = rfp;
        civ.coins = c;
        civ.prestige = p;
        civ.governmentPower = gP;
        civ.armyTradition = at;
        civ.revanchism = rev;
        civ.stability = s;
        civ.adminPower = aP;
        civ.diploPower = dP;
        civ.milPower = mP;

        civ.ruler = rul.AsRuler();
        civ.heir = heir.AsRuler();
        civ.advisorA = aA.AsAdvisor();
        civ.advisorD = aD.AsAdvisor();
        civ.advisorM = aM.AsAdvisor();
        civ.advisorsA = asA.ConvertAll(i => i.AsAdvisor());
        civ.advisorsD = asM.ConvertAll(i => i.AsAdvisor());
        civ.advisorsM = asD.ConvertAll(i => i.AsAdvisor());

        civ.adminTech = aT;        
        civ.diploTech = dT;
        civ.milTech = mT;
        civ.focus = fc;
        civ.focusCD = fcCD;
        civ.religion = rel;
        civ.government = gv;
        civ.governmentRank = gvR;

        civ.unlockedIdeaGroupSlots = 0;
        civ.techUnlocks.Clear();
        civ.unlockedBuildings.Clear();
        for (int i = 0; i < civ.adminTech + 1; i++)
        {
            Map.main.TechA[i].TakeTech(civ.CivID);
        }
        for (int i = 0; i < civ.diploTech + 1; i++)
        {
            Map.main.TechD[i].TakeTech(civ.CivID);
        }
        for (int i = 0; i < civ.milTech + 1; i++)
        {
            Map.main.TechM[i].TakeTech(civ.CivID);
        }

        civ.ideaGroups = igs;

        civ.loans = l;
        civ.reforms = rfs;
        civ.claims = cs.ConvertAll(i=>i.GetVector3Int());


        for (int i = 0; i < sts.Length; i++)
        {
            Stat stat = null;
            if (civ.stats.TryGetValue(kys[i], out stat))
            {
                civ.stats[kys[i]] = sts[i];
            }
            else
            {
                civ.stats.Add(kys[i],sts[i]);
            }
        }
        civ.CacheStats();

        civ.opinionOfThem = oot;
        civ.updateBorders = true;
    }
}