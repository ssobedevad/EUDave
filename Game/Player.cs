using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    public static Player myPlayer;
    public int myCivID = -1;
    public bool spectator;
    public int mapMode = 0;
    public Civilisation myCiv => Game.main.civs[myCivID];
    public bool tileSelected = false;   
    public TileData selectedTile = null;
    public bool siegeSelected = false;
    public List<Army> selectedArmies = new List<Army>();
    public List<Fleet> selectedFleets = new List<Fleet>();
    public Battle selectedBattle = null;
    public NavalBattle selectedNavalBattle = null;
    [SerializeField] private GameObject hexSpriteMaskPrefab;
    [SerializeField] public LineRenderer armyMove;
    [SerializeField] public GameObject resoureIndicatorPrefab;
    public List<GameObject> spriteMasks = new List<GameObject>();
    public List<GameObject> resourceIndicators = new List<GameObject>();
    public bool isHoveringUI;
    public int diplomacyMenu = -1;
    private void Awake()
    {
        myPlayer = this;
        spectator = false;
        myCivID = -1;
        diplomacyMenu = -1;      
    }
    private void Start()
    {
        Game.main.start.AddListener(HideMap);
    }
    private void Update()
    {
        if(selectedArmies.Count != 1)
        {
            armyMove.gameObject.SetActive(false);
        }
    }
    private void HideMap()
    {
        if (myCivID == -1) { spectator = true; }
        //Map.main.tileMapManager.tilemapUnknown.gameObject.SetActive(myCivID > -1);
    }
    public void SelectCiv(int civID)
    {
        if (spectator)
        {
            if (civID != myCivID)
            {
                myCivID = civID;
            }
        }
        else
        {
            if (civID != myCivID && !Game.main.Started)
            {
                if (Game.main.isMultiplayer)
                {
                    Game.main.multiplayerManager.SelectCivRpc(civID, (int)NetworkManager.Singleton.LocalClientId);
                }
                else
                {
                    myCivID = civID;
                }
            }
        }

    }
    public bool isMouseValidClick()
    {
        if (isHoveringUI) { return false; }
        Vector3Int tilePos = Map.main.tileMapManager.tilemapUnknown.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Map.main.tileMapManager.tilemapUnknown.gameObject.activeSelf == false || Map.main.tileMapManager.tilemapUnknown.GetColor(tilePos) == Color.clear)
        {
            return true;
        }
        return false;
    }
    
}
