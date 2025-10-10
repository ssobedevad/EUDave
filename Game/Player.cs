using System.Collections;
using System.Collections.Generic;
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
    public Battle selectedBattle = null;
    [SerializeField] private GameObject hexSpriteMaskPrefab;
    [SerializeField] private GameObject spriteMaskMain;
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
    private void HideMap()
    {
        if (myCivID == -1) { spectator = true; }
        //Map.main.tileMapManager.tilemapUnknown.gameObject.SetActive(myCivID > -1);
    }
    public void SelectCiv(int civID)
    {
        if (civID != myCivID)
        {
            myCivID = civID;
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
