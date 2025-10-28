using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIPanel : MonoBehaviour
{
    [SerializeField] Transform attackers, defenders,attackersBack,defendersBack;
    public List<GameObject> attackerList, defenderList,attackerBackList,defenderBackList;
    [SerializeField] GameObject regimentPrefab;
    [SerializeField] TextMeshProUGUI reservesA, reservesD, diceRollA, diceRollD,disciplineTextA,moraleTextA,tacticsTextA, disciplineTextD, moraleTextD, tacticsTextD, moraleFillTextA,moraleFillTextD;
    [SerializeField] TextMeshProUGUI terrainRollA, terrainRollD, bonusRollA, bonusRollD;
    [SerializeField] Image civColA, civColD,moraleFillA,moraleFillD;
    [SerializeField] Sprite[] unitIcons;
    private void Start()
    {
        attackerList = new List<GameObject>();
        defenderList = new List<GameObject>();
        attackerBackList = new List<GameObject>();
        defenderBackList = new List<GameObject>();
    }
    private void OnGUI()
    {
        if(Player.myPlayer.selectedBattle != null && Player.myPlayer.selectedBattle.active)
        {
            Battle battle = Player.myPlayer.selectedBattle;
            if (battle.tile.terrain.attackerDiceRoll != 0)
            {
                terrainRollA.text = battle.tile.terrain.attackerDiceRoll + "";
                terrainRollA.GetComponentInParent<Transform>().gameObject.SetActive(true);
            }
            else 
            { 
                terrainRollA.transform.parent.gameObject.SetActive(false);
            }
            terrainRollD.transform.parent.gameObject.SetActive(false);
            bonusRollA.transform.parent.gameObject.SetActive(false);
            bonusRollD.transform.parent.gameObject.SetActive(false);
            if (battle.AttackerRebels)
            {
                civColA.color = Color.black;
            }
            else
            {
                civColA.color = battle.attackerCiv.c;
            }
            if (battle.DefenderRebels)
            {
                civColD.color = Color.black;
            }
            else
            {
                civColD.color = battle.defenderCiv.c;
            }
            if (battle.attackingReserves != null && battle.attackingReserves.Count > 0)
            {
                float armyQ = 0;
                string attackerText = "";
                foreach (var reserve in battle.attackingReserves)
                {
                    armyQ += reserve.size;
                }
                if (armyQ < 1000)
                {
                    attackerText = Mathf.Round(armyQ) + "";
                }
                else
                {
                    attackerText = Mathf.Round((armyQ / 1000f) * 10f) / 10f + "k";
                }
                reservesA.text = "Reserves: " + attackerText;
            }
            else
            {
                reservesA.text = "No Reserves";
            }
            if (battle.defendingReserves != null && battle.defendingReserves.Count > 0)
            {
                float armyQ = 0;
                string defenderText = "";
                foreach (var reserve in battle.defendingReserves)
                {
                    armyQ += reserve.size;
                }
                if (armyQ < 1000)
                {
                    defenderText = Mathf.Round(armyQ) + "";
                }
                else
                {
                    defenderText = Mathf.Round((armyQ / 1000f) * 10f) / 10f + "k";
                }
                reservesD.text = "Reserves: " + defenderText;
            }
            else
            {
                reservesD.text = "No Reserves";
            }
            if(battle.attackerDiceRoll > 0)
            {
                diceRollA.text = "" + battle.attackerDiceRoll;
            }
            if (battle.defenderDiceRoll > 0)
            {
                diceRollD.text = "" + battle.defenderDiceRoll;
            }

            if (battle.attackingFrontLine != null && attackerList != null)
            {
                while (attackerList.Count != battle.attackingFrontLine.width)
                {
                    if(attackerList.Count > battle.attackingFrontLine.width)
                    {
                        Destroy(attackerList[0]);
                        attackerList.RemoveAt(0);
                    }
                    else
                    {
                        attackerList.Add(Instantiate(regimentPrefab, attackers));
                    }
                }
                for(int i = 0; i <  battle.attackingFrontLine.width; i++)
                {
                    Regiment regiment = battle.attackingFrontLine.regiments[i];
                    if(regiment != null && regiment.type > -1)
                    {
                        attackerList[i].GetComponentsInChildren<Image>()[2].enabled = true;
                        attackerList[i].GetComponentsInChildren<Image>()[2].sprite = unitIcons[regiment.type];
                        if (regiment.size > 0)
                        {
                            attackerList[i].GetComponent<Image>().color = Color.Lerp(Color.red,Color.green, regiment.morale / regiment.maxMorale);
                            attackerList[i].GetComponent<Image>().fillAmount = (float)regiment.size / (float)regiment.maxSize;
                        }
                        else
                        {
                            attackerList[i].GetComponent<Image>().color = Color.red;
                            attackerList[i].GetComponent<Image>().fillAmount = 1;
                        }
                    }
                    else
                    {
                        attackerList[i].GetComponent<Image>().color = Color.gray;
                        attackerList[i].GetComponent<Image>().fillAmount = 1;
                        attackerList[i].GetComponentsInChildren<Image>()[2].enabled = false;
                    }
                }
            }
            if (battle.attackingBackLine != null && attackerBackList != null)
            {
                while (attackerBackList.Count != battle.attackingBackLine.width)
                {
                    if (attackerBackList.Count > battle.attackingBackLine.width)
                    {
                        Destroy(attackerBackList[0]);
                        attackerBackList.RemoveAt(0);
                    }
                    else
                    {
                        attackerBackList.Add(Instantiate(regimentPrefab, attackersBack));
                    }
                }
                for (int i = 0; i < battle.attackingBackLine.width; i++)
                {
                    Regiment regiment = battle.attackingBackLine.regiments[i];
                    if (regiment != null && regiment.type > -1)
                    {
                        attackerBackList[i].GetComponentsInChildren<Image>()[2].enabled = true;
                        attackerBackList[i].GetComponentsInChildren<Image>()[2].sprite = unitIcons[regiment.type];
                        if (regiment.size > 0)
                        {
                            attackerBackList[i].GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, regiment.morale / regiment.maxMorale);
                            attackerBackList[i].GetComponent<Image>().fillAmount = (float)regiment.size / (float)regiment.maxSize;
                        }
                        else
                        {
                            attackerBackList[i].GetComponent<Image>().color = Color.red;
                            attackerBackList[i].GetComponent<Image>().fillAmount = 1;
                        }
                    }
                    else
                    {
                        attackerBackList[i].GetComponent<Image>().color = Color.gray;
                        attackerBackList[i].GetComponent<Image>().fillAmount = 1;
                        attackerBackList[i].GetComponentsInChildren<Image>()[2].enabled = false;
                    }
                }
            }
            if (battle.defendingFrontLine != null && defenderList != null)
            {
                while (defenderList.Count != battle.defendingFrontLine.width)
                {
                    if (defenderList.Count > battle.defendingFrontLine.width)
                    {
                        Destroy(defenderList[0]);
                        defenderList.RemoveAt(0);
                    }
                    else
                    {
                        defenderList.Add(Instantiate(regimentPrefab, defenders));
                    }
                }
                for (int i = 0; i < battle.defendingFrontLine.width; i++)
                {
                    Regiment regiment = battle.defendingFrontLine.regiments[i];
                    if (regiment != null && regiment.type > -1)
                    {
                        defenderList[i].GetComponentsInChildren<Image>()[2].enabled = true;
                        defenderList[i].GetComponentsInChildren<Image>()[2].sprite = unitIcons[regiment.type];
                        if (regiment.size > 0)
                        {
                            defenderList[i].GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, regiment.morale / regiment.maxMorale);
                            defenderList[i].GetComponent<Image>().fillAmount = (float)regiment.size / (float)regiment.maxSize;
                            
                        }
                        else
                        {
                            defenderList[i].GetComponent<Image>().color = Color.red;
                            defenderList[i].GetComponent<Image>().fillAmount = 1;
                        }
                    }
                    else
                    {
                        defenderList[i].GetComponent<Image>().color = Color.gray;
                        defenderList[i].GetComponent<Image>().fillAmount = 1;
                        defenderList[i].GetComponentsInChildren<Image>()[2].enabled = false;
                    }
                }
            }
            if (battle.defendingBackLine != null && defenderBackList != null)
            {
                while (defenderBackList.Count != battle.defendingBackLine.width)
                {
                    if (defenderBackList.Count > battle.defendingBackLine.width)
                    {
                        Destroy(defenderBackList[0]);
                        defenderBackList.RemoveAt(0);
                    }
                    else
                    {
                        defenderBackList.Add(Instantiate(regimentPrefab, defendersBack));
                    }
                }
                for (int i = 0; i < battle.defendingBackLine.width; i++)
                {
                    Regiment regiment = battle.defendingBackLine.regiments[i];
                    if (regiment != null && regiment.type > -1)
                    {
                        defenderBackList[i].GetComponentsInChildren<Image>()[2].enabled = true;
                        defenderBackList[i].GetComponentsInChildren<Image>()[2].sprite = unitIcons[regiment.type];
                        if (regiment.size > 0)
                        {
                            defenderBackList[i].GetComponent<Image>().color = Color.Lerp(Color.red, Color.green, regiment.morale / regiment.maxMorale);
                            defenderBackList[i].GetComponent<Image>().fillAmount = (float)regiment.size / (float)regiment.maxSize;

                        }
                        else
                        {
                            defenderBackList[i].GetComponent<Image>().color = Color.red;
                            defenderBackList[i].GetComponent<Image>().fillAmount = 1;
                        }
                    }
                    else
                    {
                        defenderBackList[i].GetComponent<Image>().color = Color.gray;
                        defenderBackList[i].GetComponent<Image>().fillAmount = 1;
                        defenderBackList[i].GetComponentsInChildren<Image>()[2].enabled = false;
                    }
                }
            }
            moraleFillA.fillAmount = battle.AverageMorale(true) / battle.AverageMaxMorale(true);
            moraleFillTextA.text = "Morale: " + Mathf.Round(battle.AverageMorale(true) * 100f) / 100f + "/" + Mathf.Round(battle.AverageMaxMorale(true) * 100f) / 100f;
            moraleFillD.fillAmount = battle.AverageMorale(false) / battle.AverageMaxMorale(false);
            moraleFillTextD.text = "Morale: " + Mathf.Round(battle.AverageMorale(false) * 100f) / 100f + "/" + Mathf.Round(battle.AverageMaxMorale(false) * 100f) / 100f;

            if (battle.AttackerRebels)
            {
                disciplineTextA.text = Mathf.Round(battle.attackerRebelStats.discipline * 100f) + "%";
                moraleTextA.text = Mathf.Round(battle.attackerRebelStats.morale * 100f) / 100f + "";
                tacticsTextA.text = Mathf.Round(battle.attackerRebelStats.tactics * battle.attackerRebelStats.discipline * 100f) / 100f + "";
            }
            else
            {
                disciplineTextA.text = Mathf.Round(battle.attackerCiv.discipline.value * 100f) + "%";
                moraleTextA.text = Mathf.Round(battle.attackerCiv.moraleMax.value * 100f) / 100f + "";
                tacticsTextA.text = Mathf.Round(battle.attackerCiv.militaryTactics.value * battle.attackerCiv.discipline.value * 100f) / 100f + "";
            }
            if (battle.DefenderRebels)
            {
                disciplineTextD.text = Mathf.Round(battle.defenderRebelStats.discipline * 100f) + "%";
                moraleTextD.text = Mathf.Round(battle.defenderRebelStats.morale * 100f) / 100f + "";
                tacticsTextD.text = Mathf.Round(battle.defenderRebelStats.tactics * battle.defenderRebelStats.discipline * 100f) / 100f + "";
            }
            else
            {
                disciplineTextD.text = Mathf.Round(battle.defenderCiv.discipline.value * 100f) + "%";
                moraleTextD.text = Mathf.Round(battle.defenderCiv.moraleMax.value * 100f) / 100f + "";
                tacticsTextD.text = Mathf.Round(battle.defenderCiv.militaryTactics.value * battle.defenderCiv.discipline.value * 100f) / 100f + "";
            }    
        }
        else
        {
            gameObject.SetActive(false);
        }
    }   
}
