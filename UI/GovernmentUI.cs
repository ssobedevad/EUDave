using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GovernmentUI : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI effects,reformProgress;
    [SerializeField] GameObject reformTierPrefab, reformNamesPrefab,reformPrefab;
    [SerializeField] Transform reformTierTransform;
    public List<GameObject> reformNames = new List<GameObject>();
    public List<GameObject> reformTiers = new List<GameObject>();
    public List<List<GameObject>> reforms = new List<List<GameObject>>();

    void ReformClick(int tier, int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        GovernmentType governmentType = Map.main.governmentTypes[civ.government];
        if (governmentType.BaseReforms.Length == 0) { return; }
        GovernmentReformTier[] tiers = governmentType.BaseReforms;
        if (civ.reforms.Count == tier)
        {
            int cost = (40 + 40 * tier);
            if (civ.reformProgress >= cost)
            {
                civ.reformProgress -= cost;
                civ.reforms.Add(id);
                BuyReform(civ, tiers[tier].Reforms[id]);
            }
        }
        else if(civ.reforms.Count > tier)
        {
            int cost = 50;
            if (civ.reformProgress >= cost && civ.reforms[tier] != id)
            {
                civ.reformProgress -= cost;
                RemoveReform(civ, tiers[tier].Reforms[civ.reforms[tier]]);
                BuyReform(civ, tiers[tier].Reforms[id]);
                civ.reforms[tier] = id;
            }
        }
    }
    public static void AddGovernment(Civilisation civ, int gov,bool init = true)
    {
        GovernmentType governmentType = Map.main.governmentTypes[gov];
        if (governmentType != null)
        {
            for (int i = 0; i < governmentType.effects.Length; i++)
            {
                civ.ApplyCivModifier(governmentType.effects[i].name, governmentType.effects[i].amount, governmentType.name, governmentType.effects[i].type);
            }
            if (init)
            {
                for (int i = 0; i < civ.reforms.Count; i++)
                {
                    BuyReform(civ, governmentType.BaseReforms[i].Reforms[civ.reforms[i]]);
                }
            }
        }
    }
    public static void RemoveGovernment(Civilisation civ, int gov)
    {
        GovernmentType governmentType = Map.main.governmentTypes[gov];
        if (governmentType != null)
        {
            for (int i = 0; i < governmentType.effects.Length; i++)
            {
                civ.RemoveCivModifier(governmentType.effects[i].name, governmentType.name);
            }
            for (int i = 0; i < civ.reforms.Count; i++)
            {
                RemoveReform(civ, governmentType.BaseReforms[i].Reforms[civ.reforms[i]]);
            }
            civ.ClearReforms(civ.government == 3);
        }
    }
    public static void RemoveReform(Civilisation civ,GovernmentReform reform)
    {
        for (int i = 0; i < reform.effects.Length; i++)
        {
            civ.RemoveCivModifier(reform.effects[i].name,reform.name);
        }
    }
    public static void BuyReform(Civilisation civ,GovernmentReform reform)
    {
        for (int i = 0; i < reform.effects.Length; i++)
        {
            civ.ApplyCivModifier(reform.effects[i].name, reform.effects[i].amount, reform.name, reform.effects[i].type);
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        reformProgress.text = "Reform Progress: " + Mathf.Round(civ.reformProgress * 100f) / 100f + "\n +" + Mathf.Round(0.5f * civ.GetAverageControl() / 100f * (1f + civ.reformProgressGrowth.v) * 100f)/100f + " Per Day";
        icon.sprite = Map.main.governmentTypes[civ.government].sprite;
        effects.text = Map.main.governmentTypes[civ.government].GetHoverText(civ);
        GovernmentType governmentType = Map.main.governmentTypes[civ.government];
        if(governmentType.BaseReforms.Length == 0) { return; }
        GovernmentReformTier[] tiers = governmentType.BaseReforms;
        while (reformNames.Count != tiers.Length)
        {
            if (reformNames.Count > tiers.Length)
            {
                int lastIndex = reformNames.Count - 1;
                Destroy(reformNames[lastIndex]);
                Destroy(reformTiers[lastIndex]);
                reformNames.RemoveAt(lastIndex);
                reformTiers.RemoveAt(lastIndex);
                reforms.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(reformNamesPrefab, reformTierTransform);                
                reformNames.Add(item);
                item = Instantiate(reformTierPrefab, reformTierTransform);
                reformTiers.Add(item);
                reforms.Add(new List<GameObject>());
            }
        }
        for(int i = 0; i < tiers.Length;i++)
        {
            GovernmentReformTier tier = tiers[i];
            List<GameObject> reformList = reforms[i];
            reformNames[i].GetComponentInChildren<TextMeshProUGUI>().text = "Tier " + (i + 1) + ": " + tier.name;
            while (reformList.Count != tier.Reforms.Length) 
            {
                if (reformList.Count > tier.Reforms.Length)
                {
                    int lastIndex = reformList.Count - 1;
                    Destroy(reformList[lastIndex]);
                    reformList.RemoveAt(lastIndex);                    
                }
                else
                {
                    int tierID = i;
                    int index = reformList.Count;
                    GameObject item = Instantiate(reformPrefab, reformTiers[i].transform);
                    item.GetComponent<Button>().onClick.AddListener(delegate { ReformClick(tierID, index); });
                    reformList.Add(item);
                }
            }
            for(int j = 0; j < tier.Reforms.Length; j++)
            {
                GameObject item = reformList[j];
                item.GetComponentsInChildren<Image>()[1].sprite = tier.Reforms[j].icon;
                string text = tier.Reforms[j].GetHoverText(civ);
                text += civ.reforms.Count == i ? "\nIt will cost " + (40 + 40 * i) + " reform progress for this" : civ.reforms.Count > i ? "\nIt will cost 50 reform progress to switch to this" : "\nNeed to unlock reform from previous tier first";
                item.GetComponent<HoverText>().text = text;
                item.GetComponent<Button>().enabled = civ.reforms.Count >= i;            
                if(civ.reforms.Count > i)
                {
                    item.GetComponentsInChildren<Image>()[0].color = civ.reforms[i] == j ? Color.yellow : Color.gray;
                }
                else if(civ.reforms.Count == i)
                {
                    item.GetComponentsInChildren<Image>()[0].color = civ.reformProgress>= (40 + 40 * i) ? Color.green : Color.red;
                }
                else
                {
                    item.GetComponentsInChildren<Image>()[0].color = Color.black;
                }
            }
        }
    }
}
