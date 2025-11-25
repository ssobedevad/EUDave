using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class Notification
{
    public string name;
    public Sprite icon;
    public string description;
    public TileData province;
    public bool isProvince;
    public int civMenu;
    public bool isCivMenu;

    public Notification(string name = "",Sprite icon = null,string description= "",TileData province = null,bool isProvince = false,int civMenu = -1,bool isCivMenu = false) 
    { 
        this.name = name;
        this.icon = icon;
        this.description = description;
        this.province = province;
        this.isProvince = isProvince;
        this.civMenu = civMenu;
    }
    public void OnClick()
    {
        if (isProvince)
        {
            Map.main.tileMapManager.SelectTile(province.pos);
            CameraController.main.rb.position = province.worldPos();
            return;
        }
        if (isCivMenu)
        {
            UIManager.main.CivUI.SetActive(true);
            UIManager.main.CivUI.GetComponent<CivUIPanel>().OpenMenu(civMenu);
            return;
        }
    }

    public string GetHoverText(Civilisation civ)
    {
        string text = name + "\n";
        text += description;
        return text;
    }
}
