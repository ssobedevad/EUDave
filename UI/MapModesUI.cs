using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapModesUI : MonoBehaviour
{
    [SerializeField] Button[] buttons;

    private void Start()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            int mode = i;
            buttons[i].onClick.AddListener(delegate { SelectMapmode(mode); });
        }
        SelectMapmode(0);
    }
    void SelectMapmode(int mode)
    {
        Player.myPlayer.mapMode = mode;
        Game.main.refreshMap = true;
    }
}
