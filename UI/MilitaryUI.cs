using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MilitaryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI forceLimit,morale,discipline,tactics,tradition,fortDefence,combatWidth;
    [SerializeField] Image infantry, cavalry, artillery;
    [SerializeField] Button buyGeneral;
    [SerializeField] Transform generalBack;
    [SerializeField] GameObject generalPrefab;
    List<GameObject> generalList = new List<GameObject>();

    private void Start()
    {
        buyGeneral.onClick.AddListener(BuyGeneral);
    }
    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        forceLimit.text = "Force Limit: " + Mathf.Round(civ.TotalMaxArmySize()/1000f) + " / "+ Mathf.Round(civ.forceLimit.v);
        morale.text = "Morale: " + Mathf.Round(civ.moraleMax.v * 100f)/100f + "";
        discipline.text = "Discipline: " + Mathf.Round(civ.discipline.v * 100f) + "%";
        tactics.text = "Tactics: " + Mathf.Round(civ.militaryTactics.v * 100f) / 100f + "";
        tradition.text = "Army Tradition: " + Mathf.Round(civ.armyTradition * 100f) / 100f + "";
        fortDefence.text = "Fort Defence: " + Mathf.Round(civ.fortDefence.v * 100f) / 100f + "";
        combatWidth.text = "Combat Width: " + Mathf.Round(civ.combatWidth.v) + "";
        SetupUnit(0, infantry);
        SetupUnit(1, cavalry);
        SetupUnit(2, artillery);
        SetupHoverText();
        List<General> generals = civ.generals.ToList();
        generals.RemoveAll(i => !i.active);
        while (generalList.Count != generals.Count)
        {
            if (generalList.Count > generals.Count)
            {
                int lastIndex = generalList.Count - 1;
                Destroy(generalList[lastIndex]);
                generalList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(generalPrefab, generalBack);
                generalList.Add(item);
            }
        }
        for (int i = 0; i < generals.Count; i++)
        {
            General general = generals[i];
            Image[] images = generalList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = generalList[i].GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = general.name + " - " + general.combatSkill + " " + general.siegeSkill + " " + general.maneuverSkill;            
        }
        string hoverText = "Buy a General for 50<sprite index=3>\n\n";
        int genPips = (1 + (int)(civ.armyTradition / 10) + (civ.ruler.active ? civ.ruler.milSkill / 3 : 0));
        hoverText += "They will have between " + genPips + " and " + (genPips + 5) + " at base\n\n";
        hoverText += "Combat Bonus: " + civ.generalCombatSkill.ToString() + "\n";
        hoverText += "Siege Bonus: " + civ.generalSiegeSkill.ToString() + "\n";
        hoverText += "Maneuver Bonus: " + civ.generalManeuverSkill.ToString() + "\n";
        buyGeneral.GetComponent<HoverText>().text = hoverText;
    }
    void BuyGeneral()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.milPower >= 50)
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendMil, 50);
            }
            else
            {
                civ.milPower -= 50;
            }
            civ.BuyGeneral();
        }
    }
    void SetupUnit(int type, Image baseImg)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        UnitType unit = civ.units[type];
        TextMeshProUGUI[] texts = baseImg.GetComponentsInChildren<TextMeshProUGUI>();
        texts[0].text = Mathf.Round(unit.baseDamage.v * 100f) / 100f + "";
        texts[1].text = Mathf.Round(unit.baseCost.v * 100f) / 100f + "";
    }
    void SetupHoverText()
    {        
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        HoverText hoverText = forceLimit.transform.parent.GetComponent<HoverText>();
        string text = "The maximum number of regiments that can be fielded without penalty is: " + Mathf.Round(civ.forceLimit.v) +"\n\n";
        text += "This is due to:\n";
        text += civ.forceLimit.ToString();
        hoverText.text = text;

        hoverText = morale.transform.parent.GetComponent<HoverText>();
        text = "The maximum morale of regiments is: " + Mathf.Round(civ.moraleMax.v * 100f) / 100f + "\n\n";
        text += "This is due to:\n";
        text += civ.moraleMax.ToString();
        hoverText.text = text;

        hoverText = discipline.transform.parent.GetComponent<HoverText>();
        text = "The discipline of regiments is: " + Mathf.Round(civ.discipline.v * 100f) + "%\n\n";
        text += "This is due to:\n";
        text += civ.discipline.ToString();
        hoverText.text = text;

        hoverText = tactics.transform.parent.GetComponent<HoverText>();
        text = "The military tactics of regiments is: " + Mathf.Round(civ.militaryTactics.v * 100f) / 100f + "\n\n";
        text += "This is due to:\n";
        text += civ.militaryTactics.ToString();
        hoverText.text = text;

        hoverText = tradition.transform.parent.GetComponent<HoverText>();
        text = "The current army traditon is: " + Mathf.Round(civ.armyTradition * 100f) / 100f + "\n\n";
        text += "This gives the following bonuses:\n";
        text += "Max Morale + " + Mathf.Round(civ.armyTradition * 0.25f) + "%\n";
        text += "Siege Ability + " + Mathf.Round(civ.armyTradition * 0.1f) + "%\n";
        text += "Morale Recovery + " + Mathf.Round(civ.armyTradition * 0.1f) + "%\n";
        text += "Population Growth + " + Mathf.Round(civ.armyTradition * 0.1f) + "%\n";
        hoverText.text = text;

        hoverText = fortDefence.transform.parent.GetComponent<HoverText>();
        text = "The global fort defence of the country is: " + Mathf.Round(civ.fortDefence.v * 100f) / 100f + "%\n\n";
        text += "This is due to:\n";
        text += civ.fortDefence.ToString();
        hoverText.text = text;

        hoverText = combatWidth.transform.parent.GetComponent<HoverText>();
        text = "The global combat with of the country is: " + Mathf.Round(civ.combatWidth.v) + "\n\n";
        text += "This is due to:\n";
        text += civ.combatWidth.ToString();
        hoverText.text = text;
    }
}
