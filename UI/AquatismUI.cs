using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AquatismUI : MonoBehaviour
{
    [SerializeField] Image aquaticFervor;
    [SerializeField] Image[] aquaticFervorBonuses;
    [SerializeField] TextMeshProUGUI fervorText;

    private void Start()
    {
        for (int i = 0; i < aquaticFervorBonuses.Length; i++)
        {
            int index = i;
            Effect effect = Map.main.religions[0].religiousMechanicEffects[index];
            aquaticFervorBonuses[i].transform.parent.GetComponent<HoverText>().text = effect.GetHoverText(Game.main.civs[0]);
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        aquaticFervor.fillAmount = civ.religiousPoints / 100f;
        fervorText.text = "Aquatic Fervor: " + Mathf.Round(civ.religiousPoints * 100f)/100f + "\n ("+Mathf.Round(civ.AquatismFervorDaily() * 100f) / 100f +")";
        fervorText.transform.parent.GetComponent<HoverText>().text = AquatismFervorHoverText(civ);
        for(int i = 0; i < aquaticFervorBonuses.Length; i++)
        {
            bool active = civ.religiousPoints > (i * 100 / aquaticFervorBonuses.Length);
            aquaticFervorBonuses[i].color = active ? Color.green : Color.red;           
        }
    }

    public string AquatismFervorHoverText(Civilisation civ)
    {
        string text = "Base: 0.1\n";
        text += "Stability: " + Mathf.Round(100f * (civ.stability * 0.1f))/100f + "\n";
        text += "Loans: " + Mathf.Round(100f * civ.loans.Count * -0.1f) / 100f + "\n";
        text += "Overextension: " + Mathf.Round(civ.overextension * -1f)/100f + "\n";
        text += "Ruler Skill: " + Mathf.Round(100f * ((civ.ruler.active ? (civ.ruler.adminSkill + civ.ruler.diploSkill + civ.ruler.milSkill) * 0.02f : 0f)))/100f + "\n";
        text += "Advisor Skill: " + Mathf.Round(100f * (((civ.advisorA.active ? (civ.advisorA.skillLevel) * 0.02f : 0f) + (civ.advisorD.active ? (civ.advisorD.skillLevel) * 0.02f : 0f) + (civ.advisorM.active ? (civ.advisorM.skillLevel) * 0.02f : 0f)) ))/100f + "\n";
        text += "Decay: " + Mathf.Round(civ.religiousPoints * -1f)/100f;
        return text;
    }
}
