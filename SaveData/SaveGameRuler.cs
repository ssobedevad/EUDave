using MessagePack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameRuler
{
    public string Name;
    public int adminSkill, diploSkill, milSkill;
    public Age age;
    public int civID;
    public bool active;
    public List<int> traitids = new List<int>();

    public SaveGameRuler()
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

    public SaveGameRuler(Ruler ruler)
    {
        Name = ruler.Name;
        adminSkill = ruler.adminSkill;
        diploSkill = ruler.diploSkill;
        milSkill = ruler.milSkill;
        age = ruler.age;
        civID = ruler.civID;
        active = ruler.active;
        var traits = Map.main.rulerTraits.ToList();
        traitids = ruler.traits.ConvertAll(i=> traits.FindIndex(j=>j.effects == i.effects));
    }
    public Ruler AsRuler()
    {
        Ruler ruler = new Ruler(adminSkill,diploSkill,milSkill,age,civID,Name);
        ruler.active = active;
        var traits = Map.main.rulerTraits.ToList();
        ruler.traits = traitids.ConvertAll(i => traits[i]);
        return ruler;
    }
}