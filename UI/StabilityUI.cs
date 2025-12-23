using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StabilityUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI stability, overextension,globalUnrest,govCap;
    [SerializeField] Button stabButton,coreAll;
    [SerializeField] GameObject rebelFactionPrefab,corePrefab,rebelsBack,coresBack;
    [SerializeField] Transform rebelFactionTransform,coreTransform;
    public List<GameObject> rebelFactions = new List<GameObject>();
    public List<GameObject> cores = new List<GameObject>();
    bool coreMode;
    private void Awake()
    {
        stabButton.onClick.AddListener(StabClick);
        //overextension.transform.parent.GetComponent<Button>().onClick.AddListener(delegate { SetMode(true); });
        //globalUnrest.transform.parent.GetComponent<Button>().onClick.AddListener(delegate { SetMode(false); });
        coreAll.onClick.AddListener(CoreAll);
        SetMode(true);
    }
    void SetMode(bool mode)
    {
        coreMode = mode;
        rebelsBack.SetActive(!mode);
        coresBack.SetActive(mode);
    }
    void StabClick()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.adminPower >= civ.GetStabilityCost() && civ.stability < 3)
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.SpendAdmin, civ.GetStabilityCost());
                Game.main.multiplayerManager.CivActionRpc(civ.CivID, MultiplayerManager.CivActions.AddStability, 1);
            }
            else
            {
                civ.adminPower -= civ.GetStabilityCost();
                civ.AddStability(1);
            }
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        overextension.text = Mathf.Round(civ.overextension * 100f)/100f + "%";
        stability.text = Mathf.Round(civ.stability) + "<sprite index=6>";
        globalUnrest.text = Mathf.Round(civ.globalUnrest.v * 100f) / 100f + "<sprite index=11>";
        govCap.text = Mathf.Round(civ.governingCapacity * 100f) / 100f + "/" + Mathf.Round(civ.governingCapacityMax.v * 100f) / 100f;
        string text = "It will cost " + civ.GetStabilityCost() + "<sprite index=1> to boost stability by 1\n\n";
        text += "This is due to:\n";
        text += "Base 100<sprite index=1>\n";
        text += civ.stabilityCost.ToString();
        stabButton.GetComponent<HoverText>().text = text;
        stabButton.interactable = civ.stability < 3;
        
        List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
        tiles.RemoveAll(i => i.hasCore);
        while (cores.Count != tiles.Count)
        {
            if (cores.Count > tiles.Count)
            {
                int lastIndex = cores.Count - 1;
                Destroy(cores[lastIndex]);
                cores.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(corePrefab, coreTransform);
                Button coreButton = item.GetComponentInChildren<Button>();
                int id = cores.Count;
                coreButton.onClick.AddListener(delegate { CoreClick(id); });
                cores.Add(item);
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            TileData tile = tiles[i];
            Image[] images = cores[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = cores[i].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = civ.c;
            texts[0].text = tile.Name;
            texts[1].text = tile.totalDev + "";
            texts[2].text = (tile.totalDev * 0.8f) + "%";
            texts[3].text = tile.GetCoreCost() + "";
            texts[4].text = tile.needsCoring() ? "Core" : tile.coreTimer + "";
            cores[i].GetComponentInChildren<Button>().interactable = tile.needsCoring();
        }
        
    }
    void CoreClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
        if (tiles.Count < id) { return; }
        tiles.RemoveAll(i => i.hasCore);
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.TileActionRpc(tiles[id].pos, MultiplayerManager.TileActions.CoreConvertStatus, 0);
        }
        else
        {
            tiles[id].StartCore();
        }
    }
    void CoreAll()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<TileData> tiles = civ.GetAllCivTiles().ConvertAll(i => Map.main.GetTile(i));
        tiles.RemoveAll(i => i.hasCore);
        if(tiles.Count == 0) { return; }
        tiles.Sort((x,y)=>y.totalDev.CompareTo(x.totalDev));
        foreach(var tile in tiles)
        {
            if (Game.main.isMultiplayer)
            {
                Game.main.multiplayerManager.TileActionRpc(tile.pos, MultiplayerManager.TileActions.CoreConvertStatus, 0);
            }
            else
            {
                tile.StartCore();
            }
        }
    }
}
