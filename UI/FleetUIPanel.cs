using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;

using UnityEngine;
using UnityEngine.UI;

public class FleetUIPanel : MonoBehaviour
{
    [SerializeField] Transform regimentTransform;
    public List<GameObject> regiments = new List<GameObject>();
    [SerializeField] TextMeshProUGUI armyName;
    [SerializeField] GameObject regimentPrefab, armyPanel, generalPanel;
    [SerializeField] Button[] buttons;
    [SerializeField] Sprite[] unitSprites;
    [SerializeField] TextMeshProUGUI[] unitQuantities;
    [SerializeField] TextMeshProUGUI[] generalStats;

    private void Start()
    {
        //buttons[0].onClick.AddListener(OpenSiegeView);
        //buttons[1].onClick.AddListener(ConsolidateRegiments);
        //buttons[2].onClick.AddListener(Split);
        //buttons[3].onClick.AddListener(Disband);
        //buttons[4].onClick.AddListener(ToggleAttach);
        //buttons[5].onClick.AddListener(DetatchMercs);
        //buttons[6].onClick.AddListener(ChooseGeneral);
        //buttons[7].onClick.AddListener(BoardFleet);
        generalPanel.SetActive(false);
        armyPanel.SetActive(true);
    }
    void ChooseGeneral()
    {
        generalPanel.SetActive(true);
        armyPanel.SetActive(false);
    }
    void BoardFleet()
    {
        if (Player.myPlayer.selectedArmies.Count != 1 || Player.myPlayer.selectedArmies[0].inBattle)
        {
            return;
        }
        Army army = Player.myPlayer.selectedArmies[0];
        if (army.tile.fleetsOnTile.Count <= 0 || army.tile.fleetsOnTile.FindAll(i => i.civID == army.civID).Count == 0)
        {
            return;
        }
        Fleet fleet = army.tile.fleetsOnTile.Find(i => i.civID == army.civID);
        int transports = 0;
        foreach (var boat in fleet.boats)
        {
            if (boat.type == 0)
            {
                transports++;
            }
        }
        while(fleet.army.Count < transports && army.regiments.Count > 0)
        {
            fleet.army.Add(army.regiments[0]);
            army.regiments.RemoveAt(0);
        }
        if (army.regiments.Count == 0)
        {
            army.OnExitTile(army.tile);
            Destroy(army.gameObject);
        }
    }
    void ToggleAttach()
    {

    }
    void DetatchMercs()
    {
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            army.DetatchMercs();
        }
    }
    private void OnGUI()
    {
       if (Player.myPlayer.selectedFleets.Count == 1 && !Player.myPlayer.selectedFleets[0].inBattle)
        {
            Fleet fleet = Player.myPlayer.selectedFleets[0];
            int transport = 0;
            int trade = 0;
            int combat = 0;
            if (fleet.boats != null && regiments != null)
            {
                while (regiments.Count != fleet.boats.Count)
                {
                    if (regiments.Count > fleet.boats.Count)
                    {
                        Destroy(regiments[0]);
                        regiments.RemoveAt(0);
                    }
                    else
                    {
                        regiments.Add(Instantiate(regimentPrefab, regimentTransform));
                    }
                }
                for (int i = 0; i < fleet.boats.Count; i++)
                {
                    Boat boat = fleet.boats[i];
                    if (boat.type == 0)
                    {
                        transport++;
                    }
                    else if (boat.type == 1)
                    {
                        trade++;
                    }
                    else if (boat.type == 2)
                    {
                        combat++;
                    }
                    Image[] images = regiments[i].GetComponentsInChildren<Image>();
                    TextMeshProUGUI[] texts = regiments[i].GetComponentsInChildren<TextMeshProUGUI>();
                    if (boat != null)
                    {
                        texts[0].text = boat.sailors + "";
                        texts[1].text ="Supply: " +  Mathf.Round(boat.supply) + "/" +Mathf.Round(boat.supplyMax);
                        texts[2].text ="Durability: " + Mathf.Round(boat.hullStrength) + "/" +Mathf.Round(boat.hullStrengthMax);
                        images[0].color = boat.mercenary ? Color.green : Color.white;
                        images[1].sprite = unitSprites[boat.type];
                        if (boat.cannons > 0)
                        {
                            texts[3].text =  boat.cannons + "<sprite index=0>";
                        }
                        else
                        {
                            texts[3].text = "";
                        }
                    }
                    else
                    {
                        texts[0].text = 0 + "";
                        texts[1].text = 0 + "";
                        texts[2].text = 0 + "";
                        texts[3].text = "";
                    }
                }
                unitQuantities[0].text = transport + "";
                unitQuantities[1].text = trade + "";
                unitQuantities[2].text = combat + "";
                for (int i = 0; i < generalStats.Length; i++)
                {
                    TextMeshProUGUI text = generalStats[i];
                    if (fleet.general != null && fleet.general.active)
                    {
                        int skill = i == 0 ? fleet.general.combatSkill : i == 1 ? fleet.general.siegeSkill : fleet.general.maneuverSkill;
                        text.text = skill + "";
                    }
                    else
                    {
                        text.text = "0";
                    }
                }
            }

        }
        else
        {
            gameObject.SetActive(false);
        }
    }
   
}
