using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.WSA;

public class Game : MonoBehaviour
{
    public static Game main;
    public Age gameTime;
    [SerializeField] public GameObject civTile;
    [SerializeField] public GameObject civTileSelected;
    [SerializeField] public Slider gameSpeed;
    [SerializeField] List<CivDataInit> civDatas = new List<CivDataInit>();
    public List<Civilisation> civs = new List<Civilisation>();
    public List<Battle> ongoingBattles = new List<Battle>();
    public List<War> ongoingWars = new List<War>();
    public List<Army> rebelFactions = new List<Army>();
    public List<RebelArmyStats> rebelStats = new List<RebelArmyStats>();
    public UnityEvent start = new UnityEvent();
    public UnityEvent tenMinTick = new UnityEvent();
    public UnityEvent hourTick = new UnityEvent();
    public UnityEvent dayTick = new UnityEvent();
    public UnityEvent monthTick = new UnityEvent();
    public UnityEvent yearTick = new UnityEvent();
    public float tenMinTickTime = 1f;
    public bool paused = true;
    private float tenMinTickTimer = 0f;
    public bool Started = false;
    public Camera _cam;
    public int highestDevelopment;
    private void Awake()
    {
        main = this;
        gameTime = new Age(0,0, 0, 0, 0,true);
        civs.Clear();
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
            civs.Add(civ);
        }
        for (int i = 0; i < civs.Count;i++)
        {
            civs[i].CivID = i;            
        }
        highestDevelopment = 1;
    }
    private void Update()
    {
        float[] speedVals = new float[] { 1f, 0.5f, 0.25f, 0.1f, 0f };
        tenMinTickTime = speedVals[(int)gameSpeed.value];
        ColorTiles();
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            paused = !paused; if (!Started)
            { 
                Started = true;
                start.Invoke();               
            } 
        }
        if (!paused) 
        {
            tenMinTickTimer += Time.deltaTime;
            if (tenMinTickTimer >= tenMinTickTime)
            {
                tenMinTick.Invoke();
                
                tenMinTickTimer = 0f;
            }
        }
    }
    private void Start()
    {       
        foreach (var civ in civs)
        {            
            civ.Init();
        }
        ColorTiles();
    }
    public void ColorTiles()
    {
        foreach(var tile in Map.main.tiles) 
        {
            if (tile == null) { continue; }
            if (Player.myPlayer.mapMode == -1)
            {
                if (DiplomacyUIPanel.main.diploCivID > -1 && UIManager.main.PeaceDealUI.activeSelf)
                {
                    if (tile.civID > -1)
                    {
                        PeaceDealUI peaceDealUI = UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>();
                        if (peaceDealUI == null || peaceDealUI.peaceDeal == null) { return; }
                        List<Vector3Int> selected = peaceDealUI.peaceDeal.provinces;
                        List<Vector3Int> possible = peaceDealUI.peaceDeal.possible;
                        Color civC = Color.black;
                        if (selected.Contains(tile.pos))
                        {
                            civC = Color.green;
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
                        Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 0)
            {
                if (tile.civID > -1)
                {
                    if (tile.occupied)
                    {
                        if (tile.selectedTileObj == null)
                        {
                            tile.selectedTileObj = Instantiate(civTileSelected, tile.worldPos(), Quaternion.identity, Map.main.tileMapManager.transform);
                        }
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
                        tile.selectedTileObj.GetComponent<SpriteRenderer>().color = civCO;
                    }
                    else if (tile.selectedTileObj != null)
                    {
                        Destroy(tile.selectedTileObj);
                    }
                    Color civC = tile.civ.c;
                    Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                    Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                }
                else
                {
                    Terrain terrain = tile.terrain;
                    if (terrain != null)
                    {
                        Color civC = terrain.c;
                        Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                        Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                    }
                }
            }
            else if (Player.myPlayer.mapMode == 1)
            {
                Terrain terrain = tile.terrain;
                if (terrain != null)
                {
                    Color civC = terrain.c;
                    Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                    Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                }
            }
            else if (Player.myPlayer.mapMode == 2)
            {
                ResourceType resource = tile.tileResource;
                if (resource != null)
                {
                    Color civC = resource.mapColor;
                    Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                    Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                }
            }
            else if (Player.myPlayer.mapMode == 3)
            {
                if (tile.civID > -1)
                {
                    float development = (float)tile.totalDev / (float)highestDevelopment;
                    Color civC = Color.Lerp(Color.red, Color.green, development);
                    Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                    Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
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
                    Map.main.tileMapManager.tilemap.SetTileFlags(tile.pos, TileFlags.None);
                    Map.main.tileMapManager.tilemap.SetColor(tile.pos, civC);
                }
            }
        }       
    }

}
