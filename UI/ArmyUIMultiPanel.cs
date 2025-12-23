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
        else if(Player.myPlayer.selectedFleets.Count > 1)
        {
            while (armies.Count != Player.myPlayer.selectedFleets.Count)
            {
                if (armies.Count > Player.myPlayer.selectedFleets.Count)
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
            for (int i = 0; i < Player.myPlayer.selectedFleets.Count; i++)
            {
                Fleet army = Player.myPlayer.selectedFleets[i];
                TextMeshProUGUI armyName = armies[i].GetComponentInChildren<TextMeshProUGUI>();
                armyName.text = "Army Name - " + Mathf.Round(army.boats.Count);
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
        Player.myPlayer.selectedFleets.Clear();
    }
    void MergeAll()
    {
        List<Army> temp = Player.myPlayer.selectedArmies.ToList();
        if (temp.Count > 1)
        {
            Army baseArmy = temp[0];            
            for (int i = 1; i < temp.Count; i++)
            {
                Army army = temp[i];
                if (army.pos == baseArmy.pos)
                {
                    Player.myPlayer.selectedArmies.Remove(army);
                    army.CombineInto(baseArmy);
                }
            }
        }

        List<Fleet> tempF = Player.myPlayer.selectedFleets.ToList();
        if (tempF.Count > 1)
        {
            Fleet baseFleet = tempF[0];
            for (int i = 1; i < temp.Count; i++)
            {
                Fleet fleet = tempF[i];
                if (fleet.pos == tempF[0].pos)
                {
                    Player.myPlayer.selectedFleets.Remove(fleet);
                    fleet.CombineInto(baseFleet);
                }
            }
        }
    }
}
