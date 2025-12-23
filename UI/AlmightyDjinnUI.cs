using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlmightyDjinnUI : MonoBehaviour
{
    [SerializeField] RectTransform almightyDjinnScale;
    [SerializeField] RectTransform almightDjinnBlock;
    [SerializeField] HoverText hoverInfo;
    [SerializeField] TextMeshProUGUI djinnStanding;
    [SerializeField] Button swayLeft,swayRight;

    private void Start()
    {
        swayLeft.onClick.AddListener(delegate { Sway(true); });
        swayRight.onClick.AddListener(delegate { Sway(false); });
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        almightyDjinnScale.rotation = Quaternion.Euler(0, 0, civ.religiousPoints * -0.2f);
        almightDjinnBlock.localPosition = new Vector3(civ.religiousPoints * 4.2f, 0);
        hoverInfo.text = DjinnStandingHoverText(civ);
        djinnStanding.text = "Djinn Standing: " + Mathf.Round(civ.religiousPoints); 
    }

    public string DjinnStandingHoverText(Civilisation civ)
    {
        string text = "";
        text += (civ.GetWars().Count == 0 ? "Peace: " + -3 : "At War: " + 3) + "\n";
        text += "Decay: " + Mathf.Round(civ.religiousPoints * -3f) / 100f + "\n\n";
        Effect peace1 = Map.main.religions[4].religiousMechanicEffects[2];
        Effect peace2 = Map.main.religions[4].religiousMechanicEffects[3];

        Effect war1 = Map.main.religions[4].religiousMechanicEffects[0];
        Effect war2 = Map.main.religions[4].religiousMechanicEffects[1];        
  
        if (civ.religiousPoints < 0)
        {
            float peaceVal1 = civ.religiousPoints >= 0 ? 0f : -civ.religiousPoints * peace1.amount;
            float peaceVal2 = civ.religiousPoints >= 0 ? 0f : -civ.religiousPoints * peace2.amount;
            text += "Effect: " + peace1.name + " +" + Mathf.Round(peaceVal1) + "%\n";
            text += "Effect: " + peace2.name + " " + Mathf.Round(peaceVal2) + "%";
        }
        else
        {
            float warval1 = civ.religiousPoints <= 0 ? 0f : civ.religiousPoints * war1.amount;
            float warval2 = civ.religiousPoints <= 0 ? 0f : civ.religiousPoints/100f * war2.amount;
            text += "Effect: " + war1.name + " +" + Mathf.Round(warval1) + "%\n";
            text += "Effect: " + war2.name + " +" + Mathf.Round(warval2);
        }
        return text;
    }

    public void Sway(bool peace)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if(civ.prestige < 0 || civ.religiousPoints > 90 || civ.religiousPoints < -90) { return; }       
        civ.AddPrestige(-10);
        civ.religiousPoints = Mathf.Clamp(civ.religiousPoints + (peace ? -30 : 30), -100, 100);
    }
}
