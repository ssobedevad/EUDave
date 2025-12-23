using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenuUI : MonoBehaviour
{
    [SerializeField] Button[] buildingButtons;
    [SerializeField] Sprite add,remove,inprogress,unknown;
    private void Start()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            int index = i;
            buildingButtons[i].onClick.AddListener(delegate { BuildingMenuClick(index); });
        }
    }
    void BuildingMenuClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Building building = Map.main.Buildings[id];
        Civilisation civ = Player.myPlayer.myCiv;
        if (!tile.buildings.Contains(id) && !tile.buildQueue.Contains(id) && (tile.civ == civ  || tile.civ.overlordID == civ.CivID))
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.Build, id);
            }
            else
            {
                tile.StartBuilding(id, civ.CivID);
            }
        }
        else if (tile.buildings.Contains(id))
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.RemoveBuilding, id);
            }
            else
            {
                tile.RemoveBuilding(id);
            }
        }
    }
    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation civ = Player.myPlayer.myCiv;
        for (int i = 0; i < buildingButtons.Length; i++)
        {
            Building building = Map.main.Buildings[i];
            bool unlocked = civ.unlockedBuildings.Contains(i);
            if (unlocked)
            {
                buildingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = building.GetCost(tile, civ) + "<sprite index=0>";
            }
            else
            {
                buildingButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "??? <sprite index=0>";
            }
            Image icon = buildingButtons[i].GetComponentsInChildren<Image>()[1];
            Image image = buildingButtons[i].GetComponentsInChildren<Image>()[2];
            buildingButtons[i].enabled = unlocked;
            icon.color = unlocked ? Color.white : Color.black;
            if (!unlocked)
            {
                image.sprite = unknown;
            }
            else if (tile.buildQueue.Contains(i))
            {
                image.sprite = inprogress;
            }
            else if (tile.buildings.Contains(i))
            {
                image.sprite = remove;
            }
            else
            {
                image.sprite = add;
            }
            SetHoverText(buildingButtons[i].GetComponent<HoverText>(), building, unlocked);
        }
    }
    void SetHoverText(HoverText hoverText, Building building,bool unlocked)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation civ = Player.myPlayer.myCiv;
        if (unlocked)
        {
            string text = "Building a " + building.Name + " Costs " + building.GetCost(tile, civ) + "<sprite index=0>\n\n";
            text += "Local Bonuses: " + tile.localConstructionCost.ToString() + "\n";
            text += "Global Bonuses: " + civ.constructionCost.ToString() + "\n\n";
            text += "This will take " + building.GetTime(tile, civ) + "<sprite index=12> to complete\n";
            text += "Local Bonuses: " + tile.localConstructionTime.ToString() + "\n";
            text += "Global Bonuses: " + civ.constructionTime.ToString() + "\n\n";

            if (building.effects.name != "")
            {
                text += "This will have the following effect on this tile:\n";
                text += building.effects.name + Modifier.ToString(building.effects.amount, tile.GetStat(building.effects.name)) + "\n";
            }
            if (building.fortLevel > 0)
            {
                text += "Fort level +" + building.fortLevel;                
            }


            hoverText.text = text;
        }
        else
        {
            hoverText.text = "Locked";
        }
    }
}
