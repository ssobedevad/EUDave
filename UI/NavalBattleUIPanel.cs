using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static FleetBattleLine;

public class NavalBattleUIPanel : MonoBehaviour
{
    [SerializeField] List<GameObject> attackerList, defenderList;
    [SerializeField] TextMeshProUGUI reservesA, reservesD, diceRollA, diceRollD, disciplineTextA, moraleTextA, tacticsTextA, disciplineTextD, moraleTextD, tacticsTextD, moraleFillTextA, moraleFillTextD;
    [SerializeField] TextMeshProUGUI terrainRollA, terrainRollD, bonusRollA, bonusRollD;
    [SerializeField] Image civColA, civColD, moraleFillA, moraleFillD;
    [SerializeField] Sprite[] seaSprites,boatSprites;

    Sprite GetBoatSprite(int index, int width)
    {
        if(width <= 1) { return boatSprites[0]; }
        if(width == 2) { return index == 0? boatSprites[1] : boatSprites[2]; }
        else
        {
            return index == 0 ? boatSprites[1] : index == width - 1 ? boatSprites[2] : boatSprites[3];
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.selectedNavalBattle != null && Player.myPlayer.selectedNavalBattle.active)
        {
            NavalBattle battle = Player.myPlayer.selectedNavalBattle;
            if (battle.AttackerDiceRollBonus() != 0)
            {
                terrainRollA.text = battle.AttackerDiceRollBonus() + "";
                terrainRollA.GetComponentInParent<Transform>().gameObject.SetActive(true);
            }
            else
            {
                terrainRollA.transform.parent.gameObject.SetActive(false);
            }
            if (battle.DefenderDiceRollBonus() != 0)
            {
                terrainRollD.text = battle.DefenderDiceRollBonus() + "";
                terrainRollD.GetComponentInParent<Transform>().gameObject.SetActive(true);
            }
            else
            {
                terrainRollD.transform.parent.gameObject.SetActive(false);
            }
            if (battle.attackerGeneral != null && battle.attackerGeneral.active)
            {
                bonusRollA.text = battle.attackerGeneral.combatSkill + "";
                bonusRollA.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                bonusRollA.transform.parent.gameObject.SetActive(false);
            }
            if (battle.defenderGeneral != null && battle.defenderGeneral.active)
            {
                bonusRollD.text = battle.defenderGeneral.combatSkill + "";
                bonusRollD.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                bonusRollD.transform.parent.gameObject.SetActive(false);
            }
            civColA.color = battle.attackerCiv.c;            
            civColD.color = battle.defenderCiv.c;          
            if (battle.attackingReserves != null && battle.attackingReserves.Count > 0)
            {
                float armyQ = 0;
                string attackerText = "";
                foreach (var reserve in battle.attackingReserves)
                {
                    armyQ += reserve.sailors;
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
                    armyQ += reserve.sailors;
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
            if (battle.attackerDiceRoll > 0)
            {
                diceRollA.text = "" + battle.attackerDiceRoll;
            }
            if (battle.defenderDiceRoll > 0)
            {
                diceRollD.text = "" + battle.defenderDiceRoll;
            }

            if (battle.attackingFrontLine != null && attackerList != null)
            {                
                for (int i = 0; i < battle.attackingFrontLine.width; i++)
                {
                    attackerList[i].GetComponent<Image>().color = Color.white;
                    attackerList[i].GetComponent<Image>().enabled = true;
                    attackerList[i].GetComponent<Image>().sprite = seaSprites[(i + Time.frameCount / 3) % seaSprites.Length];
                    BoatData boatData = battle.attackingFrontLine.boats[i];
                    if (boatData != null && boatData.boat.type > -1)
                    {
                        attackerList[i].GetComponentsInChildren<Image>()[1].sprite = GetBoatSprite(boatData.segmentID,boatData.boat.width);
                        if (boatData.boat.hullStrength > 0 && boatData.boat.sailors > 0)
                        {
                            attackerList[i].GetComponentsInChildren<Image>()[1].color = boatData.hullStrength <= 0 ? Color.black : Color.Lerp(Color.red, Color.green, (float)boatData.hullStrength / (float)boatData.hullStrengthMax);
                            attackerList[i].GetComponentsInChildren<Image>()[1].fillAmount = (float)boatData.boat.sailors / (float)boatData.boat.maxSailors;
                            attackerList[i].GetComponentInChildren<TextMeshProUGUI>().text = boatData.cannons > 0 ? boatData.cannons + "" : "";
                        }
                        else
                        {
                            attackerList[i].GetComponentsInChildren<Image>()[1].color = Color.clear;
                            attackerList[i].GetComponent<Image>().color = Color.white;
                            attackerList[i].GetComponent<Image>().fillAmount = 1;
                            attackerList[i].GetComponent<Image>().sprite = seaSprites[(i + Time.frameCount / 3) % seaSprites.Length];
                            attackerList[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        }
                    }
                    else
                    {
                        attackerList[i].GetComponentsInChildren<Image>()[1].color = Color.clear;
                        attackerList[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    }
                }
            }            
            if (battle.defendingFrontLine != null && defenderList != null)
            {

                for (int i = 0; i < battle.defendingFrontLine.width; i++)
                {
                    defenderList[i].GetComponent<Image>().color = Color.white;
                    defenderList[i].GetComponent<Image>().fillAmount = 1;
                    defenderList[i].GetComponent<Image>().sprite = seaSprites[(i + Time.frameCount / 3 + 1) % seaSprites.Length];
                    
                    BoatData boatData = battle.defendingFrontLine.boats[i];
                    if (boatData != null && boatData.boat.type > -1)
                    {
                        defenderList[i].GetComponentsInChildren<Image>()[1].sprite = GetBoatSprite(boatData.segmentID, boatData.boat.width);
                        if (boatData.boat.hullStrength > 0 && boatData.boat.sailors > 0)
                        {
                            defenderList[i].GetComponentsInChildren<Image>()[1].color = boatData.hullStrength <= 0 ? Color.black : Color.Lerp(Color.red, Color.green, (float)boatData.hullStrength / (float)boatData.hullStrengthMax);
                            defenderList[i].GetComponentsInChildren<Image>()[1].fillAmount = (float)boatData.boat.sailors / (float)boatData.boat.maxSailors;
                            defenderList[i].GetComponentInChildren<TextMeshProUGUI>().text = boatData.cannons > 0 ? boatData.cannons + "" : "";

                        }
                        else
                        {
                            defenderList[i].GetComponentsInChildren<Image>()[1].color = Color.black;
                            defenderList[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                        }
                    }
                    else
                    {
                        defenderList[i].GetComponentsInChildren<Image>()[1].color = Color.clear;
                        defenderList[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
                    }
                }
            }           
            disciplineTextA.text = Mathf.Round(battle.attackerCiv.discipline.v * 100f) + "%";
            moraleTextA.text = Mathf.Round(battle.attackerCiv.moraleMax.v * 100f) / 100f + "";
            tacticsTextA.text = Mathf.Round(battle.attackerCiv.militaryTactics.v * battle.attackerCiv.discipline.v * 100f) / 100f + "";
            disciplineTextD.text = Mathf.Round(battle.defenderCiv.discipline.v * 100f) + "%";
            moraleTextD.text = Mathf.Round(battle.defenderCiv.moraleMax.v * 100f) / 100f + "";
            tacticsTextD.text = Mathf.Round(battle.defenderCiv.militaryTactics.v * battle.defenderCiv.discipline.v * 100f) / 100f + "";            
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
