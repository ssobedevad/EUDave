using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class ArmyUIMultiPanel : MonoBehaviour
{
    [SerializeField] Transform armyTransform;
    public List<GameObject> armies = new List<GameObject>();
    [SerializeField] GameObject armyPrefab;
    [SerializeField] Button[] buttons;

    private void Start()
    {
        buttons[0].onClick.AddListener(CloseMenu);
        buttons[1].onClick.AddListener(MergeAll);
    }
    private void OnGUI()
    {
        if (Player.myPlayer.selectedArmies.Count > 1)
        {
            while (armies.Count != Player.myPlayer.selectedArmies.Count)
            {
                if (armies.Count > Player.myPlayer.selectedArmies.Count)
                {
                    int lastIndex = armies.Count - 1;
                    Destroy(armies[lastIndex]);
                    armies.RemoveAt(lastIndex);
                }
                else
                {
                    GameObject army = Instantiate(armyPrefab, armyTransform);
                    armies.Add(army);
                    army.GetComponentInChildren<Button>().onClick.AddListener(delegate { DeselectArmy(armies.IndexOf(army)); });
                    
                }
            }
            for(int i =0; i < Player.myPlayer.selectedArmies.Count; i++)
            {
                Army army = Player.myPlayer.selectedArmies[i];
                TextMeshProUGUI armyName = armies[i].GetComponentInChildren<TextMeshProUGUI>();
                armyName.text = "Army Name - " + Mathf.Round(army.ArmySize());
            }

        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    void DeselectArmy(int id)
    {
        Player.myPlayer.selectedArmies.RemoveAt(id);
    }
    void CloseMenu()
    {
        Player.myPlayer.selectedArmies.Clear();
    }
    void MergeAll()
    {
        List<Army> temp = Player.myPlayer.selectedArmies.ToList();
        for (int i = 1; i < temp.Count; i++)
        {
            Army army = temp[i];
            if (army.pos == temp[0].pos)
            {
                Player.myPlayer.selectedArmies.Remove(army);
                army.CombineInto(temp[0]);
            }
        }
    }
}
