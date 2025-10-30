using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeneralSelectorUI : MonoBehaviour
{
    [SerializeField] Button buyGeneral,close;
    [SerializeField] Transform generalBack;
    [SerializeField] GameObject generalPrefab,armyPanel;
    List<GameObject> generalList = new List<GameObject>();
    private void Start()
    {
        buyGeneral.onClick.AddListener(BuyGeneral);
        close.onClick.AddListener(CloseMenu);
    }
    void BuyGeneral()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.milPower >= 50)
        {
            civ.milPower -= 50;
            civ.BuyGeneral();
        }
    }
    void CloseMenu()
    {
        gameObject.SetActive(false);
        armyPanel.SetActive(true);
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<General> generals = civ.generals.ToList();
        generals.RemoveAll(i => !i.active);
        while (generalList.Count != generals.Count)
        {
            if (generalList.Count > generals.Count)
            {
                int lastIndex = generalList.Count - 1;
                Destroy(generalList[lastIndex]);
                generalList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(generalPrefab, generalBack);
                Button button = item.GetComponent<Button>();
                int id = generalList.Count;
                button.onClick.AddListener(delegate { SelectGeneral(id); });
                generalList.Add(item);
            }
        }
        for (int i = 0; i < generals.Count; i++)
        {
            General general = generals[i];
            Image[] images = generalList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = generalList[i].GetComponentsInChildren<TextMeshProUGUI>();

            texts[0].text = general.name + " - " + general.meleeSkill + " " + general.flankingSkill + " " + general.rangedSkill + " " + general.siegeSkill + " " + general.maneuverSkill;
        }
    }
    void SelectGeneral(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<General> generals = civ.generals.ToList();
        generals.RemoveAll(i => !i.active);
        Player.myPlayer.selectedArmies[0].AssignGeneral(generals[id]);
        CloseMenu();
    }
}
