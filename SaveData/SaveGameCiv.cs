using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public List<General> g;

    public int oid;
    public int ap;
    public bool i;

    public float rlp;
    public float rfp;
    public float c;
    public float p;
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

    public Stat mxs;
    public Stat psc;
    public Stat gcmx;
    public Stat gcm;
    public Stat dcmx;
    public Stat afe;
    public Stat la;
    public Stat adr;
    public Stat ddr;
    public Stat gms;
    public Stat gfs;
    public Stat grs;
    public Stat gss;
    public Stat gmvs;
    public Stat ras;
    public Stat rds;
    public Stat rms;
    public Stat rpg;
    public Stat mtt;
    public Stat dpl;
    public Stat mmx;
    public Stat mr;
    public Stat rs;
    public Stat cw;
    public Stat fs;
    public Stat dcm;
    public Stat dc;
    public Stat fd;
    public Stat fm;
    public Stat sa;
    public Stat cc;
    public Stat ct;
    public Stat pg;
    public Stat mp;
    public Stat te;
    public Stat ti;
    public Stat pv;
    public Stat pa;
    public Stat dac;
    public Stat cd;
    public Stat rmc;
    public Stat rc;
    public Stat rt;
    public Stat ms;
    public Stat fl;
    public Stat pgd;
    public Stat mpg;
    public Stat atd;
    public Stat mot;
    public Stat sc;
    public Stat crc;
    public Stat cvc;
    public Stat mxa;
    public Stat ac;
    public Stat acA;
    public Stat acD;
    public Stat acM;
    public Stat tc;
    public Stat tcA;
    public Stat tcD;
    public Stat tcM;
    public Stat ic;
    public Stat gu;
    public Stat tft;
    public Stat iit;
    public Stat wsc;
    public Stat daxc;
    public Stat dlfsd;
    public Stat bp;
    public Stat bt;
    public Stat dr;
    public Stat ldfs;
    public Stat ifs;
    public Stat ir;
    public Stat aei;
    public Stat mos;
    public Stat cr;
    public Stat ipm;
    public Stat mc;
    public Stat tv;
    public Stat tp;
    public Stat tvpc;
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

        g = civ.generals;

        oid = civ.overlordID;
        ap = civ.annexationProgress;
        i = civ.integrating;

        rlp = civ.religiousPoints;
        rfp = civ.reformProgress;
        c = civ.coins;
        p = civ.prestige;
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
        fcCD = civ.focus;
        rel = civ.religion;
        gv = civ.government;
        gvR = civ.governmentRank;

        igs = civ.ideaGroups;

        l = civ.loans;
        rfs = civ.reforms;
        cs = civ.claims.ConvertAll(i => new SaveGameVector3Int(i));

         mxs = civ.maxSettlements;
         psc = civ.promoteSettlementCost;
         gcmx = civ.governingCapacityMax;
         gcm = civ.governingCostModifier;
         dcmx = civ.diplomaticCapacityMax;
         afe = civ.attritionForEnemies;
         la = civ.landAttrition;
         adr = civ.attackerDiceRoll;
         ddr = civ.defenderDiceRoll;
         gms = civ.generalMeleeSkill;
         gfs = civ.generalFlankingSkill;
         grs = civ.generalRangedSkill;
         gss = civ.generalSiegeSkill;
         gmvs = civ.generalManeuverSkill;
         ras = civ.rulerAdminSkill;
         rds = civ.rulerDiploSkill;
         rms = civ.rulerMilSkill;
         rpg = civ.reformProgressGrowth;
         mtt = civ.militaryTactics;
         dpl = civ.discipline;
         mmx = civ.moraleMax;
         mr = civ.moraleRecovery;
         rs = civ.reinforceSpeed;
         cw = civ.combatWidth;
         fs = civ.flankingSlots;
         dcm = civ.devCostMod;
         dc = civ.devCost;
         fd = civ.fortDefence;
         fm = civ.fortMaintenance;
         sa = civ.siegeAbility;
         cc = civ.constructionCost;
         ct = civ.constructionTime;
         pg = civ.populationGrowth;
         mp = civ.maximumPopulation;
         te = civ.taxEfficiency;
         ti = civ.taxIncome;
         pv = civ.productionValue;
         pa = civ.productionAmount;
         dac = civ.dailyControl;
         cd = civ.controlDecay;
         rmc = civ.regimentMaintenanceCost;
         rc = civ.regimentCost;
         rt = civ.recruitmentTime;
         ms = civ.movementSpeed;
         fl = civ.forceLimit;
         pgd = civ.prestigeDecay;
         mpg = civ.monthlyPrestige;
         atd = civ.armyTraditionDecay;
         mot = civ.monthlyTradition;
         sc = civ.stabilityCost;
         crc = civ.coreCost;
         cvc = civ.conversionCost;
         mxa = civ.maximumAdvisors;
         ac = civ.advisorCosts;
         acA = civ.advisorCostsA;
         acD = civ.advisorCostsD;
         acM = civ.advisorCostsM;
         tc = civ.techCosts;
         tcA = civ.techCostsA;
         tcD = civ.techCostsD;
         tcM = civ.techCostsM;
         ic = civ.ideaCosts;
         gu = civ.globalUnrest;
         tft = civ.trueFaithTolerance;
         iit = civ.infidelIntolerance;
         wsc = civ.warScoreCost;
         daxc = civ.dipAnnexCost;
         dlfsd = civ.libDesireFromDevForSubjects;
         bp = civ.battlePrestige;
         bt = civ.battleTraditon;
         dr = civ.diploRep;
         ldfs = civ.libDesireInSubjects;
         ifs = civ.incomeFromSubjects;
         ir = civ.improveRelations;
         aei = civ.aggressiveExpansionImpact;
         mos = civ.monthsOfSeperatism;
         cr = civ.coringRange;
         ipm = civ.interestPerMonth;
         mc = civ.minControl;
         tv = civ.tradeValue;
         tp = civ.tradePenalty;
         tvpc = civ.tradeValPerCiv;
        oot = civ.opinionOfThem;
    }
    public void LoadToCiv(Civilisation civ)
    {
        civ.AIAggressiveness = AIag;
        civ.AIAdministrative = AIad;
        civ.AIDiplomatic = AIdp;
        civ.AIMilitary = AIml;

        civ.MoveCapitalCapitalTo(cpos.GetVector3Int());

        civ.controlCentres.Clear();
        for(int i = 0; i < ccp.Count;i++)
        {
            civ.controlCentres.Add(ccp[i].GetVector3Int(), ccl[i]);
            Map.main.GetTile(ccp[i].GetVector3Int()).status = ccl[i];
            Map.main.GetTile(ccp[i].GetVector3Int()).UpdateStatusModifiers();
        }

        civ.isPlayer = ip;

        foreach(var army in civ.armies)
        {
            army.OnExitTile();
            GameObject.Destroy(army.gameObject);
        }
        foreach (var fleet in civ.fleets)
        {
            fleet.OnExitTile();
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

        civ.generals = g;

        civ.overlordID = oid;
        civ.annexationProgress = ap;
        civ.integrating = i;

        civ.religiousPoints = rlp;
        civ.reformProgress = rfp;
        civ.coins = c;
        civ.prestige = p;
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
        civ.focusCD = fc;
        civ.religion = rel;
        civ.government = gv;
        civ.governmentRank = gvR;

        civ.ideaGroups = igs;

        civ.loans = l;
        civ.reforms = rfs;
        civ.claims = cs.ConvertAll(i=>i.GetVector3Int());

        civ.maxSettlements = mxs;
        civ.promoteSettlementCost = psc;
        civ.governingCapacityMax = gcmx;
        civ.governingCostModifier = gcm;
        civ.diplomaticCapacityMax = dcmx;
        civ.attritionForEnemies = afe;
        civ.landAttrition = la;
        civ.attackerDiceRoll = adr;
        civ.defenderDiceRoll = ddr;
        civ.generalMeleeSkill = gms;
        civ.generalFlankingSkill = gfs;
        civ.generalRangedSkill = grs;
        civ.generalSiegeSkill = gss;
        civ.generalManeuverSkill = gmvs;
        civ.rulerAdminSkill = ras;
        civ.rulerDiploSkill = rds;
        civ.rulerMilSkill = rms;
        civ.reformProgressGrowth = rpg;
        civ.militaryTactics = mtt;
        civ.discipline = dpl;
        civ.moraleMax = mmx;
        civ.moraleRecovery = mr;
        civ.reinforceSpeed = rs;
        civ.combatWidth = cw;
        civ.flankingSlots = fs;
        civ.devCostMod = dcm;
        civ.devCost = dc;
        civ.fortDefence = fd;
        civ.fortMaintenance = fm;
        civ.siegeAbility = sa;
        civ.constructionCost = cc;
        civ.constructionTime = ct;
        civ.populationGrowth = pg;
        civ.maximumPopulation = mp;
        civ.taxEfficiency = te;
        civ.taxIncome = ti;
        civ.productionValue = pv;
        civ.productionAmount = pa;
        civ.dailyControl = dac;
        civ.controlDecay = cd;
        civ.regimentMaintenanceCost = rmc;
        civ.regimentCost = rc;
        civ.recruitmentTime = rt;
        civ.movementSpeed = ms;
        civ.forceLimit = fl;
        civ.prestigeDecay = pgd;
        civ.monthlyPrestige = mpg;
        civ.armyTraditionDecay = atd;
        civ.monthlyTradition = mot;
        civ.stabilityCost = sc;
        civ.coreCost = crc;
        civ.conversionCost = cvc;
        civ.maximumAdvisors = mxa;
        civ.advisorCosts = ac;
        civ.advisorCostsA = acA;
        civ.advisorCostsD = acD;
        civ.advisorCostsM = acM;
        civ.techCosts = tc;
        civ.techCostsA = tcA;
        civ.techCostsD = tcD;
        civ.techCostsM = tcM;
        civ.ideaCosts = ic;
        civ.globalUnrest = gu;
        civ.trueFaithTolerance = tft;
        civ.infidelIntolerance = iit;
        civ.warScoreCost = wsc;
        civ.dipAnnexCost = daxc;
        civ.libDesireFromDevForSubjects = dlfsd;
        civ.battlePrestige = bp;
        civ.battleTraditon = bt;
        civ.diploRep = dr;
        civ.libDesireInSubjects = ldfs;
        civ.incomeFromSubjects = ifs;
        civ.improveRelations = ir;
        civ.aggressiveExpansionImpact = aei;
        civ.monthsOfSeperatism = mos;
        civ.coringRange = cr;
        civ.interestPerMonth = ipm;
        civ.minControl = mc;
        civ.tradeValue = tv;
        civ.tradePenalty = tp;
        civ.tradeValPerCiv = tvpc;
        civ.opinionOfThem = oot;
        civ.updateBorders = true;
    }
}