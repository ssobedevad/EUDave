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
            mercList[i].GetComponentInChildren<HoverText>().text = "Recruit Cost: " + recruitCost + "<sprite index=0>";
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
}
