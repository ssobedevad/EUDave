using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class FleetUIProvince : MonoBehaviour
{
    [SerializeField] GameObject siegeCanvas,armyCanvas,tabPrefab,starPrefab;
    [SerializeField] RectTransform panel;
    [SerializeField] TextMeshProUGUI armyNum, siegePercent;
    [SerializeField] Image civColor, moraleFill, siegeFill, armyPanel,attrition,exiled;
    [SerializeField] Button armySelect, siegeSelect;
    [SerializeField] Transform tabsBack,starsBack;
    List<GameObject> tabsList = new List<GameObject>();
    List<GameObject> starsList = new List<GameObject>();

    public Fleet fleet;
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
        if (fleet == null)
        {
            UIManager.main.WorldSpaceUI.Remove(gameObject);
            Destroy(gameObject); 
            return; 
        }
        if (fleet.inBattle) 
        {
            armyCanvas.SetActive(false);
            siegeCanvas.SetActive(false);
            panel.sizeDelta = new Vector2(0, 0);
            return;
        }
        transform.position = fleet.tile.worldPos();
        TileData tile = Map.main.GetTile(fleet.pos);
        if(tile.portTile != Vector3Int.zero)
        {
            Vector3 portPos = (transform.position + Map.main.GetTile(tile.portTile).worldPos()) / 2;
            transform.position = portPos;
        }
        if (tile.fleetsOnTile.Count == 1||(fleet.civID == Player.myPlayer.myCivID && fleet.path.Count > 0))
        {
            armyCanvas.SetActive(true);            
            SetupTabs(0);
            if (fleet.general != null && fleet.general.active)
            {
                SetupStars(fleet.general.Stars());
            }
            else
            {
                SetupStars(0);
            }
            armyNum.text = fleet.boats.Count + "";
            MoraleFill();
            if (tile.underSiege && fleet.path.Count == 0)
            {
                SetupSiegePanel();
                siegeCanvas.SetActive(true);
            }
            else
            {
                panel.sizeDelta = new Vector2(130, 40);
                siegeCanvas.SetActive(false);
            }
            ColorArmyPanel(fleet);
            exiled.gameObject.SetActive(fleet.exiled);
        }
        else
        {
            List<Fleet> fleets = tile.fleetsOnTile.ToList();
            fleets.RemoveAll(i => i.inBattle);
            if(fleets.Count == 0) 
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
            fleets.Sort((x, y) => PriorityScore(x).CompareTo(PriorityScore(y)));
            Fleet priority = fleets[0];
            if (priority == fleet)
            {
                armyCanvas.SetActive(true);
                SetupTabs(fleets.Count);
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
                foreach(var armyOnTile in fleets)
                {
                    if (armyOnTile.civID == priority.civID || (priority.civID > -1 && Game.main.civs[priority.civID].atWarTogether.Contains(armyOnTile.civID)))
                    {
                        size += (int)armyOnTile.boats.Count;
                    }
                    tabsList[index].GetComponent<Image>().color = GetTabColor(armyOnTile);
                    index++;
                }
                armyNum.text = size + "";
                MoraleFill();
                if (tile.underSiege && priority.path.Count == 0)
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
                exiled.gameObject.SetActive(fleet.exiled);
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
        Siege siege = fleet.tile.siege;
        siegeFill.fillAmount = (float)siege.tickTimer / (float)siege.tickTime;
        siegePercent.text = Mathf.Round(siege.progress * 100f) + "%";
    }
    void MoraleFill()
    {
        if (fleet.TotalMaxSailors() > 0)
        {
            moraleFill.fillAmount = fleet.TotalSailors() / fleet.TotalMaxSailors();
        }
        else
        {
            moraleFill.fillAmount = 0;
        }
    }
    float PriorityScore(Fleet target)
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
    Color GetTabColor(Fleet target)
    {
        if (target.civID > -1 && Player.myPlayer.myCivID > -1)
        {
            Civilisation civ = Game.main.civs[target.civID];
            Civilisation myCiv = Game.main.civs[Player.myPlayer.myCivID];
            if (target.civID == myCiv.CivID)
            {
                return Player.myPlayer.selectedFleets.Contains(target) ? Color.yellow : Color.green;
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
    void ColorArmyPanel(Fleet leader)
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
                    armyPanel.color = Player.myPlayer.selectedFleets.Contains(fleet) ? Color.yellow : Color.green;
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
    void SelectArmy()
    {
        if (!Game.main.Started || Game.main.isSaveLoad) { return; }
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedFleets.Clear();
        Player.myPlayer.selectedFleets.Add(fleet);
        UIManager.main.CivUI.SetActive(false);
        Map.main.tileMapManager.DeselectTile();
    }
    void SelectSiege()
    {
        if (!Game.main.Started || Game.main.isSaveLoad) { return; }
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedFleets.Clear();
        Player.myPlayer.selectedTile = fleet.tile;
        Player.myPlayer.tileSelected = true;
        Player.myPlayer.siegeSelected = true;
    }
}
