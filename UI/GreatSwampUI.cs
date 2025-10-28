using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GreatSwampUI : MonoBehaviour
{
    [SerializeField] Image HungerFill;
    [SerializeField] Button feed1, feed2, feed3;
    [SerializeField] Button ask1, ask2, ask3;

    private void Start()
    {
        feed1.onClick.AddListener(delegate { Feed(0); });
        feed2.onClick.AddListener(delegate { Feed(1); });
        feed3.onClick.AddListener(delegate { Feed(2); });
        ask1.onClick.AddListener(delegate { Ask(0); });
        ask2.onClick.AddListener(delegate { Ask(1); });
        ask3.onClick.AddListener(delegate { Ask(2); });
    }
    void Feed(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        Feed(civ, id);
    }
    public static void Feed(Civilisation civ, int id)
    {
        if (civ.religion != 3) { return; }
        if (id == 0)
        {
            if (civ.avaliablePopulation < civ.GetTotalMaxPopulation() * 0.1f)
            {
                return;
            }
            civ.RemovePopulation((int)(civ.GetTotalMaxPopulation() * 0.1f));
            civ.religiousPoints = Mathf.Max(0, civ.religiousPoints - 30);
        }
        else if (id == 1)
        {
            if (civ.subjects.Count == 0)
            {
                return;
            }
            foreach (var subject in civ.subjects)
            {
                Civilisation sub = Game.main.civs[subject];
                if (sub.libertyDesire >= 50) { return; }
                if (sub.avaliablePopulation < sub.GetTotalMaxPopulation() * 0.1f)
                {
                    return;
                }
                sub.RemovePopulation((int)(sub.GetTotalMaxPopulation() * 0.1f));
                sub.libertyDesireTemp.IncreaseModifier("Sacrificed Our People", 30f, 1, Decay: true);
                civ.religiousPoints = Mathf.Max(0, civ.religiousPoints - 10);
            }
        }
        else if (id == 2)
        {
            if (!civ.heir.active && civ.stability > -3)
            {
                return;
            }
            civ.religiousPoints = Mathf.Max(0, civ.religiousPoints - 25);
            civ.AddStability(-1);
        }
    }
    void Ask(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        Ask(civ, id);
    }
    public static void Ask(Civilisation civ, int id)
    {
        if (civ.religion != 3) { return; }
        if (civ.religiousPoints >= 50) { return; }
        if (id == 0)
        {
            Effect effect = Map.main.religions[3].religiousMechanicEffects[3];
            if (civ.GetStat(effect.name).modifiers.Exists(i => i.name == "Administrative Benefits")) { return; }
            civ.ApplyCivModifier(effect.name, effect.amount, "Administrative Benefits", effect.type, effect.duration);
            civ.religiousPoints = Mathf.Min(100, civ.religiousPoints + 10);
        }
        else if (id == 1)
        {
            Effect effect = Map.main.religions[3].religiousMechanicEffects[4];
            if (civ.GetStat(effect.name).modifiers.Exists(i => i.name == "Diplomatic Benefits")) { return; }
            civ.ApplyCivModifier(effect.name, effect.amount, "Diplomatic Benefits", effect.type, effect.duration);
            civ.religiousPoints = Mathf.Min(100, civ.religiousPoints + 10);
        }
        else if (id == 2)
        {
            Effect effect = Map.main.religions[3].religiousMechanicEffects[5];
            if (civ.GetStat(effect.name).modifiers.Exists(i => i.name == "Military Benefits")) { return; }
            civ.ApplyCivModifier(effect.name, effect.amount, "Military Benefits", effect.type, effect.duration);
            civ.religiousPoints = Mathf.Min(100, civ.religiousPoints + 10);
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        HungerFill.fillAmount = civ.religiousPoints / 100f;
        HungerFill.transform.parent.GetComponent<HoverText>().text = HungerHoverText(civ);
        ask1.enabled = civ.religiousPoints < 50;
        ask2.enabled = civ.religiousPoints < 50;
        ask3.enabled = civ.religiousPoints < 50;
        feed1.enabled = civ.religiousPoints > 30;
        feed2.enabled = civ.religiousPoints > 10 && civ.subjects.Count > 0;
        feed3.enabled = civ.religiousPoints > 25 && civ.heir.active && civ.stability > -3;
    }
    public string HungerHoverText(Civilisation civ)
    {
        Effect cd = Map.main.religions[3].religiousMechanicEffects[0];
        Effect tv = Map.main.religions[3].religiousMechanicEffects[1];
        Effect gu = Map.main.religions[3].religiousMechanicEffects[2];
        float cdVal = civ.religiousPoints > 50f ? (civ.religiousPoints - 50f) / 50f * cd.amount : (civ.religiousPoints - 50f) / 500f * cd.amount;        
        float tvVal = civ.religiousPoints > 50f ? (civ.religiousPoints - 50f) / 50f * tv.amount : (civ.religiousPoints - 50f) / 500f * tv.amount;      
        float guVal = civ.religiousPoints > 50f ? (civ.religiousPoints - 50f) / 50f * gu.amount : (civ.religiousPoints - 50f) / 500f * gu.amount;        
        string text = "Hunger of " + Mathf.Round(civ.religiousPoints * 100f) / 100f + "%\n";
        text += "Gives: " + cd.name + " " + Modifier.ToString(Mathf.Round(cdVal * 100f) / 100f, civ.GetStat(cd.name)) + "\n";
        text += "Gives: " + tv.name + " " + Modifier.ToString(Mathf.Round(tvVal * 100f) / 100f, civ.GetStat(tv.name)) + "\n";
        text += "Gives: " + gu.name + " " + Modifier.ToString(Mathf.Round(guVal * 100f) / 100f, civ.GetStat(gu.name)) + "\n\n";
        text += "This Increases by " + Mathf.Round(civ.HungerDaily() * 100f) / 100f + " every day\n";
        text += "Base: 0.1\n";
        text += "Provinces: " + Mathf.Round(civ.civTiles.Count) / 100f + "\n";
        text += civ.remainingDiploRelations < 0 ? "Over Diplo relations: " + Mathf.Round(civ.remainingDiploRelations * -100f) / 100f + "\n" : "";
        return text;
    }
}
