using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class ArmyUIProvince : MonoBehaviour
{
    [SerializeField] GameObject siegeCanvas,armyCanvas,tabPrefab,starPrefab;
    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI armyNum, siegePercent;
    [SerializeField] Image civColor, moraleFill, siegeFill, armyPanel,attrition,exiled;
    [SerializeField] Button armySelect, siegeSelect;
    [SerializeField] Transform tabsBack,starsBack;
    List<GameObject> tabsList = new List<GameObject>();
    List<GameObject> starsList = new List<GameObject>();

    public Army army;
    private void Start()
    {
        armySelect.onClick.AddListener(SelectArmy);
        siegeSelect.onClick.AddListener(SelectSiege);
    }
    private void OnGUI()
    {
        if (!Game.main.Started)
        {
            armyCanvas.SetActive(false);
            siegeCanvas.SetActive(false);
            panel.sizeDelta = new Vector2(0, 0);
            return;
        }
        if (army == null)
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
        transform.position = army.tile.worldPos();
        TileData tile = Map.main.GetTile(army.pos);
        if (tile.armiesOnTile.Count == 1||(army.civID == Player.myPlayer.myCivID && army.path.Count > 0))
        {
            armyCanvas.SetActive(true);            
            SetupTabs(0);
            if (army.general != null && army.general.active)
            {
                SetupStars(army.general.Stars());
            }
            else
            {
                SetupStars(0);
            }
            armyNum.text = armySizeText((int)army.ArmySize());
            MoraleFill();
            if (tile.underSiege && army.path.Count == 0 && tile.siege.armiesSieging.Contains(army))
            {
                SetupSiegePanel();
                siegeCanvas.SetActive(true);
            }
            else
            {
                panel.sizeDelta = new Vector2(130, 40);
                siegeCanvas.SetActive(false);
            }
            ColorArmyPanel(army);
            attrition.gameObject.SetActive(army.GetAttrition() > 0);
            string hoverText = "Attrition: " + Mathf.Round(army.GetAttrition() * 100f) / 100f + "%\n\n";
            hoverText += AttritionText();
            attrition.GetComponent<HoverText>().text = hoverText;
            exiled.gameObject.SetActive(army.exiled);
        }
        else
        {
            List<Army> armies = tile.armiesOnTile.ToList();
            armies.RemoveAll(i => i.inBattle);
            if(armies.Count == 0) 
            {
                armyCanvas.SetActive(false);
                siegeCanvas.SetActive(false);
                panel.sizeDelta = new Vector2(0, 0);
                attrition.gameObject.SetActive(false);
                exiled.gameObject.SetActive(false);
                SetupTabs(0);
                SetupStars(0);
                return; 
            }
            armies.Sort((x, y) => PriorityScore(x).CompareTo(PriorityScore(y)));
            Army priority = armies[0];
            if (priority == army)
            {
                armyCanvas.SetActive(true);
                SetupTabs(armies.Count);
                if(priority.general != null && priority.general.active)
                {
                    SetupStars(priority.general.Stars());
                }
                else
                {
                    SetupStars(0);
                }
                int size = 0;
                int index = 0;
                foreach(var armyOnTile in armies)
                {
                    if (armyOnTile.civID == priority.civID || (priority.civID > -1 && Game.main.civs[priority.civID].atWarTogether.Contains(armyOnTile.civID)))
                    {
                        size += (int)armyOnTile.ArmySize();
                    }
                    tabsList[index].GetComponent<Image>().color = GetTabColor(armyOnTile);
                    index++;
                }
                armyNum.text = armySizeText(size);
                MoraleFill();
                if (tile.underSiege && priority.path.Count == 0 && tile.siege.armiesSieging.Contains(priority))
                {
                    SetupSiegePanel();
                    siegeCanvas.SetActive(true);
                }
                else
                {
                    panel.sizeDelta = new Vector2(130, 40);
                    siegeCanvas.SetActive(false);
                }
                ColorArmyPanel(priority);
                attrition.gameObject.SetActive(army.GetAttrition() > 0);
                string hoverText = "Attrition: " + Mathf.Round(army.GetAttrition() * 100f) / 100f + "%\n\n";
                hoverText += AttritionText();
                attrition.GetComponent<HoverText>().text = hoverText;
                exiled.gameObject.SetActive(army.exiled);
            }
            else
            {
                armyCanvas.SetActive(false);
                siegeCanvas.SetActive(false);
                panel.sizeDelta = new Vector2(0, 0);
                attrition.gameObject.SetActive(false);
                exiled.gameObject.SetActive(false);
                SetupTabs(0);
                SetupStars(0);
                return;
            }
        }
    }
    string AttritionText()
    {
        string percent = "";       
        float armysize = army.ArmySize();
        if (army.civID > -1)
        {
            Civilisation civ = Game.main.civs[army.civID];
            foreach (var aot in army.tile.armiesOnTile)
            {
                if (civ.atWarTogether.Contains(aot.civID) && aot != army)
                {
                    armysize += aot.ArmySize();
                }
            }
            if (civ.atWarWith.Contains(army.tile.civID))
            {
                percent += "Base: 1%\n";
                percent += army.tile.fortLevel > 0 ? "From Fort Level: " + army.tile.fortLevel + "%\n" : "";
                percent += army.tile.localAttritionForEnemies.value > 0? "From Local Bonuses: " + army.tile.localAttritionForEnemies.value + "%\n" : "";
                percent += army.tile.civ.attritionForEnemies.value > 0 ? "From Global Bonuses: " + army.tile.civ.attritionForEnemies.value + "%\n" : "";
            }            
        }
        if (armysize / 1000f > army.tile.supplyLimit)
        {
            percent += "From Supply: " + Mathf.Round(100f * Mathf.Clamp((armysize / 1000f - army.tile.supplyLimit) * (10f / ((float)army.tile.supplyLimit)), 0f, 5f)) / 100f + "%\n";
        }
        if(army.civID > -1) 
        { Civilisation civ = Game.main.civs[army.civID]; percent += "Multiplied By: " + Mathf.Round(100f * (1f + civ.landAttrition.value)) + "%"; }
        return percent;
    }
    void SetupTabs(int amount)
    {
        while (tabsList.Count != amount)
        {
            if (tabsList.Count > amount)
            {
                int lastIndex = tabsList.Count - 1;
                Destroy(tabsList[lastIndex]);
                tabsList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(tabPrefab, tabsBack);
                tabsList.Add(item);
            }
        }
    }
    void SetupStars(int amount)
    {
        while (starsList.Count != amount)
        {
            if (starsList.Count > amount)
            {
                int lastIndex = starsList.Count - 1;
                Destroy(starsList[lastIndex]);
                starsList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(starPrefab, starsBack);
                starsList.Add(item);
            }
        }
    }
    void SetupSiegePanel()
    {
        panel.sizeDelta = new Vector2(260, 40);
        siegeCanvas.SetActive(true);
        Siege siege = army.tile.siege;
        siegeFill.fillAmount = (float)siege.tickTimer / (float)siege.tickTime;
        siegePercent.text = Mathf.Round(siege.progress * 100f) + "%";
    }
    void MoraleFill()
    {
        if (army.AverageMaxMorale() > 0)
        {
            moraleFill.fillAmount = army.AverageMorale() / army.AverageMaxMorale();
        }
        else
        {
            moraleFill.fillAmount = 0;
        }
    }
    float PriorityScore(Army target)
    {
        if (Player.myPlayer.myCivID > -1)
        {
            Civilisation myCiv = Player.myPlayer.myCiv;
            if (target.civID == Player.myPlayer.myCivID)
            {
                return 0f;
            }
            else if (myCiv.atWarWith.Contains(target.civID))
            {
                return 1f;
            }
            else if (myCiv.atWarTogether.Contains(target.civID))
            {
                return 2f;
            }
            else if (myCiv.allies.Contains(target.civID))
            {
                return 3f;
            }
            else
            {
                return 4f;
            }
        }
        else
        {
            return 5f;
        }
    }
    Color GetTabColor(Army target)
    {
        if (target.civID > -1 && Player.myPlayer.myCivID > -1)
        {
            Civilisation civ = Game.main.civs[target.civID];
            Civilisation myCiv = Game.main.civs[Player.myPlayer.myCivID];
            if (target.civID == myCiv.CivID)
            {
                return Player.myPlayer.selectedArmies.Contains(target) ? Color.yellow : Color.green;
            }
            else if (myCiv.atWarTogether.Contains(target.civID))
            {
                return Color.green;
            }
            else if (myCiv.atWarWith.Contains(target.civID))
            {
                return Color.red;
            }
            else if (myCiv.allies.Contains(target.civID))
            {
                return Color.blue;
            }
            else if (myCiv.subjects.Contains(target.civID))
            {
                return Color.magenta;
            }

            return Color.gray;
        }
        else
        {
            return Color.black;
        }
    }
    void ColorArmyPanel(Army leader)
    {
        if (leader.civID > -1)
        {
            Civilisation civ = Game.main.civs[leader.civID];
            civColor.color = civ.c;
            armyPanel.color = Color.grey;
            if (Player.myPlayer.myCivID > -1)
            {
                Civilisation myCiv = Game.main.civs[Player.myPlayer.myCivID];
                if (leader.civID == myCiv.CivID)
                {
                    armyPanel.color = Player.myPlayer.selectedArmies.Contains(army) ? Color.yellow : Color.green;
                }
                else if (myCiv.atWarTogether.Contains(leader.civID))
                {
                    armyPanel.color = Color.green;
                }
                else if (myCiv.atWarWith.Contains(leader.civID))
                {
                    armyPanel.color = Color.red;
                }
                else if (myCiv.allies.Contains(leader.civID))
                {
                    armyPanel.color = Color.blue;
                }
                else if (myCiv.subjects.Contains(leader.civID))
                {
                    armyPanel.color = Color.magenta;
                }
            }
        }
        else
        {
            civColor.color = Color.black;
            armyPanel.color = Color.red;
        }
    }
    string armySizeText(int armyQ)
    {
        string text =  armyQ + "";
        if(armyQ >= 1000)
        {
            text = Mathf.Round(armyQ * 10f) / 10000f + "k";
        }
        return text;
    }
    void SelectArmy()
    {
        if (!Game.main.Started && army.civID == Player.myPlayer.myCivID) { return; }
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedFleets.Clear();
        Player.myPlayer.selectedArmies.Add(army);
        UIManager.main.CivUI.SetActive(false);
        Map.main.tileMapManager.DeselectTile();
    }
    void SelectSiege()
    {
        if (!Game.main.Started) { return; }
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedFleets.Clear();
        Player.myPlayer.selectedTile = army.tile;
        Player.myPlayer.tileSelected = true;
        Player.myPlayer.siegeSelected = true;
    }
}
