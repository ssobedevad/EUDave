using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;

using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    [SerializeField] Image color;
    [SerializeField] TextMeshProUGUI civName;
    [SerializeField] TextMeshProUGUI coins, manpower, admin, diplo, mil;
    [SerializeField] TextMeshProUGUI prestige,stability;
    [SerializeField] TextMeshProUGUI Age;
    [SerializeField] Button openCivMenu;

    private void Start()
    {
        openCivMenu.onClick.AddListener(OpenCivMenu);
        coins.GetComponent<Button>().onClick.AddListener(OpenEcon);
    }
    void OpenCivMenu()
    {
        UIManager.main.CivUI.SetActive(!UIManager.main.CivUI.activeSelf);
        Map.main.tileMapManager.DeselectTile();
        Player.myPlayer.selectedArmies.Clear();
        
    }
    void OpenEcon()
    {
        UIManager.main.CivUI.SetActive(!UIManager.main.CivUI.activeSelf);
        Map.main.tileMapManager.DeselectTile();
        Player.myPlayer.selectedArmies.Clear();
        UIManager.main.CivUI.GetComponent<CivUIPanel>().OpenMenu(3);
    }
    private void OnGUI()
    {
        if (color != null)
        {            
            if (Player.myPlayer.myCivID > -1)
            {
                color.color = Player.myPlayer.myCiv.c;
            }
            else
            {
                color.color = Color.white;
            }
        }
        if (civName != null)
        {
            if (Player.myPlayer.myCivID > -1)
            {
                civName.text = Player.myPlayer.myCiv.civName;
            }
            else
            {
                civName.text = "Spectator";
            }
        }
        if (Age != null)
        {
            Age.text = Game.main.gameTime.ToDate();
        }
        if (Player.myPlayer.myCivID > -1)
        {
            Civilisation civ = Player.myPlayer.myCiv;
            float taxV = civ.TaxIncome();
            float prodV = civ.ProductionIncome();
            float tradeV = civ.TradeIncome();
            float subjV = civ.GetSubjectIncome();
            float armyM = civ.ArmyMaintainance();
            float advisorM = civ.AdvisorMaintainance();
            float fortM = civ.FortMaintenance();
            float interestM = civ.GetInterestPayment();
            float diploM = civ.DiplomaticExpenses();
            float balanceV = taxV + prodV + tradeV + subjV - armyM - advisorM - fortM - interestM - diploM;
            string hoverText = "Bank: " + Mathf.Round(civ.coins * 100f) / 100f + "<sprite index=0>\n\n";
            hoverText += "<#00ff00>Tax: +" + Mathf.Round(taxV * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Production: +" + Mathf.Round(prodV * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Trade:+" + Mathf.Round(tradeV * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Subjects:+" + Mathf.Round(subjV * 100f) / 100f + "<sprite index=0>\n\n";
            hoverText += "<#ff0000>Army: -" + Mathf.Round(armyM * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Advisors: -" + Mathf.Round(advisorM * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Forts: -" + Mathf.Round(fortM * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Interest: -" + Mathf.Round(interestM * 100f) / 100f + "<sprite index=0>\n";
            hoverText += "Diplomatic: -" + Mathf.Round(diploM * 100f) / 100f + "<sprite index=0>\n\n";
            hoverText += (balanceV > 0 ? "<#00ff00>Balance: +" : "<#ff0000>Balance: ") + Mathf.Round(balanceV*100f)/100f + "<sprite index=0>\n";
            coins.GetComponent<HoverText>().text = hoverText;
            coins.text = (balanceV > 0 ? "<#00ff00>" : "<#ff0000>") + Mathf.Round(civ.coins) + "<sprite index=0>";
            Color c = Color.Lerp(Color.red, Color.green, (float)civ.GetTotalTilePopulation() / (float)(civ.GetTotalMaxPopulation() + 1));
            if (civ.GetTotalTilePopulation() >= civ.GetTotalMaxPopulation())
            {
                c = Color.magenta;
            }
            string colorCode  = "<#" + ColorUtility.ToHtmlStringRGB(c) + ">";
            int armyQ = civ.GetTotalTilePopulation();
            if (armyQ < 1000)
            {
                manpower.text = colorCode +Mathf.Round(armyQ) + "<sprite index=0>";
            }
            else if (armyQ < 1000000)
            {
                manpower.text = colorCode + Mathf.Round(armyQ / 1000f) + "k" + "<sprite index=0>";
            }
            else 
            {
                manpower.text = colorCode + Mathf.Round(armyQ / 1000000f) + "M" + "<sprite index=0>";
            }
            string manpowerText = "You have " + civ.GetTotalTilePopulation() + "<sprite index=4> working in your country\n\n";
            manpowerText += "You gain a maximum of " + civ.GetTotalPopulationGrowth() + "<sprite index=4> per day\n";
            manpowerText += "You can have up to " + civ.GetTotalMaxPopulation() + "<sprite index=4>\n";
            manpowerText += "You currently have " + Mathf.Round((float)civ.GetTotalTilePopulation()/(float)civ.GetTotalMaxPopulation()*100f) + "% of maximum<sprite index=4>\n";
            manpower.GetComponent<HoverText>().text = manpowerText;
            admin.text = civ.adminPower + "<sprite index=0>";
            diplo.text = civ.diploPower + "<sprite index=0>";
            mil.text = civ.milPower + "<sprite index=0>";
            hoverText = "Prestige: " + Mathf.Round(civ.prestige * 100f) / 100f + "<sprite index=5>\n\n";
            hoverText += "This Changes By: " + Mathf.Round(civ.GetMonthlyPrestigeChange() * 100f) / 100f + "<sprite index=5> every 30<sprite index=12>\n";
            hoverText += "Monthly Prestige: " + Mathf.Round(civ.monthlyPrestige.value * 100f) / 100f + "<sprite index=5>\n";
            hoverText += "Prestige Decay: " + Mathf.Round(civ.prestigeDecay.value * 100f) + "%\n\n";
            hoverText += "This Level of Prestige<sprite index=5> Gives the Following:\n\n";
            hoverText += (civ.prestige >= 0 ? "<#00ff00>" : "<#ff0000>") + "Tax Efficiency: " +(civ.prestige >= 0? "+" : "")+ Mathf.Round(civ.prestige * 0.1f) + "%\n";
            hoverText += "Morale: " + (civ.prestige >= 0 ? "+" : "") + Mathf.Round(civ.prestige * 0.1f) + "%\n";
            hoverText += "Population Growth: " + (civ.prestige >= 0 ? "+" : "") + Mathf.Round(civ.prestige * 0.1f) + "%\n";
            prestige.GetComponent<HoverText>().text = hoverText;
            prestige.text = Mathf.Round(civ.prestige) + "<sprite index=0>";
            hoverText = "Stability: " + Mathf.Round(civ.stability * 100f) / 100f + "<sprite index=6>\n\n";            
            hoverText += "This Level of Stability<sprite index=6> Gives the Following:\n\n";
            if(civ.stability > 0)
            {
                hoverText += "<#ff0000>Stability Cost: +"  + Mathf.Round(civ.stability * 50f) + "%\n";
            }
            else if (civ.stability < 0)
            {
                hoverText += "<#ff0000>Interest: +" + Mathf.Round(civ.stability * -1f) + "%\n";
            }
            hoverText += (civ.stability >= 0 ? "<#00ff00>" : "<#ff0000>") + "Global Unrest: " + (civ.stability >= 0 ? "" : "+") + Mathf.Round(civ.stability * -1f) + "\n";
            hoverText += "Daily Control: " + (civ.stability >= 0 ? "+" +civ.stability * 0.01f : civ.stability * 0.02f) + "%\n";
            hoverText += "Tax Efficiency: " + (civ.stability >= 0 ? "+" : "") + Mathf.Round(civ.stability * 5f) + "%\n";
            stability.GetComponent<HoverText>().text = hoverText;
            stability.text = Mathf.Round(civ.stability) + "<sprite index=6>";
        }
    }
}
