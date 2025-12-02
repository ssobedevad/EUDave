using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager main;

    [SerializeField] public GameObject BattleUIPrefab,CombatTextPrefab,DebugCirclePrefab,ArmyUIPrefab,FleetUIPrefab;
    [SerializeField] GameObject TileUI,BattleUI,NavalBattleUI,SiegeUI,ArmyUI,ArmyUIMulti,FleetUI;
    [SerializeField] public GameObject CivUI,PeaceDealUI,WarUI,countryChooser,toolbar,interestingCountries;
    [SerializeField] public List<GameObject> UI;
    [SerializeField] public List<GameObject> WorldSpaceUI;
    [SerializeField] public Transform worldCanvas,worldCanvasText,unitCanvas,battleTransform,eventTransform;
    [SerializeField] RectTransform Selector;
    [SerializeField] public GameObject mouseText,startGameButton,settings,saveGames;
    [SerializeField] public LoadingScreen loadingScreen;
    [SerializeField] public GameObject eventPrefab,battleResultPrefab,playerCTAPrefab;
    [SerializeField] public Sprite[] icons;

    Vector2 selectStart;
    public Rect selectRect;
    public bool dragStart = false;
    private void Awake()
    {
        main = this;
    }
    public void NewCombatText(string text,Vector2 pos,float duration = 1f,bool useGravity = false)
    {
        CombatText combatText = Instantiate(CombatTextPrefab, pos, Quaternion.identity, worldCanvas).GetComponent<CombatText>();
        combatText.text = text;
        combatText.timeLeft = duration;
        combatText.hasGravity = useGravity;
    }
    public static bool IsMouseOverUI(RectTransform rectTransform)
    {
        if (!rectTransform.gameObject.activeInHierarchy) { return false; }
        Rect position = GetGlobalPosition(rectTransform);
        return position.Contains(Input.mousePosition);
    }
    public static bool IsMouseOverUIWorld(RectTransform rectTransform)
    {
        if (!rectTransform.gameObject.activeInHierarchy) { return false; }
        Rect position = GetGlobalPosition(rectTransform);
        return position.Contains(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    private static Rect GetGlobalPosition(RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
    }
    private void OnGUI()
    {
        Player.myPlayer.isHoveringUI = false;
        foreach (GameObject go in UI) 
        {
            if (IsMouseOverUI(go.GetComponent<RectTransform>()))
            {
                Player.myPlayer.isHoveringUI = true;
            }
        }
        foreach (GameObject go in WorldSpaceUI)
        {
            if (IsMouseOverUIWorld(go.GetComponent<RectTransform>()))
            {
                Player.myPlayer.isHoveringUI = true;
            }
        }
        interestingCountries.SetActive(!Game.main.Started);
        toolbar.SetActive(Game.main.Started);
        countryChooser.SetActive(!Game.main.Started && Player.myPlayer.myCivID > -1);
        TileUI.SetActive(Player.myPlayer.selectedTile != null && Player.myPlayer.tileSelected && !Player.myPlayer.siegeSelected);
        BattleUI.SetActive(Player.myPlayer.selectedBattle != null && Player.myPlayer.selectedBattle.active);
        NavalBattleUI.SetActive(Player.myPlayer.selectedNavalBattle != null && Player.myPlayer.selectedNavalBattle.active);
        SiegeUI.SetActive(Player.myPlayer.selectedTile != null && Player.myPlayer.selectedTile.underSiege && Player.myPlayer.selectedTile.siege != null && Player.myPlayer.siegeSelected);
        ArmyUI.SetActive((Player.myPlayer.selectedArmies.Count == 1 && !Player.myPlayer.selectedArmies[0].inBattle) && !Player.myPlayer.siegeSelected);
        FleetUI.SetActive((Player.myPlayer.selectedFleets.Count == 1 && !Player.myPlayer.selectedFleets[0].inBattle) && !Player.myPlayer.siegeSelected);
        ArmyUIMulti.SetActive(Player.myPlayer.selectedArmies.Count > 1|| Player.myPlayer.selectedFleets.Count > 1);

        Player.myPlayer.selectedArmies.RemoveAll(i => i == null);
        Player.myPlayer.selectedFleets.RemoveAll(i => i == null);
        if (Player.myPlayer.selectedArmies.Count > 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            Player.myPlayer.selectedArmies.RemoveAt(0);
        }
        if (Player.myPlayer.selectedFleets.Count > 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            Player.myPlayer.selectedFleets.RemoveAt(0);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject.FindGameObjectsWithTag("Debug").ToList().ForEach(i=>Destroy(i));

        }

        if (!Game.main.Started) { return; }
        if (Input.GetMouseButtonDown(0) && !Player.myPlayer.isHoveringUI)
        {
            selectStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectRect = new Rect();
            dragStart = true;
        }
        else if (Input.GetMouseButton(0) && dragStart)
        {
            Vector2 mousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectRect.x = Mathf.Min(selectStart.x, mousePos.x);
            selectRect.y = Mathf.Min(selectStart.y, mousePos.y);
            selectRect.width = Mathf.Abs(mousePos.x - selectStart.x);
            selectRect.height = Mathf.Abs(mousePos.y - selectStart.y);
            Selector.position = selectRect.position;
            Selector.sizeDelta = selectRect.size/2;
            Selector.gameObject.SetActive(true);
        } 
        else if (Input.GetMouseButtonUp(0) && dragStart)
        {
            if(selectRect.size.magnitude > 1)
            {
                UIManager.main.CivUI.SetActive(false);
                Map.main.tileMapManager.DeselectTile();
                Player.myPlayer.selectedArmies.Clear();
                Player.myPlayer.selectedFleets.Clear();
                List<Army> possibleArmies = new List<Army>();
                List<Fleet> possibleFleets = new List<Fleet>();
                if (Player.myPlayer.myCivID == -1)
                {
                    foreach (var civ in Game.main.civs)
                    {
                        possibleArmies.AddRange(civ.armies);
                        possibleFleets.AddRange(civ.fleets);
                    }
                }
                else
                {
                    possibleArmies.AddRange(Player.myPlayer.myCiv.armies);
                    possibleFleets.AddRange(Player.myPlayer.myCiv.fleets);
                }
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    foreach (var army in possibleArmies)
                    {
                        Vector2 pos = Map.main.tileMapManager.tilemap.CellToWorld(army.pos);
                        if (selectRect.Contains(pos))
                        {
                            Player.myPlayer.selectedArmies.Add(army);
                        }

                    }
                }
                if (Player.myPlayer.selectedArmies.Count == 0)
                {
                    foreach (var fleet in possibleFleets)
                    {
                        Vector2 pos = Map.main.tileMapManager.tilemap.CellToWorld(fleet.pos);
                        if (selectRect.Contains(pos))
                        {
                            Player.myPlayer.selectedFleets.Add(fleet);
                        }
                    }
                }
            }

        }
        else
        {
            Selector.gameObject.SetActive(false);
            dragStart = false;
        }
    }
}
