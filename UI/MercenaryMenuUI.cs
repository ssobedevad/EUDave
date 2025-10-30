using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryMenuUI : MonoBehaviour
{
    [SerializeField] Transform listBack;
    [SerializeField] GameObject mercPrefab;
    List<GameObject> mercList = new List<GameObject>();
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<MercenaryGroup> possiblemercs = myCiv.GetPossibleMercs();
        while (mercList.Count != possiblemercs.Count)
        {
            if (mercList.Count > possiblemercs.Count)
            {
                int lastIndex = mercList.Count - 1;
                Destroy(mercList[lastIndex]);
                mercList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(mercPrefab, listBack);
                int index = mercList.Count;
                item.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyMerc(index); });
                mercList.Add(item);
            }
        }
        for(int i = 0; i < possiblemercs.Count; i++)
        {
            MercenaryGroup merc = possiblemercs[i];
            int regimentCount = merc.baseRegiments + merc.regimentsPerYearExtra * Game.main.gameTime.years;
            int cavCount = (int)(regimentCount * merc.cavalryPercent);
            float recruitCost = tile.GetMercRecruitCost(Map.main.mercenaries.ToList().IndexOf(merc));
            TextMeshProUGUI[] texts = mercList[i].GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = mercList[i].GetComponentsInChildren<Image>();
            texts[0].text = merc.Name;
            texts[1].text = regimentCount + "k (" + cavCount + "k cavalry)";
            string hoverText = "Recruit Cost: " + recruitCost + "<sprite index=0>\n";
            hoverText += "Maintenaince Cost Increase: +" + Mathf.Round(ArmyMaintainanceIncrease(myCiv, regimentCount - cavCount, cavCount) * 100f)/100f + "<sprite index=0>"; 
            mercList[i].GetComponentInChildren<HoverText>().text = hoverText;
        }
    }
    void BuyMerc(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<MercenaryGroup> possiblemercs = myCiv.GetPossibleMercs();
        MercenaryGroup merc = possiblemercs[id];
        tile.StartRecruitingMercenary(Map.main.mercenaries.ToList().IndexOf(merc));
    }

    public float ArmyMaintainanceIncrease(Civilisation civ, int infantry, int cavalry)
    {
        float original = civ.ArmyMaintainance();
        float armyCosts = 0f;
        foreach (var army in civ.armies)
        {
            foreach (var regiment in army.regiments)
            {
                float mult = regiment.mercenary ? (0.5f + Game.main.gameTime.years * 0.25f) : 1f;
                armyCosts += mult * civ.units[regiment.type].baseCost * (float)regiment.size / (float)regiment.maxSize * 0.25f / 12f;
            }
        }
        armyCosts += infantry * (0.5f + Game.main.gameTime.years * 0.25f) * civ.units[0].baseCost * 0.25f / 12f;
        armyCosts += cavalry * (0.5f + Game.main.gameTime.years * 0.25f) * civ.units[0].baseCost * 0.25f / 12f;
        if (civ.TotalMaxArmySize() / 1000f  + infantry + cavalry > civ.forceLimit.value)
        {
            float increase = (civ.forceLimit.value + (civ.TotalMaxArmySize() / 1000f+ infantry + cavalry - civ.forceLimit.value) * 2) / civ.forceLimit.value;
            armyCosts *= increase;
        }
        armyCosts *= (1f + civ.regimentCost.value);
        armyCosts *= (1f + civ.regimentMaintenanceCost.value);
        return armyCosts - original;
    }
}
