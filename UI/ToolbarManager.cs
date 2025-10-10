using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarManager : MonoBehaviour
{
    [SerializeField] Image color;
    [SerializeField] TextMeshProUGUI civName;
    [SerializeField] TextMeshProUGUI coins, manpower, admin, diplo, mil;
    [SerializeField] TextMeshProUGUI prestige;
    [SerializeField] TextMeshProUGUI Age;

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
            float armyM = civ.ArmyMaintainance();
            float advisorM = civ.AdvisorMaintainance();
            float fortM = civ.FortMaintenance();
            float interestM = civ.GetInterestPayment();
            float balanceV = taxV + prodV - armyM - advisorM - fortM - interestM;
            coins.text = (balanceV > 0 ? "<#00ff00>" : "<#ff0000>") + Mathf.Round(civ.coins) + "<sprite index=0>";
            Color c = Color.Lerp(Color.red, Color.green, (float)civ.GetTotalTilePopulation() / (float)(civ.GetTotalMaxPopulation() + 1));
            if (civ.GetTotalTilePopulation() >= civ.GetTotalMaxPopulation())
            {
                c = Color.magenta;
            }
            string colorCode  = "<#" + c.ToHexString() + ">";
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
            admin.text = civ.adminPower + "<sprite index=0>";
            diplo.text = civ.diploPower + "<sprite index=0>";
            mil.text = civ.milPower + "<sprite index=0>";
            prestige.text = Mathf.Round(civ.prestige) + "<sprite index=0>";
        }
    }
}
