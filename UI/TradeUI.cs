using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        for(int i = 0; i < civ.tradeRegions.Length;i++)
        {
            if (civ.tradeRegions[i])
            {
                TradeRegion region = Map.main.tradeRegions.Values.ToArray()[i];
                description.text += region.name + ": " + Mathf.Round(region.GetTradeIncome(civ) * (region.name == capitalRegion ? 1f : 1f - civ.tradePenalty.v) * 100f) / 100f + "<sprite index=0>" + (region.name == capitalRegion ? "\nNo Penalty On Capital Node" : "\nNon Capital Node Penalty:" + Mathf.Round(-civ.tradePenalty.v * 100f) + "%") + "\n";
                description.text += "Number of Civs (" + region.civs.Count + ") Gives: +" + Mathf.Round(100f + region.civs.Count * civ.tradeValPerCiv.v * 100f) + "%\n\n";
            }
        }
        float tradeValue = (1f + civ.tradeValue.v);
        description.text += "\nMultiplied by: " + Mathf.Round(tradeValue * 100) + "%";
    }
}
