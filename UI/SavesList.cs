using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavesList : MonoBehaviour
{
    [SerializeField] GameObject civListItemPrefab;
    [SerializeField] Transform civListBack;
    List<GameObject> civList = new List<GameObject>();
    List<SaveGameData> saves = new List<SaveGameData>();

    private void Awake()
    {
        saves = SaveLoad.LoadSaves();
    }
    private void Update()
    {
        if (Game.main.Started)
        {
            gameObject.SetActive(false);
        }
        int amount = Mathf.Max(1, saves.Count);
        
        while (civList.Count != amount)
        {
            if (civList.Count > amount)
            {
                int lastIndex = civList.Count - 1;
                Destroy(civList[lastIndex]);
                civList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(civListItemPrefab, civListBack);
                civList.Add(item);
            }
        }
        for (int i = 0; i < saves.Count; i++)
        {
            SaveGameData save = saves[i];
            Image[] images = civList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = civList[i].GetComponentsInChildren<TextMeshProUGUI>();
            if (save.civID > -1)
            {
                Civilisation civ = Game.main.civs[save.civID];             
                images[1].color = civ.c;
                texts[0].text = civ.civName;                
            }
            else
            {
                images[1].color = Color.black;
                texts[0].text = "Spectator";
            }
            texts[1].text = save.gameTime.ToString(true, true);

        }
        if(saves.Count == 0)
        {
            Image[] images = civList[0].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = civList[0].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = Color.black;
            texts[0].text = "No Saves";
            texts[1].text = "N/A";
        }
    }
}
