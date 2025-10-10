using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivUIPanel : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject[] panels;
    [SerializeField] TextMeshProUGUI sectionName;
    private void Start()
    {
        for(int i =0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(delegate { OpenMenu(index); });
        }
    }
    private void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
        Player.myPlayer.siegeSelected = false;
    }
    public void OpenMenu(int index)
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(i == index);          
        }
        sectionName.text = panels[index].name;
    }
}
