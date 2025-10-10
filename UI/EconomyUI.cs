using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tax,prod;
    [SerializeField] TextMeshProUGUI army,advisors,fort,interest;
    [SerializeField] TextMeshProUGUI balance;
    [SerializeField] Button loan;
    private void Start()
    {
        loan.onClick.AddListener(Loan);
    }
    void Loan()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        civ.TakeLoan();
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        float taxV = civ.TaxIncome();
        float prodV = civ.ProductionIncome();
        float armyM = civ.ArmyMaintainance();
        float advisorM = civ.AdvisorMaintainance();
        float fortM = civ.FortMaintenance();
        float interestM = civ.GetInterestPayment();
        tax.text = "<#00ff00>+" + Mathf.Round(taxV * 100f)/100f;
        prod.text = "<#00ff00>+" + Mathf.Round(prodV * 100f) / 100f;
        army.text = "<#ff0000>-" + Mathf.Round(armyM * 100f) / 100f;
        advisors.text = "<#ff0000>-" + Mathf.Round(advisorM * 100f) / 100f;
        fort.text = "<#ff0000>-" + Mathf.Round(fortM * 100f) / 100f;
        interest.text = "<#ff0000>-" + Mathf.Round(interestM * 100f) / 100f;
        loan.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Round(civ.GetLoanSize() * 100f) / 100f + "<sprite index=0>";
        loan.interactable = civ.loans.Count < civ.GetMaxLoans();
        loan.GetComponentInChildren<HoverText>().text = "You have " + civ.loans.Count + " loans out of a maximum of " + civ.GetMaxLoans();
        float balanceV = taxV + prodV - armyM - advisorM - fortM - interestM;
        if(balanceV > 0)
        {
            balance.text = "<#00ff00>+" + Mathf.Round(balanceV * 100f) / 100f;
        }
        else
        {
            balance.text = "<#ff0000>" + Mathf.Round(balanceV * 100f) / 100f;
        }
    }
}
