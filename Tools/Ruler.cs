using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Ruler
{
    public string Name;
    public int adminSkill,diploSkill, milSkill;
    public Age age;
    public int civID;
    public bool active;
    public List<Trait> traits = new List<Trait>();
    bool isActive;
    public Ruler(int AdminSkill,int DiploSkill,int MilSkill,Age Age,int CivID,string name)
    {
        adminSkill = AdminSkill;
        diploSkill = DiploSkill;
        milSkill = MilSkill;
        age = Age;
        civID = CivID;
        active = true;
        isActive = true;
        Name = name;
        Game.main.dayTick.AddListener(CheckDeath);
    }
    public Ruler(Ruler clone)
    {
        adminSkill = clone.adminSkill;
        diploSkill = clone.diploSkill;
        milSkill = clone.milSkill;
        age = new Age(clone.age);
        civID = clone.civID;
        active = clone.active;
        isActive = clone.isActive;
        Name = clone.Name;
        traits = clone.traits;
        if (!active)
        {
            Game.main.dayTick.AddListener(CheckDeath);
            active = true;
            isActive = true;
        }
    }
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Game.main.dayTick.AddListener(CheckDeath);
        }
    }
    public static Ruler NewHeir(int BonusAdminSkill, int BonusDiploSkill, int BonusMilSkill,int CivID,int dynastyId = -1)
    {
        Civilisation civ = Game.main.civs[CivID];
        List<WeightedChoice> skillWeights = new List<WeightedChoice>();
        for (int i = 0; i < 4; i++)
        {
            skillWeights.Add(new WeightedChoice(i, 10));
        }
        int admin = WeightedChoiceManager.getChoice(skillWeights).choiceID + WeightedChoiceManager.getChoice(skillWeights).choiceID;
        int diplo = WeightedChoiceManager.getChoice(skillWeights).choiceID + WeightedChoiceManager.getChoice(skillWeights).choiceID;
        int mil = WeightedChoiceManager.getChoice(skillWeights).choiceID + WeightedChoiceManager.getChoice(skillWeights).choiceID;
        string name = GenerateName(dynastyId == -1 ? civ : Game.main.civs[dynastyId]);
        return new Ruler(Mathf.Max(0,admin + BonusAdminSkill), Mathf.Max(0, diplo +BonusDiploSkill), Mathf.Max(0, mil +BonusMilSkill),Age.zero, CivID, name);
    }
    public static string GenerateName(Civilisation civ)
    {
        string name = "";
        name += civ.government == 0 ? "King " : civ.government == 1 ? "Baron " : civ.government == 2 ? "Grand General " : civ.government == 3 ? "Tribal Leader " : "Holy Leader ";
        name += civ.civName;
        return name;
    }
    public void Kill()
    {
        if (civID > -1 && Game.main.civs[civID].ruler == this)
        {
            Game.main.civs[civID].RemoveRulerTraits(this);
        }
        age.DeActivate();
        active = false;
        if (Player.myPlayer.myCivID == civID)
        {
        }
        Game.main.dayTick.RemoveListener(CheckDeath);

    }
    void CheckDeath()
    {
        if (Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        if (age.y > 0)
        {
            if (UnityEngine.Random.Range(0f, 1000f) < (float)age.m/12f + age.y)
            {
                if (Game.main.isMultiplayer)
                {
                    if (civID > -1 && Game.main.civs[civID].ruler == this)
                    {
                        Game.main.multiplayerManager.CivActionRpc(civID, MultiplayerManager.CivActions.RulerDeath, 0);
                    }
                    else if (civID > -1 && Game.main.civs[civID].heir == this)
                    {
                        Game.main.multiplayerManager.CivActionRpc(civID, MultiplayerManager.CivActions.RulerDeath, 1);
                    }
                }
                else
                {
                    Kill();
                }
            }
        }
        if(traits.Count < Mathf.Min(3, age.y + 1))
        {
            if (Game.main.isMultiplayer)
            {
                if (civID > -1 && Game.main.civs[civID].ruler == this)
                {
                    Game.main.multiplayerManager.CivActionRpc(civID, MultiplayerManager.CivActions.RulerTrait, GetTrait());
                }
                else if (civID > -1 && Game.main.civs[civID].heir == this)
                {
                    Game.main.multiplayerManager.CivActionRpc(civID, MultiplayerManager.CivActions.HeirTrait, GetTrait());
                }
            }
            else
            {
                AddTrait(GetTrait());
            }
        }
    }
    public int GetTrait()
    {
        List<WeightedChoice> traitList = new List<WeightedChoice>();
        for (int i = 0; i < Map.main.rulerTraits.Length; i++)
        {
            Trait rulerTrait = Map.main.rulerTraits[i];
            traitList.Add(new WeightedChoice(i, traits.Exists(i => i.name == rulerTrait.name) ? 0 : 10));
        }
        return WeightedChoiceManager.getChoice(traitList).choiceID;
    }
    public void AddTrait(int trait)
    {       
        Trait chosen = Map.main.rulerTraits[trait];
        traits.Add(chosen);
        if(civID > -1 && Game.main.civs[civID].ruler == this)
        {
            Game.main.civs[civID].AddRulerTraits();
        }
    }
}
