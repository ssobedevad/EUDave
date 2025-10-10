using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class ArmyUIProvince : MonoBehaviour
{
    [SerializeField] GameObject siegeCanvas,armyCanvas;
    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI armyNum, siegePercent;
    [SerializeField] Image civColor, moraleFill, siegeFill, armyPanel;
    [SerializeField] Button armySelect, siegeSelect;

    public Army army;
    private void Start()
    {
        armySelect.onClick.AddListener(SelectArmy);
        siegeSelect.onClick.AddListener(SelectSiege);
    }
    private void OnGUI()
    {
        if(army == null)
        {
            UIManager.main.WorldSpaceUI.Remove(gameObject);
            Destroy(gameObject); 
            return; 
        }
        if (army.inBattle) 
        {
            armyCanvas.SetActive(false);
            siegeCanvas.SetActive(false);
            panel.sizeDelta = new Vector2(0, 0);
            return;
        }
        armyCanvas.SetActive(true);
        siegeCanvas.SetActive(true);
        transform.position = army.tile.worldPos();
        TileData tile = Map.main.GetTile(army.pos);
        float armyQ = army.ArmySize();
        if (armyQ < 1000)
        {
            armyNum.text = Mathf.Round(armyQ) + "";
        }
        else
        {
            armyNum.text = Mathf.Round(armyQ * 10f) / 10000f + "k";
        }
        if (tile != null && tile.armiesOnTile.Count > 0)
        {
            int index = tile.armiesOnTile.IndexOf(army);            
        }
        if (army.AverageMaxMorale() > 0)
        {
            moraleFill.fillAmount = army.AverageMorale() / army.AverageMaxMorale();
        }
        else
        {
            moraleFill.fillAmount = 0;
        }
        if (tile.underSiege && army.path.Count == 0 && tile.siege.armiesSieging.Contains(army))
        {
            panel.sizeDelta = new Vector2(260, 40);
            siegeCanvas.SetActive(true);
            Siege siege = tile.siege;
            siegeFill.fillAmount = (float)siege.tickTimer / (float)siege.tickTime;
            siegePercent.text = Mathf.Round(siege.progress * 100f) + "%";
        }
        else
        {
            panel.sizeDelta = new Vector2(130, 40);
            siegeCanvas.SetActive(false);
        }
        if (army.civID > -1)
        {
            Civilisation civ = Game.main.civs[army.civID];
            civColor.color = civ.c;
            armyPanel.color = Color.grey;
            if (Player.myPlayer.myCivID > -1)
            {
                Civilisation myCiv = Game.main.civs[Player.myPlayer.myCivID];
                if (army.civID == myCiv.CivID)
                {
                    armyPanel.color = Player.myPlayer.selectedArmies.Contains(army) ? Color.yellow : Color.green;
                }
                else if (myCiv.atWarWith.Contains(army.civID))
                {
                    armyPanel.color = Color.red;
                }
                else if (myCiv.militaryAccess.Contains(army.civID))
                {
                    armyPanel.color = Color.blue;
                }
            }
        }
        else
        {
            civColor.color = Color.black;
            armyPanel.color = Color.red;
        }
    }
    void SelectArmy()
    {
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedArmies.Add(army);
        UIManager.main.CivUI.SetActive(false);
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
        Player.myPlayer.siegeSelected = false;
    }
    void SelectSiege()
    {
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedTile = army.tile;
        Player.myPlayer.tileSelected = true;
        Player.myPlayer.siegeSelected = true;
    }
}
