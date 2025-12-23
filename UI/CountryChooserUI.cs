using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountryChooserUI : MonoBehaviour
{
    [SerializeField] Image icon, gov, religion;
    [SerializeField] TextMeshProUGUI countryName, ruler, admin, diplo, mil;
    [SerializeField] Image nationalIdeas;
    [SerializeField] Image[] ideas;
    [SerializeField] TextMeshProUGUI pop, dev, provs, fort;

    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1 || Game.main.Started) { gameObject.SetActive(false);return; }
        Civilisation civ = Player.myPlayer.myCiv;
        icon.color = civ.c;
        gov.sprite = Map.main.governmentTypes[civ.government].sprite;
        gov.GetComponent<HoverText>().text = Map.main.governmentTypes[civ.government].GetHoverText(civ);
        religion.sprite = Map.main.religions[civ.religion].sprite;
        religion.GetComponent<HoverText>().text = Map.main.religions[civ.religion].GetHoverText(civ);
        countryName.text = civ.civName;
        ruler.text = civ.ruler.active ? civ.ruler.Name : "No Ruler";
        admin.text = civ.ruler.active ? civ.ruler.adminSkill + "" : "-1";
        diplo.text = civ.ruler.active ? civ.ruler.diploSkill + "" : "-1";
        mil.text = civ.ruler.active ? civ.ruler.milSkill + "" : "-1";
        string ideasText = civ.nationalIdeas.traditions[0].GetHoverText(civ);
        ideasText += civ.nationalIdeas.traditions[1].GetHoverText(civ);
        nationalIdeas.GetComponent<HoverText>().text = ideasText;
        for(int i = 0; i < civ.nationalIdeas.ideas.Length; i++)
        {
            Idea idea = civ.nationalIdeas.ideas[i];
            ideas[i].GetComponentsInChildren<Image>()[1].sprite = idea.icon;
            ideas[i].GetComponent<HoverText>().text = idea.GetHoverText(civ);
        }
        pop.text = Mathf.Round(civ.GetTotalTilePopulation()/1000f) + "k";
        dev.text = civ.GetTotalDev() + "";
        provs.text = civ.GetAllCivTiles().Count + "";
        fort.text = Mathf.Round(1 + civ.FortMaintenance()) + "";
    }
}
