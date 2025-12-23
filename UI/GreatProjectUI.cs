using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GreatProjectUI : MonoBehaviour
{
    [SerializeField] Image icon,t1,t2,t3;
    [SerializeField] Button upgrade;
    [SerializeField] TextMeshProUGUI cost, time;

    private void Start()
    {
        upgrade.onClick.AddListener(UpgradeClick);
    }
    private void OnGUI()
    {
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        if (tile.greatProject == null) { gameObject.SetActive(false); return; }
        Civilisation civ = tile.civ;
        GreatProject gp = tile.greatProject;
        icon.sprite = gp.icon;
        string hoverText = gp.Name + "\n\n"; 
        hoverText += (gp.tier == 0 ? "No Effect at T0" : gp.tier == 1 ? gp.GetHoverTextT1(tile.civ) : gp.tier == 2 ? gp.GetHoverTextT2(tile.civ) : gp.tier == 3 ? gp.GetHoverTextT3(tile.civ) : "Unknow Tier");
        bool isMet = true;
        if(gp.conditions.Length > 0)
        {
            foreach(var condition in gp.conditions)
            {
                bool met = condition.isMet(civ);
                if (!met) { isMet = false; }
                hoverText += "\n"+condition.ToString(civ) + " ("+ met+")";
            }
        }
        icon.GetComponent<HoverText>().text = hoverText;
        string costText = Mathf.Round(gp.GetCost(tile, tile.civ)) + "<sprite index=0>";
        string timeText = Mathf.Round(gp.GetTime(tile, tile.civ)) + "<sprite index=12>";
        cost.text = gp.tier < 3 ? costText : "MAX";
        string text = "Upgrading "+gp.Name+ " Costs: " + costText +"\n\n";
        text += "Local Bonuses: " + tile.localConstructionCost.ToString() + "\n";
        text += "Global Bonuses: " + tile.civ.constructionCost.ToString() + "\n\n";
        cost.transform.parent.GetComponent<HoverText>().text = text;
        time.text = gp.tier < 3 ? Mathf.Round(gp.GetTime(tile, tile.civ) - gp.buildTimer) + "<sprite index=12>" : "MAX";
        text = "Upgrading " + gp.Name + " Takes: " + timeText + "\n\n";
        text += "Local Bonuses: " + tile.localConstructionTime.ToString() + "\n";
        text += "Global Bonuses: " + tile.civ.constructionTime.ToString() + "\n\n";
        time.transform.parent.GetComponent<HoverText>().text = text;
        t1.transform.parent.GetComponent<HoverText>().text = gp.GetHoverTextT1(tile.civ);
        t2.transform.parent.GetComponent<HoverText>().text = gp.GetHoverTextT2(tile.civ);
        t3.transform.parent.GetComponent<HoverText>().text = gp.GetHoverTextT3(tile.civ);
        upgrade.interactable = !gp.isBuilding && gp.tier < 3 && isMet;
        if (gp.tier < 3)
        {
            upgrade.GetComponent<HoverText>().text = "To Upgrade " + gp.Name + " to tier " + (gp.tier + 1) + "\n\nIt will cost: " + costText + "\nIt will take: " + timeText;
        }
        else { upgrade.GetComponent<HoverText>().text = "Maximum Level"; }
    }
    void UpgradeClick()
    {
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        if(Player.myPlayer.myCivID == -1 || tile.civID == -1) { return; }
        if (Player.myPlayer.myCivID != tile.civID) { return; }
        if (tile.greatProject == null) { gameObject.SetActive(false); return; }
        Civilisation civ = tile.civ;
        GreatProject gp = tile.greatProject;
        if (gp.isBuilding) { return; }
        float cost = gp.GetCost(tile, civ);
        if (civ.coins >= cost)
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.UpgradeGreatProj, civ.CivID);
            }
            else
            {
                gp.isBuilding = true;
                civ.coins -= cost;
            }
        }
    }
}
