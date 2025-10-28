using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EconomyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tax,prod,trade,subjects;
    [SerializeField] TextMeshProUGUI army,advisors,fort,interest,diplomatic;
    [SerializeField] TextMeshProUGUI balance;
    [SerializeField] Button loan,repayLoans;
    private void Start()
    {
        loan.onClick.AddListener(Loan);
        repayLoans.onClick.AddListener(RepayLoans);
    }
    void Loan()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        civ.TakeLoan();
    }
    void RepayLoans()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if(civ.loans.Count > 0)
        {
            while (civ.loans.Count > 0 && civ.coins >= civ.loans[0].value)
            {
                civ.coins -= civ.loans[0].value;
                civ.loans.RemoveAt(0);
            }
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        float taxV = civ.TaxIncome();
        float prodV = civ.ProductionIncome();
        float tradeV = civ.TradeIncome();
        float subjV = civ.GetSubjectIncome();
        float armyM = civ.ArmyMaintainance();
        float advisorM = civ.AdvisorMaintainance();
        float fortM = civ.FortMaintenance();
        float interestM = civ.GetInterestPayment();
        float diploExpensesM = civ.DiplomaticExpenses();
        tax.text = "<#00ff00>+" + Mathf.Round(taxV * 100f)/100f;
        prod.text = "<#00ff00>+" + Mathf.Round(prodV * 100f) / 100f;
        trade.text = "<#00ff00>+" + Mathf.Round(tradeV * 100f) / 100f;
        subjects.text = "<#00ff00>+" + Mathf.Round(subjV * 100f) / 100f;
        army.text = "<#ff0000>-" + Mathf.Round(armyM * 100f) / 100f;
        advisors.text = "<#ff0000>-" + Mathf.Round(advisorM * 100f) / 100f;
        fort.text = "<#ff0000>-" + Mathf.Round(fortM * 100f) / 100f;
        interest.text = "<#ff0000>-" + Mathf.Round(interestM * 100f) / 100f;
        diplomatic.text = "<#ff0000>-" + Mathf.Round(diploExpensesM * 100f) / 100f;
        loan.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Round(civ.GetLoanSize() * 100f) / 100f + "<sprite index=0>";
        loan.interactable = civ.loans.Count < civ.GetMaxLoans();
        loan.GetComponentInChildren<HoverText>().text = "You have " + civ.loans.Count + " loans out of a maximum of " + civ.GetMaxLoans();
        float totalDebt = 0;
        civ.loans.ForEach(i => totalDebt += i.value);
        repayLoans.GetComponentInChildren<HoverText>().text = "You have " + totalDebt + " total debt";
        repayLoans.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Round(totalDebt * 100f) / 100f + "<sprite index=0>";
        float balanceV = taxV + prodV + tradeV + subjV - armyM - advisorM - fortM - interestM - diploExpensesM;
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
