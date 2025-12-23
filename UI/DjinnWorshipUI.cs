using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DjinnWorshipUI : MonoBehaviour
{
    [SerializeField] Image DjinnUnityFill;
    [SerializeField] Button djinnFavor,djnnFavorDev,djinnFavorRel;

    private void Start()
    {
        djinnFavor.onClick.AddListener(delegate { DjinnFavor(1); });
        djinnFavor.GetComponent<HoverText>().text = "Consume 50 Djinn Unity:\n" + "Gains 20% Siege Ability for 30<sprite index=12>";
        djnnFavorDev.onClick.AddListener(delegate { DjinnFavor(2); });
        djnnFavorDev.GetComponent<HoverText>().text = "Consume 50 Djinn Unity:\n" + "Gains -20% Development Cost for 30<sprite index=12>";
        djinnFavorRel.onClick.AddListener(delegate { DjinnFavor(3); });
        djinnFavorRel.GetComponent<HoverText>().text = "Consume 50 Djinn Unity:\n" + "Gains 25% Conversion Cost for 30<sprite index=12>";
    }
    void DjinnFavor(int index)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;       
        if (Game.main.isMultiplayer)
        {
            Game.main.multiplayerManager.CivExtraActionRpc(civ.CivID, MultiplayerManager.CivExtraActions.ReligiousMechanic, index);
        }
        else
        {
            DjinnFavor(civ, index);
        }
    }
    public static void DjinnFavor(Civilisation civ,int index)
    {
        if (civ.religion != 1) { return; }
        if (civ.religiousPoints > 50)
        {
            Effect[] effects = Map.main.religions[1].religiousMechanicEffects;
            if(civ.GetStat(effects[index].name).ms.Exists(i=>i.n == "Djinn Favor")){ return; }
            civ.ApplyCivModifier(effects[index].name, effects[index].amount,"Djinn Favor", effects[index].type,4320);
            civ.religiousPoints -= 50;
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        DjinnUnityFill.fillAmount = civ.religiousPoints / 100f;
        DjinnUnityFill.transform.parent.GetComponent<HoverText>().text = DjinnUnityHoverText(civ);
    }

    public string DjinnUnityHoverText(Civilisation civ)
    {
        Effect effect = Map.main.religions[1].religiousMechanicEffects[0];
        string text = "Djinn Unity of "+ Mathf.Round(civ.religiousPoints * 100f) / 100f + "\nGives: " + effect.name +" "+Modifier.ToString(Mathf.Round(effect.amount * civ.religiousPoints)/100f,civ.GetStat(effect.name)) + "\n\n";
        text += "This Increases by " + Mathf.Round(civ.DjinnUnityDaily() * 100f) / 100f +" every day\n";
        text += "Wars: " + Mathf.Round(civ.GetWars().Count * 2f) + "\n";
        text += "Army Tradition: " + Mathf.Round(civ.armyTradition * 5f) / 100f + "\n";
        text += "Army Size: " + Mathf.Round(civ.TotalArmySize() * 0.002f) / 100f + "\n";
        text += "Decay: " + Mathf.Round(civ.religiousPoints * -5f) / 100f;
        return text;
    }
}
