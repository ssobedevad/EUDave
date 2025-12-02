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
            AdminTech.GetComponent<Image>().color = civ.adminPower >= GetTechCost(nextAdminTech,civ) ? Color.green : Color.red;
            string hoverText = "It Will Cost " + GetTechCost(nextAdminTech, civ) + "<sprite index=1>\n\n";
            hoverText += "Admin Tech Cost: " + civ.techCostsA.ToString() + "\n\n";
            hoverText += "Tech Cost: " + civ.techCosts.ToString() + "\n\n";
            int monthDiff = GetAheadMonths(civ, 0);
            if (monthDiff > 0)
            {
                hoverText += "Ahead of Time: " + (monthDiff * 30) + "<sprite index=12>\n";
                hoverText += "Tech Cost: +" + (monthDiff * 0.1f) + "%\n";
            }
            if (GetAheadTime(civ,0) > 0) 
            { 
                hoverText += "Ahead of Time Bonus: Tax Efficiency +20%\n";
            }
            AdminTech.GetComponent<HoverText>().text = hoverText;
        }
        if (d)
        {
            Tech nextDiploTech = Map.main.TechD[civ.diploTech + 1];
            diploTexts[0].text = nextDiploTech.Name + " (" + (civ.diploTech + 1) + ") " + GetTechCost(nextDiploTech, civ) + "<sprite index=2>";
            SetTechDescription(nextDiploTech, diploTexts[1]);
            DiploTech.GetComponent<Image>().color = civ.diploPower >= GetTechCost(nextDiploTech, civ) ? Color.green : Color.red;
            string hoverText = "It Will Cost " + GetTechCost(nextDiploTech, civ) + "<sprite index=2>\n\n";
            hoverText += "Diplo Tech Cost: " + civ.techCostsD.ToString() + "\n\n";
            hoverText += "Tech Cost: " + civ.techCosts.ToString() + "\n\n";
            int monthDiff = GetAheadMonths(civ, 1);
            if (monthDiff > 0)
            {
                hoverText += "Ahead of Time: " + (monthDiff * 30) + "<sprite index=12>\n";
                hoverText += "Tech Cost: +" + (monthDiff * 0.1f) + "%\n";
            }
            if (GetAheadTime(civ, 1) > 0)
            {
                hoverText += "Ahead of Time Bonus: Production Value +20%\n";
            }
            DiploTech.GetComponent<HoverText>().text = hoverText;
        }
        if (m)
        {
            Tech nextMilTech = Map.main.TechM[civ.milTech + 1];
            milTexts[0].text = nextMilTech.Name + " (" + (civ.milTech + 1) + ") " + GetTechCost(nextMilTech, civ) + "<sprite index=3>";
            SetTechDescription(nextMilTech, milTexts[1]);
            MilTech.GetComponent<Image>().color = civ.milPower >= GetTechCost(nextMilTech, civ) ? Color.green : Color.red;
            string hoverText = "It Will Cost " + GetTechCost(nextMilTech, civ) + "<sprite index=2>\n\n";
            hoverText += "Mil Tech Cost: " + civ.techCostsM.ToString() + "\n\n";
            hoverText += "Tech Cost: " + civ.techCosts.ToString() + "\n\n";
            int monthDiff = GetAheadMonths(civ, 2);
            if (monthDiff > 0)
            {
                hoverText += "Ahead of Time: " + (monthDiff * 30) + "<sprite index=12>\n";
                hoverText += "Tech Cost: +" + (monthDiff * 0.1f) + "%\n";
            }
            if (GetAheadTime(civ, 2) > 0)
            {
                hoverText += "Ahead of Time Bonus: Population Growth +20%\n";
            }
            MilTech.GetComponent<HoverText>().text = hoverText;
        }       
        SetCategoryDescription(0, adminText);
        SetCategoryDescription(1, diploText);
        SetCategoryDescription(2, milText);
    }
    void SetTechDescription(Tech tech, TextMeshProUGUI text)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        text.text = "This Tech Gives:\n\n";
        foreach (var unlock in tech.unlock)
        {
            text.text += "Unlocks: " + unlock + "\n";
        }
        for (int i = 0; i < tech.effect.Length; i++)
        {
            text.text += "Effect: " + tech.effect[i] + Modifier.ToString(tech.effectStrength[i], civ.GetStat(tech.effect[i])) + "\n";
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
            text.text += modifiers[i] + Modifier.ToString(modifierValues[i], civ.GetStat(modifiers[i])) + "\n";
        }

    }
    public static int GetAheadTime(Civilisation civ, int category)
    {
        Tech current;
        if (category == 0)
        {
            if(civ.adminTech <= 3) { return 0; }
            current = Map.main.TechA[Mathf.Min(civ.adminTech+1, Map.main.TechA.Length -1)];
        }
        else if (category == 1)
        {
            if (civ.diploTech <= 3) { return 0; }
            current = Map.main.TechD[Mathf.Min(civ.diploTech + 1, Map.main.TechD.Length - 1)];
        }
        else
        {
            if (civ.milTech <= 3) { return 0; }
            current = Map.main.TechM[Mathf.Min(civ.milTech + 1, Map.main.TechM.Length - 1)];
        }
        if (Game.main.gameTime.totalTicks() < current.expectedDate.totalTicks())
        {
            return current.expectedDate.totalTicks() - Game.main.gameTime.totalTicks();
        }
        else
        {
            return 0;
        }

    }
    public static int GetAheadMonths(Civilisation civ, int category)
    {
        Tech current;
        if(category == 0)
        {
            current = Map.main.TechA[civ.adminTech];
        }
        else if(category == 1)
        {
            current = Map.main.TechD[civ.diploTech];
        }
        else
        {
            current = Map.main.TechM[civ.milTech];
        }
        if (Game.main.gameTime.totalTicks() < current.expectedDate.totalTicks())
        {
            return (current.expectedDate.totalTicks() - Game.main.gameTime.totalTicks()) / (6 * 24 * 30);           
        }
        else
        {
            return 0;
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
        cost += (int)(baseCost * (civ.techCosts.v));
        if(tech.type == 0)
        {
            cost += (int)(baseCost * (civ.techCostsA.v));
        }
        if (tech.type == 1)
        {
            cost += (int)(baseCost * (civ.techCostsD.v));
        }
        if (tech.type == 2)
        {
            cost += (int)(baseCost * (civ.techCostsM.v));
        }
        return Mathf.Max(1,cost);
    }
    public static int GetTechCostNoAhead(Tech tech, Civilisation civ)
    {
        int baseCost = 600;
        int cost = baseCost;
        cost += (int)(baseCost * (civ.techCosts.v));
        if (tech.type == 0)
        {
            cost += (int)(baseCost * (civ.techCostsA.v));
        }
        if (tech.type == 1)
        {
            cost += (int)(baseCost * (civ.techCostsD.v));
        }
        if (tech.type == 2)
        {
            cost += (int)(baseCost * (civ.techCostsM.v));
        }
        return Mathf.Max(1, cost);
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
