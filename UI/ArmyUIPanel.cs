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
    [SerializeField] GameObject regimentPrefab;
    [SerializeField] Button[] buttons;
    [SerializeField] Sprite[] unitSprites;

    private void Start()
    {
        buttons[0].onClick.AddListener(OpenSiegeView);
        buttons[1].onClick.AddListener(ConsolidateRegiments);
        buttons[2].onClick.AddListener(Split);
        buttons[3].onClick.AddListener(Disband);
    }
    private void OnGUI()
    {
        if (Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle)
        {
            Army army = Player.myPlayer.selectedArmies[0];           
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
                    Image[] images = regiments[i].GetComponentsInChildren<Image>();
                    TextMeshProUGUI regSize = regiments[i].GetComponentInChildren<TextMeshProUGUI>();
                    if (regiment != null)
                    {
                        regSize.text = regiment.size + "";
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
            int space = Game.main.civs[army.civID].AddPopulation((int)army.ArmySize());
            if (space >= (int)army.ArmySize())
            {
                army.OnExitTile();
                Destroy(army);
            }
            else
            {
                while (space > 0 && army.regiments.Count > 0)
                {
                    if (space >= army.regiments[0].size)
                    {
                        space -= army.regiments[0].size;
                        army.regiments.RemoveAt(0);
                    }
                    else
                    {
                        army.regiments[0].size -= space;
                        space = 0;
                    }
                }
                if(army.regiments.Count == 0)
                {
                    army.OnExitTile();
                    Destroy(army);
                }
            }
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
