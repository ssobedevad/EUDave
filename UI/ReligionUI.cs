using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReligionUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI effects,tolerance,unity;
    [SerializeField] GameObject[] religionMechanics;
    [SerializeField] GameObject MainPanel;
    [SerializeField] Button openMenu;

    private void Start()
    {
        MainPanel.SetActive(true);
        openMenu.onClick.AddListener(OpenreligionMechanic);
    }
    void OpenreligionMechanic()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if(religionMechanics.Length <= civ.religion) { return; }
        religionMechanics[civ.religion].SetActive(true);
        MainPanel.SetActive(false);
    }

    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        icon.sprite = Map.main.religions[civ.religion].sprite;
        unity.text = "Religious Unity: "+Mathf.Round(civ.religiousUnity * 1000f) / 10f + "%";
        unity.GetComponent<HoverText>().text = RUText();
        effects.text = Map.main.religions[civ.religion].GetHoverText(civ);
        tolerance.text = "Tolerance of the True Faith: "+civ.trueFaithTolerance.value +"\n" + "Intolerance of Infidels: " + civ.infidelIntolerance.value;
    }

    string RUText()
    {
        if (Player.myPlayer.myCivID == -1) { return ""; }
        Civilisation civ = Player.myPlayer.myCiv;
        string text = "Religious Unity: " + Mathf.Round(civ.religiousUnity * 100f) + "%\n\n";
        if (civ.religiousUnity < 1) 
        {
            text += "Stability Cost: +" + Mathf.Round((1f - civ.religiousUnity) * 10000f)/100f + "%\n";
            text += "Global Unrest: +" + Mathf.Round((1f - civ.religiousUnity) * 30000f)/100f;
        }
        else
        {
            text += "Monthly Prestige: +1\n";
        }
        return text;
    }
}
