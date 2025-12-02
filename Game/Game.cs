using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class Game : MonoBehaviour
{
    public static Game main;
    public Age gameTime;
    [SerializeField] public GameObject civTile;
    [SerializeField] public GameObject civTileSelected;
    [SerializeField] List<CivDataInit> civDatas = new List<CivDataInit>();
    public List<Civilisation> civs = new List<Civilisation>();
    public List<Battle> ongoingBattles = new List<Battle>();
    public List<NavalBattle> ongoingNavalBattles = new List<NavalBattle>();
    public List<War> ongoingWars = new List<War>();
    public UnityEvent start = new UnityEvent();
    public UnityEvent tenMinTick = new UnityEvent();
    public UnityEvent hourTick = new UnityEvent();
    public UnityEvent dayTick = new UnityEvent();
    public UnityEvent monthTick = new UnityEvent();
    public UnityEvent yearTick = new UnityEvent();
    public float tenMinTickTime = 1f;
    public float AI_MAX_AGGRESSIVENESS = 100f;
    public bool replaceSave = true;
    public bool paused = true;
    private float tenMinTickTimer = 0f;
    public bool Started = false;
    public Camera _cam;
    public int highestDevelopment;
    public float highestIncome;
    public bool refreshMap;
    public int gameSpeed = 0;
    public string saveGameName;
    public bool isSaveLoad;
    private void Awake()
    {
        main = this;
        gameTime = new Age(0,0, 0, 0, 0,true);
        civs.Clear();
        dayTick.AddListener(RefreshTradeRegions);
        yearTick.AddListener(AutoSave);
        foreach(var civData in civDatas)
        {
            Civilisation civ = new Civilisation();
            civ.civName = civData.Name;
            civ.c = civData.c;
            civ.nationalIdeas = civData.ideas;
            civ.ruler = civData.ruler;
            civ.heir = civData.heir;
            civ.adminTech = civData.techLevel;
            civ.diploTech = civData.techLevel;
            civ.milTech = civData.techLevel;
            civ.religion = civData.religion;
            civ.government = civData.government;
            civ.overlordID = civData.overlordID;
            civ.reforms.Add(civData.startReform);
            civs.Add(civ);
        }
        for (int i = 0; i < civs.Count;i++)
        {
            civs[i].CivID = i;            
        }
        highestDevelopment = 1;
        refreshMap = true;
        float maxAggro = PlayerPrefs.GetFloat("AIAGGRO");
        int replaceSaveVal = PlayerPrefs.GetInt("ReplaceSave");
        if (maxAggro > 0)
        {
            AI_MAX_AGGRESSIVENESS = maxAggro;
        }
        replaceSave = replaceSaveVal == 0;
    }
    async void AutoSave()
    {
        isSaveLoad = true;
        UIManager.main.loadingScreen.display = "Autosaving";
        UIManager.main.loadingScreen.currentPhase = "Init";
        UIManager.main.loadingScreen.gameObject.SetActive(true);
        await SaveGameManager.SaveSave();
        UIManager.main.loadingScreen.gameObject.SetActive(false);
        isSaveLoad = false;
    }
    public void StartGame()
    {
        if (!Started)
        {
            Started = true;
            start.Invoke();
            RefreshTradeRegions();
        }
    }
    public void ResetGame()
    {
        gameTime.Reset();

    }
    private void Update()
    {
        float[] speedVals = new float[] { 1f/3f, 1f/6f, 1f/12f, 1f/30f, 0f };
        tenMinTickTime = speedVals[(int)gameSpeed];
        if (Input.GetKeyDown(KeyCode.Space) && Game.main.Started)
        { 
            paused = !paused; 
        }
        if (!paused && !isSaveLoad) 
        {
            tenMinTickTimer += Time.deltaTime;
            if (tenMinTickTimer >= tenMinTickTime)
            {
                tenMinTick.Invoke();
                
                tenMinTickTimer = 0f;
            }
        }
        ColorTiles();
    }
    void RefreshTradeRegions()
    {
        foreach(var tradeRegion in Map.main.tradeRegions.Values)
        {
            tradeRegion.Refresh();
        }
    }
    private void Start()
    {       
        foreach (var civ in civs)
        {            
            civ.Init();
        }
    }
    public bool Equal(Color a, Color b)
    {
        if(a.r != b.r) { return false; }
        if (a.g != b.g) { return false; }
        if (a.b != b.b) { return false; }
        if (a.a != b.a) { return false; }
        return true;
    }
    void SetSelectorColor(TileData tile,Color color)
    {
        if (!Equal(tile.selectorCol, color))
        {
            tile.selectedTileObj.color = color;
            tile.selectorCol = color;
        }
    }
    public void ColorTiles()
    {
        foreach(var tile in Map.main.tiles) 
        {
            if (tile == null) { continue; }
            if (refreshMap)
            {
                SetSelectorColor(tile, Color.clear);
            }
            if (Player.myPlayer.mapMode == -1)
            {
                if (DiplomacyUIPanel.main.diploCivID > -1 && UIManager.main.PeaceDealUI.activeSelf)
                {
                    if (tile.civID > -1 && Player.myPlayer.myCivID > -1)
                    {
                        Civilisation civ = Player.myPlayer.myCiv;
                        PeaceDealUI peaceDealUI = UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>();
                        if (peaceDealUI == null || peaceDealUI.peaceDeal == null) { return; }
                        List<Vector3Int> selected = peaceDealUI.peaceDeal.provinces;
                        List<int> civIDs = peaceDealUI.peaceDeal.civTo;
                        List<Vector3Int> possible = peaceDealUI.peaceDeal.possible;
                        Color civC = Color.black;
                        if (selected.Contains(tile.pos))
                        {
                            int index = selected.IndexOf(tile.pos);
                            civC = Game.main.civs[civIDs[index]].c;
                        }
                        else if (possible.Contains(tile.pos))
                        {
                            civC = Color.red;
                        }
                        else if (Player.myPlayer.myCivID == tile.civID)
                        {
                            civC = Color.blue;
                        }
                        else if (DiplomacyUIPanel.main.diploCivID == tile.civID)
                        {
                            civC = Color.gray;
                        }
                        else if (civ.atWarWith.Contains(tile.civID) && (peaceDealUI.peaceDeal.war.attackerCiv.CivID == DiplomacyUIPanel.main.diploCivID || peaceDealUI.peaceDeal.war.defenderCiv.CivID == DiplomacyUIPanel.main.diploCivID))
                        {
                            civC = Color.gray;
                        }
                        if (!Equal(tile.currentCol,civC))
                        {
                            tile.currentCol = civC;
                            Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 0)
            {
                if (tile.civID > -1)
                {
                    if (tile.occupied)
                    {                       
                        Color civCO;
                        if (tile.occupiedByID > -1)
                        {
                            civCO = civs[tile.occupiedByID].c;
                        }
                        else
                        {
                            civCO = Color.black;
                        }
                        civCO.a *= 0.4f;
                        SetSelectorColor(tile, civCO);
                    }
                    else
                    {
                        SetSelectorColor(tile, Color.clear);
                    }
                    Color civC = tile.civ.c;
                    if(tile.civ.overlordID > -1)
                    {
                        civC = Game.main.civs[tile.civ.overlordID].c;
                        civC.r *= 0.8f;
                        civC.g *= 0.8f;
                        civC.b *= 0.8f;
                    }

                    if (!Equal(tile.currentCol,civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = tile.Name;
                        tile.tileText.fontSize = 0.2f;
                    }
                }
                else
                {
                    Terrain terrain = tile.terrain;
                    if (terrain != null)
                    {
                        Color civC = terrain.c;

                        if (!Equal(tile.currentCol,civC))
                        {
                            tile.currentCol = civC;
                            Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 1)
            {
                Terrain terrain = tile.terrain;
                if (terrain != null)
                {
                    Color civC = terrain.c;

                    if (!Equal(tile.currentCol,civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        if (tile.civID > -1)
                        {
                            tile.tileText.text = terrain.name;
                            tile.tileText.fontSize = 0.2f;
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 2)
            {
                ResourceType resource = tile.tileResource;
                if (resource != null)
                {
                    Color civC = resource.mapColor;

                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        if (tile.civID > -1)
                        {
                            tile.tileText.text = resource.name;
                            tile.tileText.fontSize = 0.2f;
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 3)
            {
                if (tile.civID > -1)
                {
                    float development = (float)tile.totalDev / (float)highestDevelopment;
                    Color civC = Color.Lerp(Color.red, Color.green, development);

                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = ""+tile.totalDev;
                        tile.tileText.fontSize = 0.5f;
                    }
                }          
            }
            else if (Player.myPlayer.mapMode == 4)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.green;
                    if (tile.fortLevel > 0)
                    {
                        civC = Color.Lerp(Color.white, Color.black, tile.fortLevel / 2f); ;
                    }
                    else if (tile.hasZOC)
                    {
                        civC = Color.red;
                    }

                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = "";
                        tile.tileText.fontSize = 0.2f;
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 5)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.gray;
                    if (tile.religion > -1)
                    {
                        Religion religion = Map.main.religions[tile.religion];
                        civC = religion.c;
                        if (!Equal(tile.currentCol, civC))
                        {
                            tile.currentCol = civC;
                            Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                            tile.tileText.text = religion.name;
                            tile.tileText.fontSize = 0.2f;
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 6)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.gray;
                    if (tile.civ.government > -1)
                    {
                        GovernmentType government = Map.main.governmentTypes[tile.civ.government];
                        civC = government.c;

                        if (!Equal(tile.currentCol, civC))
                        {
                            tile.currentCol = civC;
                            Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                            tile.tileText.text = government.name;
                            tile.tileText.fontSize = 0.2f;
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 7)
            {
                if (tile.civID > -1)
                {                   
                    Color civC = Color.gray;
                    if (DiplomacyUIPanel.main != null && DiplomacyUIPanel.main.diploCivID > -1)
                    {
                        Civilisation civ = civs[DiplomacyUIPanel.main.diploCivID];
                        if (civ.CivID == tile.civID)
                        {
                            civC = Color.magenta;
                        }
                        else if (civ.atWarTogether.Contains(tile.civID))
                        {
                            civC = Color.green;
                        }
                        else if (civ.militaryAccess.Contains(tile.civID))
                        {
                            civC = Color.blue;
                        }
                        else if (civ.atWarWith.Contains(tile.civID))
                        {
                            civC = Color.red;
                        }
                        else if (civ.subjects.Contains(tile.civID))
                        {
                            civC = Color.cyan;
                        }
                        else if (civ.allies.Contains(tile.civID))
                        {
                            civC = Color.yellow;
                        }
                    }

                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = "";
                        tile.tileText.fontSize = 0.2f;
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 8)
            {
                if (tile.civID > -1)
                {
                    float control = tile.control/100f;
                    Color civC = Color.Lerp(Color.red, Color.green, control);

                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = "" + Mathf.Round(tile.control);
                        tile.tileText.fontSize = 0.5f;
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 9)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.gray;
                    if (DiplomacyUIPanel.main != null && DiplomacyUIPanel.main.diploCivID > -1)
                    {
                        Civilisation civ = civs[DiplomacyUIPanel.main.diploCivID];
                        if (civ.updateBorders || refreshMap)
                        {
                            if (tile.civID == civ.CivID)
                            {
                                civC = Color.green;
                            }                           
                            else if (civ.CanCoreTile(tile))
                            {
                                civC = Color.red;
                            }
                            if (tile.cores.Contains(civ.CivID))
                            {
                                SetSelectorColor(tile, Color.green);
                            }
                            else if (civ.claims.Contains(tile.pos))
                            {
                                SetSelectorColor(tile, Color.yellow);
                            }
                            if (!Equal(tile.currentCol, civC))
                            {
                                tile.currentCol = civC;
                                Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                                tile.tileText.text = "";
                                tile.tileText.fontSize = 0.2f;
                            }
                        }
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 10)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.gray;
                    if (tile.hasMarket)
                    {
                        if (tile.marketLevel == 1)
                        {
                            SetSelectorColor(tile, Color.green);
                        }
                        else if(tile.marketLevel == 2)
                        {
                            SetSelectorColor(tile, Color.blue);
                        }
                    }
                    if(tile.tradeRegion != "")
                    {
                        float lerp = (float)Map.main.tradeRegions.Keys.ToList().IndexOf(tile.tradeRegion)/ (float)Map.main.tradeRegions.Keys.Count;
                        civC = Color.Lerp(Color.yellow,Color.magenta,lerp);
                    }
                    if (!Equal(tile.currentCol, civC))
                    {
                        tile.currentCol = civC;
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                        tile.tileText.text = "" + tile.marketLevel;
                        tile.tileText.fontSize = 0.5f;
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 11)
            {
                if (tile.civID > -1)
                {
                    Color civC = Color.gray;
                    if (DiplomacyUIPanel.main != null && DiplomacyUIPanel.main.diploCivID > -1)
                    {
                        Civilisation civ = civs[DiplomacyUIPanel.main.diploCivID];
                        if (Army.HasAccess(tile.civID, civ))
                        {
                            civC = Color.yellow;
                        }
                        if (!Equal(tile.currentCol, civC))
                        {
                            tile.currentCol = civC;
                            Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                            tile.tileText.text = "";
                            tile.tileText.fontSize = 0.2f;
                        }
                    }
                }
            }
        }
        refreshMap = false;
    }

}
