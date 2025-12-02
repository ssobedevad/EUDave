using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap,tilemapUnknown;
    [SerializeField] public TilemapCollider2D tilemapCollider2D;
    public Vector3Int selectedPos;
    public Vector3Int OldselectedPos;
    private GameObject selector;
    [SerializeField] private GameObject selectorPrefab;
    [SerializeField] public TileBase dropped, normal , raised,doubleRaised;

    private void OnMouseOver()
    {
        if (Player.myPlayer.isHoveringUI||Game.main.isSaveLoad) { return; }
        Vector3Int tilemapPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        TileData tileData = Map.main.GetTile(tilemapPos);
        if (Input.GetMouseButtonDown(1) && Game.main.Started)
        {          
            if(Player.myPlayer.selectedArmies.Count > 0 && Game.main.Started && Player.myPlayer.myCivID > -1)
            {
                foreach (var army in Player.myPlayer.selectedArmies)
                {
                    if(Player.myPlayer.myCivID == -1) { army.SetPath(tilemapPos);continue; }
                    Civilisation civ = Player.myPlayer.myCiv;
                    if (army.civID == civ.CivID || (civ.subjects.Contains(army.civID) && Game.main.civs[army.civID].libertyDesire < 50f))
                    {
                        army.SetPath(tilemapPos);
                    }
                }
            }
            else if (Player.myPlayer.selectedFleets.Count > 0 && Game.main.Started && Player.myPlayer.myCivID > -1)
            {
                foreach (var fleet in Player.myPlayer.selectedFleets)
                {
                    if (Player.myPlayer.myCivID == -1) { fleet.SetPath(tilemapPos); continue; }
                    Civilisation civ = Player.myPlayer.myCiv;
                    if (fleet.civID == civ.CivID || (civ.subjects.Contains(fleet.civID) && Game.main.civs[fleet.civID].libertyDesire < 50f))
                    {
                        fleet.SetPath(tilemapPos);
                    }
                }
            }
            else
            {             
                UIManager.main.CivUI.SetActive(true);
                Game.main.refreshMap = true;
                UIManager.main.CivUI.GetComponent<CivUIPanel>().OpenMenu(2);
                DiplomacyUIPanel.main.diploCivID = tileData.civID;
                DiplomacyUIPanel.main.CancelWarDec();
                DeselectTile();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if(UIManager.main.selectRect.size.magnitude < 1)
            {
                CheckTileClick();
            }
        }
    }
    public void CheckTileClick()
    {
        if (Player.myPlayer.isHoveringUI) { return; }
        Vector3Int tilemapPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Player.myPlayer.mapMode == -1)
        {
            PeaceDealUI peaceDealUI = UIManager.main.PeaceDealUI.GetComponent<PeaceDealUI>();
            if (peaceDealUI == null || peaceDealUI.peaceDeal == null) { return; }
            List<Vector3Int> selected = peaceDealUI.peaceDeal.provinces;
            List<Vector3Int> possible = peaceDealUI.peaceDeal.possible;
            if (selected.Contains(tilemapPos)) { peaceDealUI.peaceDeal.RemoveProvince(tilemapPos); }
            else if (possible.Contains(tilemapPos)) { peaceDealUI.peaceDeal.AddProvince(tilemapPos); }
        }
        else
        {
            if (tilemapPos == selectedPos)
            {
                DeselectTile();
                return;
            }
            SelectTile(tilemapPos);
        }        
    }
    public void DeselectTile()
    {
        if (selector != null)
        {
            Destroy(selector);
        }
        OldselectedPos = selectedPos;
        selectedPos = Vector3Int.zero;
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
    }
    public void SelectTile(Vector3Int tilemapPos)
    {
        TileData tileData = Map.main.GetTile(tilemapPos);
        OldselectedPos = selectedPos;
        selectedPos = tilemapPos;
        if (Game.main.Started)
        {
            UIManager.main.CivUI.SetActive(false);
            Player.myPlayer.tileSelected = true;
            Player.myPlayer.selectedTile = tileData;
            Player.myPlayer.siegeSelected = tileData.underSiege && !Player.myPlayer.siegeSelected;
            Player.myPlayer.selectedArmies.Clear();
            Player.myPlayer.selectedFleets.Clear();
            MoveSelector(tilemapPos);
        }
        //Debug.Log("On Tile " + tileData.armiesOnTile.Count);
        
        if (!Game.main.Started || Player.myPlayer.spectator)
        {
            Player.myPlayer.SelectCiv(Map.main.GetTile(selectedPos).civID);
        }
    }
    public void ClearSelection()
    {
        if (selectedPos != null)
        {
            Destroy(selector);
            OldselectedPos = Vector3Int.zero;
            selectedPos = Vector3Int.zero;
            Player.myPlayer.selectedTile = null;
            Player.myPlayer.tileSelected = false;
        }
    }
    void MoveSelector(Vector3Int pos)
    {
        if(selector == null)
        {
            selector = Instantiate(selectorPrefab, tilemap.CellToWorld(pos), Quaternion.identity);
        }
        else
        {
            selector.transform.position = tilemap.CellToWorld(pos);
        }
    }    
}
