using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TradeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI description;

    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        description.text = Mathf.Round(civ.TradeIncome()* 100f)/100f + "<sprite index=0>\n\n";
        TileData capital = Map.main.GetTile(civ.capitalPos);
        string capitalRegion = capital.tradeRegion;
        foreach (var region in civ.tradeRegions)
        {
            description.text += region + ": "+ Mathf.Round(Map.main.tradeRegions[region].GetTradeIncome(civ) * (region == capitalRegion ? 1f : 1f - civ.tradePenalty.value) *100f)/100f+ "<sprite index=0>"+ (region == capitalRegion ? "\nNo Penalty On Capital Node" : "\nNon Capital Node Penalty:" + Mathf.Round(-civ.tradePenalty.value*100f) + "%") + "\n";
            description.text += "Number of Civs (" + Map.main.tradeRegions[region].civs.Count + ") Gives: +"+Mathf.Round(100f + Map.main.tradeRegions[region].civs.Count * civ.tradeValPerCiv.value * 100f) + "%\n\n";
        }
        float tradeValue = (1f + civ.tradeValue.value);
        description.text += "\nMultiplied by: " + Mathf.Round(tradeValue * 100) + "%";
    }
}
