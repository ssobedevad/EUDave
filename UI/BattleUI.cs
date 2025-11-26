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
    public NavalBattle navalBattle;

    private void Start()
    {
        button.onClick.AddListener(ButtonClick);
    }
    private void OnGUI()
    {
        if(battle != null && battle.active)
        {
            attackerImage.color = battle.attackerCiv.c;
            defenderImage.color = battle.defenderCiv.c;
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
        else if (navalBattle != null && navalBattle.active)
        {                        
            attackerImage.color = navalBattle.attackerCiv.c;                       
            defenderImage.color = navalBattle.defenderCiv.c;            
            float amorale = navalBattle.TotalSailors(true);
            float dmorale = navalBattle.TotalSailors(false);
            float amaxmorale = navalBattle.TotalMaxSailors(true);
            float dmaxmorale = navalBattle.TotalMaxSailors(false);
            moraleFillA.fillAmount = amorale / amaxmorale;
            moraleFillD.fillAmount = dmorale / dmaxmorale;
            if (navalBattle.AttackerDiceRollBonus() < 0)
            {
                bonusA.transform.parent.gameObject.SetActive(true);
                bonusA.sprite = minus;
                bonusA.color = Color.red;
            }
            else if (navalBattle.AttackerDiceRollBonus() > 0)
            {
                bonusA.transform.parent.gameObject.SetActive(true);
                bonusA.sprite = plus;
                bonusA.color = Color.green;
            }
            else
            {
                bonusA.transform.parent.gameObject.SetActive(false);
            }
            if (navalBattle.DefenderDiceRollBonus() < 0)
            {
                bonusD.transform.parent.gameObject.SetActive(true);
                bonusD.sprite = minus;
                bonusD.color = Color.red;
            }
            else if (navalBattle.DefenderDiceRollBonus() > 0)
            {
                bonusD.transform.parent.gameObject.SetActive(true);
                bonusD.sprite = plus;
                bonusD.color = Color.green;
            }
            else
            {
                bonusD.transform.parent.gameObject.SetActive(false);
            }
            float armyQ = navalBattle.attackingFrontLine.GetBoats().Count + navalBattle.attackingReserves.Count;
            if (armyQ < 1000)
            {
                attacker.text = Mathf.Round(armyQ) + "";
            }
            else
            {
                attacker.text = Mathf.Round((armyQ / 1000f) * 10f) / 10f + "k";
            }
            armyQ = navalBattle.defendingFrontLine.GetBoats().Count + navalBattle.defendingReserves.Count;
            if (armyQ < 1000)
            {
                defender.text = Mathf.Round(armyQ) + "";
            }
            else
            {
                defender.text = Mathf.Round((armyQ / 1000f) * 10f) / 10f + "k";
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void ButtonClick()
    {
        if (battle != null)
        {
            Player.myPlayer.selectedBattle = battle;
        }
        else if(navalBattle != null)
        {
            Player.myPlayer.selectedNavalBattle = navalBattle;
        }
        Player.myPlayer.selectedTile = null;
        Player.myPlayer.tileSelected = false;
        UIManager.main.CivUI.SetActive(false);
        Player.myPlayer.selectedArmies.Clear();
        Player.myPlayer.selectedFleets.Clear();
    }

}
