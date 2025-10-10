using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Ruler
{
    public string Name;
    public int adminSkill,diploSkill, milSkill;
    public Age age;
    public int civID;
    public bool active;
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
        Game.main.monthTick.AddListener(CheckDeath);
    }
    public Ruler(Ruler clone)
    {
        adminSkill = clone.adminSkill;
        diploSkill = clone.diploSkill;
        milSkill = clone.milSkill;
        age = clone.age;
        civID = clone.civID;
        active = clone.active;
        isActive = clone.isActive;
        Name = clone.Name;
        if (!active)
        {
            Game.main.monthTick.AddListener(CheckDeath);
            active = true;
            isActive = true;
        }
    }
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            Game.main.monthTick.AddListener(CheckDeath);
        }
    }
    public static Ruler NewHeir(int BonusAdminSkill, int BonusDiploSkill, int BonusMilSkill,int CivID)
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
        return new Ruler(admin + BonusAdminSkill,diplo+BonusDiploSkill,mil+BonusMilSkill,Age.zero, CivID,civ.ruler.Name + "I");
    }
    public void Kill()
    {
        age.DeActivate();
        active = false;
        if (Player.myPlayer.myCivID == civID)
        {
            Debug.Log("Ruler Death");
        }
        Game.main.dayTick.RemoveListener(CheckDeath);

    }
    void CheckDeath()
    {
        if (age.months > 0 || age.years > 0)
        {
            if (UnityEngine.Random.Range(0f, 1000f) < age.months + age.years * 12)
            {
                Kill();
            }
        }
    }
}
