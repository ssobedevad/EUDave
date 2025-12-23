using MessagePack;
using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class Advisor
{
    public int skillLevel;
    public Age age;
    public string Name;
    public string effect;
    public float effectStrength;
    public EffectType effectType;
    public Effect effects;
    public int civID;
    public int type;
    public bool active;
    public Sprite icon;

    public float HireCost(Civilisation civ, int increaseLevel = 0) 
    {
        float baseCost = 10 * Mathf.Pow(skillLevel + increaseLevel, 2);
        return baseCost * (1f + civ.advisorCosts.v + (type == 0? civ.advisorCostsA.v : type == 1? civ.advisorCostsD.v : civ.advisorCostsM.v));
    }
    public float Salary(Civilisation civ,int increaseLevel = 0)
    {
        float baseCost = 1 * Mathf.Pow(skillLevel + increaseLevel, 2);
        return baseCost * (1f + civ.advisorCosts.v + (type == 0 ? civ.advisorCostsA.v : type == 1 ? civ.advisorCostsD.v : civ.advisorCostsM.v));
    } 
    public Advisor(int skill,Age Age, int CivID, int Type, string Effect = "", float EffectStrength = 0f, EffectType effetType = 0)
    {
        skillLevel = skill;
        age = Age;
        civID = CivID;
        active = true;
        effect = Effect;
        effectStrength = EffectStrength;
        type = Type;
        this.effectType = effetType;
        effects = new Effect();
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
        effects = clone.effects;
        type = clone.type;
        effectType = clone.effectType;
        icon = clone.icon;
    }
    public void Activate()
    {
        Game.main.monthTick.AddListener(CheckDeath);       
    }
    public static Advisor NewRandomAdvisor(int type,int CivID,ref int index)
    {
        Advisor advisor = new Advisor(0,Age.zero,-1,-1);
        if (type == 0)
        {
            index = UnityEngine.Random.Range(0, Map.main.advisorsA.Length);
            advisor = new Advisor(Map.main.advisorsA[index]);
        }
        else if (type == 1)
        {
            index = UnityEngine.Random.Range(0, Map.main.advisorsD.Length);
            advisor = new Advisor(Map.main.advisorsD[index]);
        }
        else if (type == 2)
        {
            index = UnityEngine.Random.Range(0, Map.main.advisorsM.Length);
            advisor = new Advisor(Map.main.advisorsM[index]);
        }
        advisor.age = new Age(0, 0, 0, UnityEngine.Random.Range(1, 11));
        advisor.skillLevel = 1;
        int years = Game.main.gameTime.y;
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
        }
        Game.main.monthTick.RemoveListener(CheckDeath);
    }
    void CheckDeath()
    {
        if(Game.main.isMultiplayer && !NetworkManager.Singleton.IsServer) { return; }
        if(age.m > 0 || age.y > 0)
        {
            if(UnityEngine.Random.Range(0f,1000f) < age.m + age.y * 12)
            {
                Kill();
            }
        }
    }

}
