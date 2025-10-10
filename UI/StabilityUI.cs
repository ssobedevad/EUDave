using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StabilityUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stability, overextension,globalUnrest;
    [SerializeField] Button stabButton;
    [SerializeField] GameObject rebelFactionPrefab;
    [SerializeField] Transform rebelFactionTransform;
    public List<GameObject> rebelFactions = new List<GameObject>();

    private void Start()
    {
        stabButton.onClick.AddListener(StabClick);
    }
    void StabClick()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
        {
            civ.adminPower -= civ.GetStabilityCost();
            civ.AddStability(1);
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        overextension.text = Mathf.Round(civ.overextension * 100f)/100f + "%";
        stability.text = Mathf.Round(civ.stability) + "<sprite index=6>";
        globalUnrest.text = Mathf.Round(civ.globalUnrest.value * 100f) / 100f + "<sprite index=11>";
        string text = "It will cost " + civ.GetStabilityCost() + "<sprite index=1> to boost stability by 1\n\n";
        text += "This is due to:\n";
        text += "Base 100<sprite index=1>\n";
        text += civ.stabilityCost.ToString();
        stabButton.GetComponent<HoverText>().text = text;
        stabButton.interactable = civ.stability < 3;
        List<RebelFaction> factions = civ.rebelFactions.Values.ToList();
        factions.RemoveAll(i => i.totalUnrest <= 0 || i.provinces.Count <= 0 || i.size <= 1000);
        while (rebelFactions.Count != civ.rebelFactions.Count)
        {
            if (rebelFactions.Count > civ.rebelFactions.Count)
            {
                int lastIndex = rebelFactions.Count - 1;
                Destroy(rebelFactions[lastIndex]);
                UIManager.main.UI.Remove(rebelFactions[lastIndex]);
                rebelFactions.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(rebelFactionPrefab, rebelFactionTransform);
                rebelFactions.Add(item);
                UIManager.main.UI.Add(item);
            }
        }
        for (int i = 0; i < civ.rebelFactions.Values.Count; i++)
        {
            RebelFaction rebelFaction = civ.rebelFactions.Values.ToList()[i];
            rebelFactions[i].GetComponentInChildren<TextMeshProUGUI>().text = "size: " + rebelFaction.size + " total unrest: " + rebelFaction.totalUnrest + " uprising progress: " + rebelFaction.uprisingProgress + " total provinces: " + rebelFaction.provinces.Count;
        }
    }
}
