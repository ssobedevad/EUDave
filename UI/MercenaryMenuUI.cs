using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MercenaryMenuUI : MonoBehaviour
{
    [SerializeField] Transform listBack;
    [SerializeField] GameObject unitItem;
    List<GameObject> unitList = new List<GameObject>();
    int unitCount = 3;
    int shipCount = 3;
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<MercenaryGroup> possiblemercs = myCiv.GetPossibleMercs();
        if (!tile.isCoastal) { shipCount = 0; }
        else 
        {
            if (myCiv.techUnlocks.Contains("Transport")) { shipCount = 1; }
            if (myCiv.techUnlocks.Contains("Supply Ship")) { shipCount = 2; }
            if (myCiv.techUnlocks.Contains("Warship")) { shipCount = 3; }
        }
        unitCount = 1;
        if(myCiv.techUnlocks.Contains("Flanking Units")) { unitCount = 2; }
        if (myCiv.techUnlocks.Contains("Siege Units")) { unitCount = 3; }
        while (unitList.Count != possiblemercs.Count + unitCount + shipCount)
        {
            if (unitList.Count > possiblemercs.Count + unitCount + shipCount)
            {
                int lastIndex = unitList.Count - 1;
                Destroy(unitList[lastIndex]);
                unitList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(unitItem, listBack);
                int index = unitList.Count;
                item.GetComponentInChildren<Button>().onClick.AddListener(delegate { BuyUnit(index); });
                unitList.Add(item);
            }
        }
        for(int i = 0; i < possiblemercs.Count; i++)
        {
            MercenaryGroup merc = possiblemercs[i];
            int regimentCount = merc.baseRegiments + merc.regimentsPerYearExtra * Game.main.gameTime.years;
            int cavCount = (int)(regimentCount * merc.cavalryPercent);
            float recruitCost = tile.GetMercRecruitCost(Map.main.mercenaries.ToList().IndexOf(merc));
            TextMeshProUGUI[] texts = unitList[i].GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = unitList[i].GetComponentsInChildren<Image>();
            texts[0].text = merc.Name;
            texts[1].text = regimentCount + "k (" + cavCount + "k cavalry)";
            string hoverText = "Recruit Cost: " + recruitCost + "<sprite index=0>\n";
            hoverText += "Maintenaince Cost Increase: +" + Mathf.Round(ArmyMaintainanceIncrease(myCiv, regimentCount - cavCount, cavCount,0) * 100f)/100f + "<sprite index=0>"; 
            unitList[i].GetComponentInChildren<HoverText>().text = hoverText;
        }
        for (int i = 0; i < unitCount; i++)
        {
            int baseIndex = possiblemercs.Count + i;
            UnitType unit = myCiv.units[i];
            float recruitCost = tile.GetRecruitCost(i);
            TextMeshProUGUI[] texts = unitList[baseIndex].GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = unitList[baseIndex].GetComponentsInChildren<Image>();
            texts[0].text = "Raise " + unit.name;
            texts[1].text = 1 + "k";
            string hoverText = "Recruit Cost: " + recruitCost + "<sprite index=0>\n";
            hoverText += "Maintenaince Cost Increase: +" + Mathf.Round(ArmyMaintainanceIncrease(myCiv,i == 0 ? 1 : 0, i == 1 ? 1 : 0, i == 2 ? 1 : 0) * 100f) / 100f + "<sprite index=0>";
            unitList[baseIndex].GetComponentInChildren<HoverText>().text = hoverText;
        }
        for (int i = 0; i < shipCount; i++)
        {
            int baseIndex = possiblemercs.Count + unitCount + i;
            BoatType boat = myCiv.boats[i];
            float recruitCost = boat.baseCost;
            TextMeshProUGUI[] texts = unitList[baseIndex].GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = unitList[baseIndex].GetComponentsInChildren<Image>();
            texts[0].text = "Raise " + boat.name;
            texts[1].text = 1 + "k";
            string hoverText = "Recruit Cost: " + recruitCost + "<sprite index=0>\n";
            hoverText += "Maintenaince Cost Increase: +" + Mathf.Round(ArmyMaintainanceIncrease(myCiv, i == 0 ? 1 : 0, i == 1 ? 1 : 0, i == 2 ? 1 : 0) * 100f) / 100f + "<sprite index=0>";
            unitList[baseIndex].GetComponentInChildren<HoverText>().text = hoverText;
        }
    }
    void BuyUnit(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (!Player.myPlayer.tileSelected) { return; }
        TileData tile = Player.myPlayer.selectedTile;
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<MercenaryGroup> possiblemercs = myCiv.GetPossibleMercs();
        if (id < possiblemercs.Count)
        {
            MercenaryGroup merc = possiblemercs[id];
            tile.StartRecruitingMercenary(Map.main.mercenaries.ToList().IndexOf(merc));
        }
        else if(id < possiblemercs.Count + unitCount)
        {
            int unitID = id - possiblemercs.Count;
            tile.StartRecruiting(unitID);
        }
        else if (id < possiblemercs.Count + unitCount + shipCount)
        {
            int unitID = id - possiblemercs.Count - unitCount;
            tile.StartRecruitingBoat(unitID);
        }
    }

    public float ArmyMaintainanceIncrease(Civilisation civ, int infantry, int cavalry,int artillery)
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
        armyCosts += cavalry * (0.5f + Game.main.gameTime.years * 0.25f) * civ.units[1].baseCost * 0.25f / 12f;
        armyCosts += artillery * (0.5f + Game.main.gameTime.years * 0.25f) * civ.units[2].baseCost * 0.25f / 12f;
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
