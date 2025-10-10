using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TechnologyUI : MonoBehaviour
{
    [SerializeField] Button AdminTech, DiploTech, MilTech;
    [SerializeField] TextMeshProUGUI adminText, diploText, milText;

    private void Start()
    {
        AdminTech.onClick.AddListener(delegate { TryTakeTech(0); });
        DiploTech.onClick.AddListener(delegate { TryTakeTech(1); });
        MilTech.onClick.AddListener(delegate { TryTakeTech(2); });
    }

    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        TextMeshProUGUI[] adminTexts = AdminTech.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] diploTexts = DiploTech.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] milTexts = MilTech.GetComponentsInChildren<TextMeshProUGUI>();
        bool a = true, d = true, m = true;
        if (Map.main.TechA.Length <= civ.adminTech + 1) { adminTexts[0].text = "Max"; adminTexts[1].text = "";  a = false; }
        if (Map.main.TechD.Length <= civ.diploTech + 1) { diploTexts[0].text = "Max"; diploTexts[1].text = "";d = false;  }
        if (Map.main.TechM.Length <= civ.milTech + 1) { milTexts[0].text = "Max"; milTexts[1].text = ""; m = false; }

        if (a)
        {
            Tech nextAdminTech = Map.main.TechA[civ.adminTech + 1];
            adminTexts[0].text = nextAdminTech.Name + " (" + (civ.adminTech + 1) + ") " + GetTechCost(nextAdminTech,civ) + "<sprite index=1>";
            SetTechDescription(nextAdminTech, adminTexts[1]);
        }
        if (d)
        {
            Tech nextDiploTech = Map.main.TechD[civ.diploTech + 1];
            diploTexts[0].text = nextDiploTech.Name + " (" + (civ.diploTech + 1) + ") " + GetTechCost(nextDiploTech, civ) + "<sprite index=2>";
            SetTechDescription(nextDiploTech, diploTexts[1]);
        }
        if (m)
        {
            Tech nextMilTech = Map.main.TechM[civ.milTech + 1];
            milTexts[0].text = nextMilTech.Name + " (" + (civ.milTech + 1) + ") " + GetTechCost(nextMilTech, civ) + "<sprite index=3>";
            SetTechDescription(nextMilTech, milTexts[1]);
        }       
        SetCategoryDescription(0, adminText);
        SetCategoryDescription(1, diploText);
        SetCategoryDescription(2, milText);
    }
    void SetTechDescription(Tech tech, TextMeshProUGUI text)
    {
        text.text = "This Tech Gives:\n\n";
        foreach (var unlock in tech.unlock)
        {
            text.text += "Unlocks: " + unlock + "\n";
        }
        for (int i = 0; i < tech.effect.Length; i++)
        {
            text.text += "Effect: " + tech.effect[i] + " +" + tech.effectStrength[i] + "\n";
        }
    }
    void SetCategoryDescription(int category, TextMeshProUGUI text)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        int civTech = category == 0 ? civ.adminTech : category == 1 ?civ.diploTech :civ.milTech;
        civTech++;
        List<Tech> techs = category == 0 ? Map.main.TechA.Take(civTech).ToList() : category == 1 ? Map.main.TechD.Take(civTech).ToList() : Map.main.TechM.Take(civTech).ToList();
        List<string> modifiers = new List<string>();
        List<float> modifierValues = new List<float>();
        List<string> unlocks = new List<string>();
        foreach(var tech in techs)
        {
            if(tech.effect.Length > 0)
            {
                for(int i = 0; i < tech.effect.Length;i++)
                {
                    var effect = tech.effect[i];
                    if (modifiers.Contains(effect))
                    {
                        int index = modifiers.IndexOf(effect);
                        modifierValues[index] += tech.effectStrength[i];
                    }
                    else
                    {
                        modifiers.Add(effect);
                        modifierValues.Add(tech.effectStrength[i]);
                    }
                }
            }
            if (tech.unlock.Length > 0)
            {
                for (int i = 0; i < tech.unlock.Length; i++)
                {
                    if (!unlocks.Contains(tech.unlock[i]))
                    {
                        unlocks.Add(tech.unlock[i]);
                    }
                }
            }
        }
        text.text = "This Level of Tech Gives:\n\n";
        foreach (var unlock in unlocks)
        {
            text.text += "Unlocks: " + unlock + "\n";
        }
        for (int i = 0; i < modifiers.Count; i++)
        {
            text.text += modifiers[i] + " +" + modifierValues[i] + "\n";
        }

    }
    public static int GetTechCost(Tech tech,Civilisation civ)
    {
        int baseCost = 600;
        int cost = baseCost;
        if(Game.main.gameTime.totalTicks() < tech.expectedDate.totalTicks())
        {
            float monthDiff = (tech.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);
            cost += (int)(baseCost * (monthDiff * 0.1f));
        }
        cost += (int)(baseCost * (civ.techCosts.value));
        if(tech.type == 0)
        {
            cost += (int)(baseCost * (civ.techCostsA.value));
        }
        if (tech.type == 1)
        {
            cost += (int)(baseCost * (civ.techCostsD.value));
        }
        if (tech.type == 2)
        {
            cost += (int)(baseCost * (civ.techCostsM.value));
        }
        return Mathf.Max(1,cost);
    }
    void TryTakeTech(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if(id == 0 && Map.main.TechA.Length <= civ.adminTech + 1) { return; }
        if (id == 1 && Map.main.TechD.Length <= civ.diploTech + 1) { return; }
        if (id == 2 && Map.main.TechM.Length <= civ.milTech + 1) { return; }
        Tech tech = id == 0 ? Map.main.TechA[civ.adminTech + 1] : id == 1 ? Map.main.TechD[civ.diploTech + 1] : Map.main.TechM[civ.milTech + 1];
        int resource = id == 0 ? civ.adminPower : id == 1 ? civ.diploPower : civ.milPower;
        int amount = GetTechCost(tech, civ);
        if (resource >= amount)
        {
            tech.TakeTech(civ.CivID);
            if (id == 0) { civ.adminPower -= amount; civ.adminTech++; }
            else if (id == 1) { civ.diploPower -= amount; civ.diploTech++; }
            else if (id == 2) { civ.milPower -= amount;civ.milTech++; }
        }
    }
}
