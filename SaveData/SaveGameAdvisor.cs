using MessagePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameAdvisor
{
    public int s;
    public Age a;
    public int t;
    public int i;
    public bool v;

    public SaveGameAdvisor()
    {

    }
    List<Advisor> advisorList(int type)
    {
        switch(type)
        {
            case 0:
                return Map.main.advisorsA.ToList();
            case 1:
                return Map.main.advisorsD.ToList();
            case 2:
                return Map.main.advisorsM.ToList();
            default:
                return null;
        }
    }

    public SaveGameAdvisor(Advisor advisor)
    {
        s = advisor.skillLevel;
        a = advisor.age;
        t = advisor.type;
        v = advisor.active;
        i = advisorList(t).FindIndex(i=>i.effect == advisor.effect);
    }
    public Advisor AsAdvisor()
    {
        if(i == -1) { return new Advisor(); }
        Advisor advisor = new Advisor(advisorList(t)[i]);
        advisor.skillLevel = s;
        advisor.age = a;
        advisor.type = t;
        advisor.active = v;
        return advisor;
    }
}