using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalDeitiesUI : MonoBehaviour
{
    [SerializeField] Button[] deities;

    private void Start()
    {
        for(int i = 0; i < deities.Length; i++)
        {
            int index = i;
            Effect effect = Map.main.religions[2].religiousMechanicEffects[index];
            deities[i].onClick.AddListener(delegate { DeityClick(index); });
            deities[i].GetComponent<HoverText>().text = effect.GetHoverText(Game.main.civs[0]);
        }
    }
    public static void SelectDeity(Civilisation civ, int id)
    {
        if (civ.religiousPoints > -1 || civ.religion != 2) { return; }
        Effect newE = Map.main.religions[2].religiousMechanicEffects[id];
        civ.religiousPoints = id;
        civ.ApplyCivModifier(newE.name, newE.amount, "Deity", newE.type);
    }
    public static void DeityClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        SelectDeity(civ, id);
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        for (int i = 0; i < deities.Length; i++)
        {
            int index = i;
            deities[i].GetComponent<Image>().color = ((int)civ.religiousPoints == i ? Color.green : Color.red);
        }
    }
}
