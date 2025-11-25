using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class InterestingCountriesUI : MonoBehaviour
{
    [SerializeField] Image[] icons;
    [SerializeField] int[] ids;

    private void Start()
    {
        for(int i = 0; i < icons.Length;i++)
        {
            Civilisation civilisation = Game.main.civs[ids[i]];
            Image icon = icons[i];
            icon.color = civilisation.c;
            HoverText text = icon.GetComponent<HoverText>();
            text.text = civilisation.civName;
            Button button = icon.GetComponent<Button>();
            int id = ids[i];
            button.onClick.AddListener(delegate { SelectCountry(id); });
        }
    }
    void SelectCountry(int id)
    {
        if (id != Player.myPlayer.myCivID)
        {
            Player.myPlayer.myCivID = id;
            Civilisation civ = Game.main.civs[id];
            CameraController.main.rb.position = Map.main.tileMapManager.tilemap.CellToWorld(civ.capitalPos);
        }
    }
}
