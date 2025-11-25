using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;

using UnityEngine;
using UnityEngine.UI;

public class ArmyUIPanel : MonoBehaviour
{
    [SerializeField] Transform regimentTransform;
    public List<GameObject> regiments = new List<GameObject>();
    [SerializeField] TextMeshProUGUI armyName;
    [SerializeField] GameObject regimentPrefab,armyPanel,generalPanel;
    [SerializeField] Button[] buttons;
    [SerializeField] Sprite[] unitSprites;
    [SerializeField] TextMeshProUGUI[] unitQuantities;
    [SerializeField] TextMeshProUGUI[] generalStats;

    private void Start()
    {
        buttons[0].onClick.AddListener(OpenSiegeView);
        buttons[1].onClick.AddListener(ConsolidateRegiments);
        buttons[2].onClick.AddListener(Split);
        buttons[3].onClick.AddListener(Disband);
        buttons[4].onClick.AddListener(ToggleAttach);
        buttons[5].onClick.AddListener(DetatchMercs);
        buttons[6].onClick.AddListener(ChooseGeneral);
        buttons[7].onClick.AddListener(BoardFleet);
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
        while (fleet.army.Count < transports && army.regiments.Count > 0)
        {
            fleet.army.Add(army.regiments[0]);
            army.regiments.RemoveAt(0);
        }
        if (army.regiments.Count == 0)
        {
            army.OnExitTile();
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
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            int infantry = 0;
            int cavalry = 0;
            int artillery = 0;
            if (army.regiments != null && regiments != null)
            {
                while (regiments.Count != army.regiments.Count)
                {
                    if (regiments.Count > army.regiments.Count)
                    {
                        Destroy(regiments[0]);
                        regiments.RemoveAt(0);
                    }
                    else
                    {
                        regiments.Add(Instantiate(regimentPrefab, regimentTransform));
                    }
                }
                for (int i = 0; i < army.regiments.Count; i++)
                {
                    Regiment regiment = army.regiments[i];
                    if(regiment.type == 0)
                    {
                        infantry++;
                    }
                    else if (regiment.type == 1)
                    {
                        cavalry++;
                    }
                    else if (regiment.type == 2)
                    {
                        artillery++;
                    }
                    Image[] images = regiments[i].GetComponentsInChildren<Image>();
                    TextMeshProUGUI regSize = regiments[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (regiment != null)
                    {
                        regSize.text = regiment.size + "";
                        images[0].color = regiment.mercenary ? Color.green : Color.white;
                        images[1].sprite = unitSprites[regiment.type];
                        if (regiment.size > 0)
                        {
                            images[4].fillAmount = regiment.morale / regiment.maxMorale;
                        }
                        else
                        {
                            images[4].fillAmount = 0;
                        }
                    }
                    else
                    {
                        regSize.text = 0 + "";
                        images[4].fillAmount = 0;
                    }
                }
                unitQuantities[0].text = infantry + "";
                unitQuantities[1].text = cavalry + "";
                unitQuantities[2].text = artillery + "";
                for (int i = 0; i < generalStats.Length; i++)
                {
                    TextMeshProUGUI text = generalStats[i];
                    if (army.general != null && army.general.active)
                    {
                        int skill = i == 0 ? army.general.meleeSkill : i == 1 ? army.general.flankingSkill : i == 2 ? army.general.rangedSkill : i == 3 ? army.general.siegeSkill : army.general.maneuverSkill;                  
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
    void Disband()
    {
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            army.Disband();
        }
    }
    void OpenSiegeView()
    {
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            if (Map.main.GetTile(army.pos).underSiege)
            {
                Player.myPlayer.selectedArmies.Clear();
                Player.myPlayer.selectedTile = army.tile;
                Player.myPlayer.siegeSelected = true;
            }
        }
    }
    void ConsolidateRegiments()
    {
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            army.Consolidate(!Input.GetKey(KeyCode.LeftShift));
        }
    }
    void Split()
    {
        //Debug.Log(Player.myPlayer.selectedArmies.Count + "selected");
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];
            //Debug.Log(army.regiments.Count + "regiments");
            army.Split();
        }
    }
}
