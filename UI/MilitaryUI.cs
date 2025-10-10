using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MilitaryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI forceLimit,morale,discipline,tactics,tradition,fortDefence,combatWidth;
    [SerializeField] Image infantry, cavalry, artillery;

    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        forceLimit.text = Mathf.Round(civ.TotalMaxArmySize()/1000f) + " / "+ Mathf.Round(civ.forceLimit.value);
        morale.text = Mathf.Round(civ.moraleMax.value * 100f)/100f + "";
        discipline.text = Mathf.Round(civ.discipline.value * 100f) + "%";
        tactics.text = Mathf.Round(civ.militaryTactics.value * 100f) / 100f + "";
        tradition.text = Mathf.Round(civ.armyTradition * 100f) / 100f + "";
        fortDefence.text = Mathf.Round(civ.fortDefence.value * 100f) / 100f + "";
        combatWidth.text = Mathf.Round(civ.combatWidth.value) + "";
        SetupUnit(0, infantry);
        SetupUnit(1, cavalry);
        SetupUnit(2, artillery);
        SetupHoverText();
    }
    void SetupUnit(int type, Image baseImg)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        UnitType unit = civ.units[type];
        float combatAbility = type == 0 ? civ.infantryCombatAbility.value : type == 1 ? civ.flankingCombatAbility.value : civ.siegeCombatAbility.value;
        TextMeshProUGUI[] texts = baseImg.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = Mathf.Round(unit.meleeDamage.value * 100f) / 100f + "";
        texts[1].text = Mathf.Round(unit.flankingDamage.value * 100f) / 100f + "";
        texts[2].text = Mathf.Round(unit.rangedDamage.value * 100f) / 100f + "";
        texts[3].text = Mathf.Round(combatAbility * 100f) / 100f + "";
        texts[4].text = Mathf.Round(unit.baseCost * 100f) / 100f + "";
    }
    void SetupHoverText()
    {        
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        HoverText hoverText = forceLimit.transform.parent.GetComponent<HoverText>();
        string text = "The maximum number of regiments that can be fielded without penalty is: " + Mathf.Round(civ.forceLimit.value) +"\n\n";
        text += "This is due to:\n";
        text += civ.forceLimit.ToString();
        hoverText.text = text;

        hoverText = morale.transform.parent.GetComponent<HoverText>();
        text = "The maximum morale of regiments is: " + Mathf.Round(civ.moraleMax.value * 100f) / 100f + "\n\n";
        text += "This is due to:\n";
        text += civ.moraleMax.ToString();
        hoverText.text = text;

        hoverText = discipline.transform.parent.GetComponent<HoverText>();
        text = "The discipline of regiments is: " + Mathf.Round(civ.discipline.value * 100f) + "%\n\n";
        text += "This is due to:\n";
        text += civ.discipline.ToString();
        hoverText.text = text;

        hoverText = tactics.transform.parent.GetComponent<HoverText>();
        text = "The military tactics of regiments is: " + Mathf.Round(civ.militaryTactics.value * 100f) / 100f + "\n\n";
        text += "This is due to:\n";
        text += civ.militaryTactics.ToString();
        hoverText.text = text;

        hoverText = tradition.transform.parent.GetComponent<HoverText>();
        text = "The current army traditon is: " + Mathf.Round(civ.armyTradition * 100f) / 100f + "\n\n";
        text += "This gives the following bonuses:\n";
        text += "Max Morale + " + Mathf.Round(civ.armyTradition * 0.25f) + "%\n";
        text += "Siege Ability + " + Mathf.Round(civ.armyTradition * 0.1f) + "%\n";
        hoverText.text = text;

        hoverText = fortDefence.transform.parent.GetComponent<HoverText>();
        text = "The global fort defence of the country is: " + Mathf.Round(civ.fortDefence.value * 100f) / 100f + "%\n\n";
        text += "This is due to:\n";
        text += civ.fortDefence.ToString();
        hoverText.text = text;

        hoverText = combatWidth.transform.parent.GetComponent<HoverText>();
        text = "The global combat with of the country is: " + Mathf.Round(civ.combatWidth.value) + "\n\n";
        text += "This is due to:\n";
        text += civ.combatWidth.ToString();
        hoverText.text = text;
    }
}
