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
    List<string> saves = new List<string>();

    private void Awake()
    {
        saves = SaveLoadTestMP.LoadSaves();
    }
    private void Update()
    {
        if (Game.main.Started)
        {
            gameObject.SetActive(false);
        }
        saves = SaveLoadTestMP.LoadSaves();
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
                int index = civList.Count;
                civList.Add(item);

                Button[] buttons = item.GetComponentsInChildren<Button>();
                buttons[0].onClick.AddListener(delegate { OpenSave(index); });
                buttons[1].onClick.AddListener(delegate { RemoveSave(index); });
            }
        }
        for (int i = 0; i < saves.Count; i++)
        {
            Image[] images = civList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = civList[i].GetComponentsInChildren<TextMeshProUGUI>();
            string saveText = saves[i].Split("(")[1];
            string saveDate = saves[i].Split("{")[1];
            int civid = -1;
            int totalTicks = -1;
            if (int.TryParse(saveText.Split(")")[0], out civid) && civid > -1 && int.TryParse(saveDate.Split("}")[0], out totalTicks) && totalTicks > -1)
            {
                images[1].color = Game.main.civs[civid].c;
                texts[0].text = Game.main.civs[civid].civName + " " + new Age(totalTicks).ToDate();
            }
            else 
            { 
                images[1].color = Color.black;
                texts[0].text = "UNKNOWN";
            }

        }
        if(saves.Count == 0)
        {
            Image[] images = civList[0].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = civList[0].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = Color.black;
            texts[0].text = "No Saves";
        }
    }
    async void OpenSave(int i)
    {
        if (saves.Count > i)
        {
            UIManager.main.loadingScreen.display = "Loading Save File";
            UIManager.main.loadingScreen.currentPhase = "Init";
            UIManager.main.loadingScreen.gameObject.SetActive(true);
            await SaveGameManager.LoadSave(saves[i]);
            UIManager.main.startGameButton.SetActive(false);
            UIManager.main.loadingScreen.gameObject.SetActive(false);
        }
    }
    void RemoveSave(int i)
    {
        if (saves.Count > i)
        {
            SaveLoadTestMP.RemoveSave(saves[i]);
        }
    }
}
