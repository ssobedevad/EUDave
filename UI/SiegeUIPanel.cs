using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class SiegeUIPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI siegeProgress,diceRoll,fortLevel,siegeAbility;
    [SerializeField] Image siegeProgressFill,tickProgressFill,ownerCivCol;

    private void OnGUI()
    {
        if (Player.myPlayer.selectedTile != null && Player.myPlayer.selectedTile.underSiege && Player.myPlayer.selectedTile.siege != null)
        {
            
            Siege siege = Player.myPlayer.selectedTile.siege;
            siegeProgress.text = "Siege Progress: " + Mathf.Round(siege.progress * 100f) + "%";
            float fortDefence = (siege.target.localDefensiveness.value + siege.target.civ.fortDefence.value);
            if(siege.leaderCivID > -1)
            {
                fortDefence -= Game.main.civs[siege.leaderCivID].siegeAbility.value;
            }
            siegeAbility.text = Mathf.Round(fortDefence * 100f) + "%";
            fortLevel.text = "Level: " + siege.target.fortLevel;
            string fortLevelText = "Current fort level gives:\n";
            fortLevelText += "Dice Roll impact on siege progress * " + Mathf.Round(100f/((siege.target.fortLevel*2) + 0.5f)) + "%";
            fortLevel.transform.parent.GetComponent<HoverText>().text = fortLevelText;
            siegeProgressFill.fillAmount = siege.progress;
            int tickTime = (int)(24 * (1f + fortDefence));
            string tickSpeedText = "A siege tick happens every "+ Mathf.Round(tickTime * 100f/6f)/100f +" hours \n";
            tickProgressFill.fillAmount = (float)siege.tickTimer/(float)siege.tickTime;
            tickProgressFill.GetComponent<HoverText>().text = tickSpeedText;
            diceRoll.text = siege.progressRoll + (siege.artillery>0 ? "+" + siege.artillery/(1+siege.fortLevel) : "") + ((siege.siegeGeneral!=null &&siege.siegeGeneral.active)? "+" + siege.siegeGeneral.siegeSkill : "");
            if (siege.leaderCivID > -1)
            {
                ownerCivCol.color = Game.main.civs[siege.leaderCivID].c;
            }
            else
            {
                ownerCivCol.color = Color.black;
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
