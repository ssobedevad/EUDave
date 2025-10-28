using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class WarInfoUI : MonoBehaviour
{
    [SerializeField] Transform attackers, defenders, battles;
    [SerializeField] GameObject warParticpant, battleresult;
    [SerializeField] TextMeshProUGUI warName, warScore;
    [SerializeField] Image warGoal;

    public War war;

    List<GameObject> attackerList = new List<GameObject>();
    List<GameObject> defenderList = new List<GameObject>();
    List<GameObject> battleList = new List<GameObject>();

    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1 || war == null||!war.active) { gameObject.SetActive(false); return; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false); return;
        }
        Civilisation civ = Player.myPlayer.myCiv;
        bool isattacker = war.GetOpposingLeader(civ.CivID) == war.defenderCiv;
        warScore.text = Mathf.Round(war.warScore * (!isattacker ? -1 : 1)) + "%";
        bool controlWarGoal = (!isattacker ? !war.attackerControlsWarGoal : war.attackerControlsWarGoal);
        warGoal.color = controlWarGoal ? Color.green : Color.red;
        warGoal.sprite = controlWarGoal ? UIManager.main.icons[0] : UIManager.main.icons[1];
        warGoal.GetComponent<HoverText>().text = (controlWarGoal?"Controls War Goal ":"Does not Control War Goal -") + war.warGoalTimer;
        warName.text = war.GetName();
        while (attackerList.Count != war.attackerAllies.Count + 1)
        {
            if (attackerList.Count > war.attackerAllies.Count + 1)
            {
                int lastIndex = attackerList.Count - 1;
                Destroy(attackerList[lastIndex]);
                attackerList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(warParticpant, attackers);
                attackerList.Add(item);
            }
        }
        Civilisation attacker = war.attackerCiv;
        attackerList[0].GetComponentsInChildren<Image>()[1].color = attacker.c;
        attackerList[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = attacker.civName + " (Leader)";
        for (int i = 0; i < war.attackerAllies.Count; i++)
        {
            Civilisation ally = war.attackerAllies[i];
            attackerList[i+1].GetComponentsInChildren<Image>()[1].color = ally.c;
            attackerList[i+1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = ally.civName;
        }
        while (defenderList.Count != war.defenderAllies.Count + 1)
        {
            if (defenderList.Count > war.defenderAllies.Count + 1)
            {
                int lastIndex = defenderList.Count - 1;
                Destroy(defenderList[lastIndex]);
                defenderList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(warParticpant, defenders);
                defenderList.Add(item);
            }
        }
        Civilisation defender = war.defenderCiv;
        defenderList[0].GetComponentsInChildren<Image>()[1].color = defender.c;
        defenderList[0].GetComponentsInChildren<TextMeshProUGUI>()[0].text = defender.civName + " (Leader)";
        for (int i = 0; i < war.defenderAllies.Count; i++)
        {
            Civilisation ally = war.defenderAllies[i];
            defenderList[i + 1].GetComponentsInChildren<Image>()[1].color = ally.c;
            defenderList[i + 1].GetComponentsInChildren<TextMeshProUGUI>()[0].text = ally.civName;
        }
        while (battleList.Count != war.battleResults.Count)
        {
            if (battleList.Count > war.battleResults.Count)
            {
                int lastIndex = battleList.Count - 1;
                Destroy(battleList[lastIndex]);
                battleList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(warParticpant, battles);
                battleList.Add(item);
            }
        }

        for (int i = 0; i < war.battleResults.Count; i++)
        {
            float result = war.battleResults[i];
            battleList[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = "Battle: " +(isattacker ? result : -result) + "";
        }
    }
}
