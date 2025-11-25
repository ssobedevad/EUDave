using System;
using UnityEngine;

[Serializable]
public class Advisor
{
    public int skillLevel;
    public Age age;
    public string Name;
    public string effect;
    public float effectStrength;
    public int effectType;
    public Effect effects;
    public int civID;
    public int type;
    public bool active;
    public Sprite icon;
    public float HireCost(Civilisation civ, int increaseLevel = 0) 
    {
        float baseCost = 10 * Mathf.Pow(skillLevel + increaseLevel, 2);
        return baseCost * (1f + civ.advisorCosts.value + (type == 0? civ.advisorCostsA.value : type == 1? civ.advisorCostsD.value : civ.advisorCostsM.value));
    }
    public float Salary(Civilisation civ,int increaseLevel = 0)
    {
        float baseCost = 1 * Mathf.Pow(skillLevel + increaseLevel, 2);
        return baseCost * (1f + civ.advisorCosts.value + (type == 0 ? civ.advisorCostsA.value : type == 1 ? civ.advisorCostsD.value : civ.advisorCostsM.value));
    } 
    public Advisor(int skill,Age Age, int CivID, int Type, string Effect = "", float EffectStrength = 0f, int effetType = 0)
    {
        skillLevel = skill;
        age = Age;
        civID = CivID;
        active = true;
        effect = Effect;
        effectStrength = EffectStrength;
        type = Type;
        this.effectType = effetType;
    }
    public Advisor()
    {

    }
    public Advisor(Advisor clone)
    {
        skillLevel= clone.skillLevel;
        age = clone.age;
        civID = clone.civID;
        active = clone.active;
        effect = clone.effect;
        effectStrength = clone.effectStrength;
        type = clone.type;
        effectType = clone.effectType;
        icon = clone.icon;
    }
    void Activate()
    {
        Game.main.monthTick.AddListener(CheckDeath);       
    }
    public static Advisor NewRandomAdvisor(int type,int CivID)
    {
        Advisor advisor = new Advisor(0,Age.zero,-1,-1);
        if (type == 0)
        {
            advisor = new Advisor(Map.main.advisorsA[UnityEngine.Random.Range(0, Map.main.advisorsA.Length)]);
        }
        else if (type == 1)
        {
            advisor = new Advisor(Map.main.advisorsD[UnityEngine.Random.Range(0, Map.main.advisorsD.Length)]);
        }
        else if (type == 2)
        {
            advisor = new Advisor(Map.main.advisorsM[UnityEngine.Random.Range(0, Map.main.advisorsM.Length)]);
        }
        advisor.age = new Age(0, 0, 0, UnityEngine.Random.Range(1, 11));
        advisor.skillLevel = 1;
        int years = Game.main.gameTime.years;
        if(years > 0)
        {
            advisor.skillLevel += UnityEngine.Random.Range(0, Mathf.Min(years,5));
        }
        advisor.civID = CivID;
        advisor.active = true;
        advisor.Activate();
        return advisor;
    }
    public void Kill()
    {
        age.DeActivate();
        active = false;
        if (Player.myPlayer.myCivID == civID)
        {
            //Debug.Log("Advisor Death");
        }
        Game.main.monthTick.RemoveListener(CheckDeath);
    }
    void CheckDeath()
    {
        if(age.months > 0 || age.years > 0)
        {
            if(UnityEngine.Random.Range(0f,1000f) < age.months + age.years * 12)
            {
                Kill();
            }
        }
    }

}
