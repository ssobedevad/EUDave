using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public class TileMapManager : MonoBehaviour
{
    [SerializeField] public Tilemap tilemap,tilemapUnknown;
    [SerializeField] public TilemapCollider2D tilemapCollider2D;
    public Vector3Int selectedPos;
    public Vector3Int OldselectedPos;
    private GameObject selector;
    [SerializeField] private GameObject selectorPrefab;

    private void OnMouseOver()
    {
        if (Player.myPlayer.isHoveringUI) { return; }
        Vector3Int tilemapPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        TileData tileData = Map.main.GetTile(tilemapPos);
        if (Input.GetMouseButtonDown(1))
        {          
            if(Player.myPlayer.selectedArmies.Count > 0 && Game.main.Started && Player.myPlayer.myCivID > -1)
            {
                Player.myPlayer.selectedArmies.ForEach(i=>i.SetPath(tilemapPos));
            }
            else
            {             
                UIManager.main.CivUI.SetActive(true);
                UIManager.main.CivUI.GetComponent<CivUIPanel>().OpenMenu(2);
                DiplomacyUIPanel.main.diploCivID = tileData.civID;
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
    void DeselectTile()
    {
        Destroy(selector);
        OldselectedPos = selectedPos;
        selectedPos = Vector3Int.zero;
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
    }
    void SelectTile(Vector3Int tilemapPos)
    {
        TileData tileData = Map.main.GetTile(tilemapPos);
        OldselectedPos = selectedPos;
        selectedPos = tilemapPos;
        UIManager.main.CivUI.SetActive(false);
        Player.myPlayer.tileSelected = true;
        Player.myPlayer.selectedTile = tileData;
        Player.myPlayer.siegeSelected = tileData.underSiege && !Player.myPlayer.siegeSelected;
        Player.myPlayer.selectedArmies.Clear();
        
        if (!Game.main.Started || Player.myPlayer.spectator)
        {
            Player.myPlayer.SelectCiv(Map.main.GetTile(selectedPos).civID);
        }
        MoveSelector(tilemapPos);
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
