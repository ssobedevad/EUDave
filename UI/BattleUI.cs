using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI attacker, defender;
    [SerializeField] Image attackerImage, defenderImage;
    [SerializeField] Button button;
    [SerializeField] Image moraleFillA, moraleFillD;
    [SerializeField] Image bonusA, bonusD;
    [SerializeField] Sprite plus, minus;
    public Battle battle;

    private void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }
    private void OnGUI()
    {
        if(battle != null && battle.active)
        {
            if (battle.AttackerRebels)
            {
                attackerImage.color = Color.black;
            }
            else
            {
                attackerImage.color = battle.attackerCiv.c;
            }
            if (battle.DefenderRebels)
            {
                defenderImage.color = Color.black;
            }
            else
            {
                defenderImage.color = battle.defenderCiv.c;
            }
            float amorale = battle.AverageMorale(true);
            float dmorale = battle.AverageMorale(false);
            float amaxmorale = battle.AverageMaxMorale(true);
            float dmaxmorale = battle.AverageMaxMorale(false);
            moraleFillA.fillAmount = amorale / amaxmorale;
            moraleFillD.fillAmount = dmorale/dmaxmorale;
            if(battle.AttackerDiceRollBonus() < 0)
            {
                bonusA.transform.parent.gameObject.SetActive(true);
                bonusA.sprite = minus;
                bonusA.color = Color.red;
            }
            else if (battle.AttackerDiceRollBonus() > 0)
            {
                bonusA.transform.parent.gameObject.SetActive(true);
                bonusA.sprite = plus;
                bonusA.color = Color.green;
            }
            else
            {
                bonusA.transform.parent.gameObject.SetActive(false);
            }
            if (battle.DefenderDiceRollBonus() < 0)
            {
                bonusD.transform.parent.gameObject.SetActive(true);
                bonusD.sprite = minus;
                bonusD.color = Color.red;
            }
            else if (battle.DefenderDiceRollBonus() > 0)
            {
                bonusD.transform.parent.gameObject.SetActive(true);
                bonusD.sprite = plus;
                bonusD.color = Color.green;
            }
            else
            {
                bonusD.transform.parent.gameObject.SetActive(false);
            }
            float armyQ = battle.GetSideSize(true);
            if (armyQ < 1000)
            {
                attacker.text = Mathf.Round(armyQ) + "";
            }
            else
            {
                attacker.text = Mathf.Round((armyQ / 1000f) * 10f) / 10f + "k";
            }
            armyQ = battle.GetSideSize(false);
            if (armyQ < 1000)
            {
                defender.text = Mathf.Round(armyQ) + "";
            }
            else
            {
                defender.text = Mathf.Round((armyQ/1000f) * 10f) / 10f + "k";
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void ButtonClick()
    {
        Player.myPlayer.selectedBattle = battle;
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
        UIManager.main.CivUI.SetActive(false);
        Player.myPlayer.selectedArmies.Clear();
    }

}
