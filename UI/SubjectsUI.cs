using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MultiplayerManager;
using static UnityEngine.GraphicsBuffer;

public class SubjectsUI : MonoBehaviour
{
    [SerializeField] GameObject subjectPrefab,subjectTypePrefab;
    [SerializeField] Transform  subjectTransform,subjectTypeTransform;
    [SerializeField] Button[] subjectInteractions;
    [SerializeField] GameObject subjectInfo;
    [SerializeField] GameObject subjectPanel, interactionPanel, subjectTypePanel,integrationPanel;
    [SerializeField] TextMeshProUGUI sName, sType, sDev, sPop, sIncome, sMP, sControl, sTech, sIdeas, sArmies, sNavies;
    [SerializeField] TextMeshProUGUI subjectInteractionDescription,subjectTypeEffects,integrationInfo;
    [SerializeField] Image subjectInteractionIcon;
    [SerializeField] Sprite coins, population;
    public List<GameObject> subjects = new List<GameObject>();
    public List<GameObject> subjectTypes = new List<GameObject>();
    Civilisation selected;
    bool grant;
    [SerializeField] Slider slider;
    int mode;
    List<int> possiblePromotions = new List<int>();
    private void Start()
    {
        slider.onValueChanged.AddListener(SliderValueChanged);
        interactionPanel.SetActive(false);
        subjectTypePanel.SetActive(false);
        integrationPanel.SetActive(false);
        subjectInteractions[0].onClick.AddListener(delegate { OpenInteraction(0); });
        subjectInteractions[1].onClick.AddListener(delegate { OpenInteraction(1); });
        subjectInteractions[3].onClick.AddListener(IntegrationPanel);
        subjectInteractions[4].onClick.AddListener(OpenSubjectTypeMenu);
        subjectInteractions[5].onClick.AddListener(SendOffer);
        subjectInteractions[6].onClick.AddListener(ToggleIntegration);
    }
    private void OnDisable()
    {
        interactionPanel.SetActive(false);
        subjectTypePanel.SetActive(false);
        integrationPanel.SetActive(false);
    }
    void OpenSubjectTypeMenu()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        SubjectType type = selected.subjectType > -1 ? Map.main.subjectTypes[selected.subjectType] : Map.main.subjectTypes[0];
        possiblePromotions = type.possiblePromotions.ToList();
        if (civ.isMarshLeader && selected.subjectType != 4)
        {
            possiblePromotions.Add(4);
        }
        subjectTypeEffects.text = type.GetDescription(civ);
        while (subjectTypes.Count != possiblePromotions.Count)
        {
            if (subjectTypes.Count > possiblePromotions.Count)
            {
                int lastIndex = subjectTypes.Count - 1;
                Destroy(subjectTypes[lastIndex]);
                subjectTypes.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(subjectTypePrefab, subjectTypeTransform);
                Button button = item.GetComponent<Button>();
                int id = subjectTypes.Count;
                button.onClick.AddListener(delegate { PromotionClick(id); });
                subjectTypes.Add(item);
            }
        }
        for (int i = 0; i < possiblePromotions.Count; i++)
        {
            SubjectType promotion = Map.main.subjectTypes[possiblePromotions[i]];
            subjectTypes[i].GetComponentInChildren<TextMeshProUGUI>().text = "Promote to " + promotion.SubjectTypeName;
            subjectTypes[i].GetComponent<HoverText>().text = promotion.GetDescription(civ);
        }
        interactionPanel.SetActive(false);
        integrationPanel.SetActive(false);
        subjectTypePanel.SetActive(true);
    }
    void PromotionClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        SubjectType type = selected.subjectType > -1 ? Map.main.subjectTypes[selected.subjectType] : Map.main.subjectTypes[0];

        if (type.SubjectEffects.Length > 0)
        {
            foreach (var effect in type.SubjectEffects)
            {
                selected.RemoveCivModifier(effect.name, type.SubjectTypeName);
            }
        }
        if (type.OverlordEffects.Length > 0)
        {
            foreach (var effect in type.OverlordEffects)
            {
                civ.RemoveCivModifier(effect.name, selected.civName);
            }
        }

        selected.subjectType = possiblePromotions[id];
        type = selected.subjectType > -1 ? Map.main.subjectTypes[selected.subjectType] : Map.main.subjectTypes[0];
        if (type.SubjectEffects.Length > 0)
        {
            foreach (var effect in type.SubjectEffects)
            {
                selected.ApplyCivModifier(effect.name, effect.amount, type.SubjectTypeName, effect.type);
            }
        }
        if (type.OverlordEffects.Length > 0)
        {
            foreach (var effect in type.OverlordEffects)
            {
                civ.ApplyCivModifier(effect.name, effect.amount, selected.civName, effect.type);
            }
        }
        selected.SetLibertyDesire();
        subjectTypePanel.SetActive(false);
    }
    void OpenInteraction(int id)
    {
        slider.value = 0;
        mode = id;
        interactionPanel.SetActive(true);
        subjectTypePanel.SetActive(false);
        integrationPanel.SetActive(false);
        SetupInteraction();
    }
    float GetCoinValue()
    {
        if (slider.value > 0)
        {
            float DailyIncome = selected.GetTotalIncome();
            float days = slider.value * 60f;
            return DailyIncome * days;
        }
        else
        {
            float DailyIncome = selected.GetTotalIncome();
            float days = slider.value * -60f;
            return DailyIncome * days;
        }
    }
    int GetPopulationValue()
    {
        if (Player.myPlayer.myCivID == -1) { return 0; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (slider.value > 0)
        {
            int dailyPop = selected.GetTotalPopulationGrowth();
            int days = (int)(slider.value * 60f);
            return Mathf.Min(dailyPop * days,civ.avaliablePopulation, selected.GetTotalMaxPopulation() - selected.GetTotalTilePopulation());
        }
        else
        {
            int dailyPop = selected.GetTotalPopulationGrowth();
            int days = (int)(slider.value * -60f);
            return Mathf.Min(dailyPop * days, selected.avaliablePopulation , civ.GetTotalMaxPopulation() - civ.GetTotalTilePopulation());
        }
    }
    void SliderValueChanged(float newValue)
    {
        if(mode == 0)
        {
            float Coins = GetCoinValue();
            string text = "";
            if(newValue > 0)
            {
                text = "Give " + Mathf.Round(Coins * 100f)/100f + "<sprite index=0> to " + selected.civName + "\n";
                text += "This will decrease liberty desire by " + Mathf.Round(newValue * 30f) + "%";
            }
            else
            {
                text = "Take " + Mathf.Round(Coins * 100f) / 100f + "<sprite index=0> from " + selected.civName + "\n";
                text += "This will increase liberty desire by " + Mathf.Round(-newValue * 60f) + "%";
            }
            subjectInteractionDescription.text = text;
        }
        else if (mode == 1)
        {
            int Pop = GetPopulationValue();
            string text = "";
            if (newValue > 0)
            {
                text = "Give " + Pop + "<sprite index=4> to " + selected.civName + "\n";
                text += "This will decrease liberty desire by " + Mathf.Round(newValue * 30f) + "%";
            }
            else
            {
                text = "Take " + Pop + "<sprite index=4> from " + selected.civName + "\n";
                text += "This will increase liberty desire by " + Mathf.Round(-newValue * 60f) + "%";
            }
            subjectInteractionDescription.text = text;
        }
    }

    void SetupInteraction()
    {
        subjectInteractionDescription.text = "";
        subjectInteractionIcon.sprite = mode == 0 ? coins : population;
        slider.handleRect.GetComponent<Image>().sprite = mode == 0 ? coins : population;
    }
    void SendOffer()
    {
        if(slider.value == 0) { return; }
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (mode == 0)
        {
            float Coins = GetCoinValue();
            if (slider.value > 0)
            {
                if (civ.coins < Coins) { return; }
                selected.coins += Coins;
                civ.coins -= Coins;
            }
            else
            {               
                if(selected.libertyDesire > 50f) { return; }
                selected.coins -= Coins;
                civ.coins += Coins;
            }
        }
        else if (mode == 1)
        {
            int Pop = GetPopulationValue();          
            if (slider.value > 0)
            {
                if (civ.avaliablePopulation < Pop || (selected.GetTotalMaxPopulation() - selected.GetTotalTilePopulation()) < Pop) { return; }
                selected.AddPopulation(Pop);
                civ.RemovePopulation(Pop);
            }
            else
            {
                if (selected.libertyDesire > 50f || selected.avaliablePopulation < Pop || (civ.GetTotalMaxPopulation() - civ.GetTotalTilePopulation()) < Pop) { return; }
                civ.AddPopulation(Pop);
                selected.RemovePopulation(Pop);
            }
        }

        float value = slider.value * 30f * (slider.value > 0 ? 1f : -2f);
        selected.libertyDesireTemp.IncreaseModifier(mode == 0 ? "Economic Actions" : "Population Actions", value, EffectType.Flat, Decay: true);
        selected.SetLibertyDesire();

        interactionPanel.SetActive(false);
    }
    void IntegrationPanel()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        int totalCost = selected.GetIntegrationCost(civ);
        string text = "it will cost " + totalCost+ "<sprite index=2> to integrate " + selected.civName + "\n\n";
        string bText = "Begin Integration";
        TextMeshProUGUI buttonText = subjectInteractions[6].GetComponentInChildren<TextMeshProUGUI>();
        if (selected.libertyDesire > 50f)
        {
            text += "This cannot happen while liberty desire is above 50%\n\n";
            bText = "Cannot Integrate";
        }
        else
        {
            int daily = (2 + (int)civ.diploRep.v);
            int days = Mathf.CeilToInt((float)totalCost/ (float)daily);
            text += "This happens at a rate of "+ daily + "<sprite index=2> per day\n\n";
            text += "Estimated Duration " + days + " days\n\n";
        }
        if (selected.integrating)
        {
            int percent = (int)Mathf.Round((float)selected.annexationProgress * 100f / (float)totalCost);
            text += "Current Progress " + selected.annexationProgress + "/" + totalCost + "<sprite index=2> ("+ percent + "%)\n\n";
            bText = "Cancel Integration";
        }
        integrationInfo.text = text;
        buttonText.text = bText;
        integrationPanel.SetActive(true);
    }
    void ToggleIntegration()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        if (selected.integrating)
        {
            selected.integrating = false;
        }
        else
        {
            if(selected.libertyDesire > 50f)
            {
                return;
            }
            else
            {
                selected.integrating = true;
            }
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (integrationPanel.activeSelf)
        {
            IntegrationPanel();
        }
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        while (subjects.Count != civSubjects.Count)
        {
            if (subjects.Count > civSubjects.Count)
            {
                int lastIndex = subjects.Count - 1;
                Destroy(subjects[lastIndex]);
                subjects.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(subjectPrefab, subjectTransform);
                Button coreButton = item.GetComponent<Button>();
                int id = subjects.Count;
                coreButton.onClick.AddListener(delegate { SubjectClick(id); });
                subjects.Add(item);
            }
        }
        for (int i = 0; i < civSubjects.Count; i++)
        {
            Civilisation subject = civSubjects[i];
            Image[] images = subjects[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = subjects[i].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = civ.c;
            texts[0].text = subject.civName;
            texts[1].text = Mathf.Round(subject.opinionOfThem[civ.CivID].v) + "";
            texts[2].text = Mathf.Round(subject.libertyDesire ) + "%";
            float subjectIncome = subject.libertyDesire < 50f ? subject.GetTotalIncome() * (0.1f + civ.incomeFromSubjects.v) : 0f;
            texts[3].text = Mathf.Round(subjectIncome * 100f) /100f + "<sprite index=0>";
            texts[2].GetComponent<HoverText>().text = LibertyDesireText(subject);
        }       
        if (civSubjects.Contains(selected))
        {
            SubjectType type = selected.subjectType > -1 ? Map.main.subjectTypes[selected.subjectType] : Map.main.subjectTypes[0];
            sName.text = selected.civName;
            sType.text = type.SubjectTypeName;
            sDev.text = "Total Dev: " +selected.GetTotalDev() + "";
            sPop.text = selected.avaliablePopulation + "<sprite index=4>";
            sIncome.text = Mathf.Round(selected.GetTotalIncome() * 100f) / 100f + "<sprite index=0>";
            sMP.text = selected.adminPower + "<sprite index=1>  " + selected.diploPower + "<sprite index=2>  " + selected.milPower + "<sprite index=3>";
            sArmies.text = "Armies: " + (int)(selected.TotalMaxArmySize() / 1000f) + "k <sprite index=3>";
            sNavies.text = "Boats: " + (int)(selected.fleets.Sum(i => i.boats.Count)) + "<sprite index=2>";
            sTech.text = selected.adminTech + "<sprite index=1>  " + selected.diploTech + "<sprite index=2>  " + selected.milTech + "<sprite index=3>";
            sIdeas.text = "Ideas: " + selected.totalIdeas + "";
            sControl.text = "Average Control: " + Mathf.Round(selected.GetAverageControl()) + "%";
            subjectPanel.SetActive(true);
        }
        else
        {
            subjectPanel.SetActive(false); 
        }
        
    }
    void SubjectClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        selected = civSubjects[id];
    }
   
    string LibertyDesireText(Civilisation subject)
    {
        if (subject.overlordID == -1) { return "Not Valid Subject"; }
        Civilisation overlord = Game.main.civs[subject.overlordID];
        SubjectType type = subject.subjectType > -1 ? Map.main.subjectTypes[subject.subjectType] : Map.main.subjectTypes[0];
        string ld = "Liberty Desire: " + Mathf.Round(subject.libertyDesire) + "%\n";
        if (type.CountsOwnArmies)
        {
            ld += "From Relative Military Strength: " + Mathf.Round(MathF.Min(100f, (subject.TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f)) + "%\n";
        }
        foreach (var ally in subject.allies)
        {
            ld += "From Relative Military Strength of "+ Game.main.civs[ally].civName + ": " + Mathf.Round(MathF.Min(100f, (Game.main.civs[ally].TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f)) + "%\n";
        }
        if (type.CountsOtherSubjectArmies)
        {
            foreach (var ally in overlord.subjects)
            {
                if (ally == subject.CivID) { continue; }
                Civilisation subCiv = Game.main.civs[ally];
                SubjectType subCivType = subCiv.subjectType > -1 ? Map.main.subjectTypes[subCiv.subjectType] : Map.main.subjectTypes[0];
                if (subCivType.CountsOtherSubjectArmies)
                {
                    ld += "From Relative Military Strength of " + Game.main.civs[ally].civName + ": " + Mathf.Round(MathF.Min(100f, (Game.main.civs[ally].TotalMilStrength() + 0.1f) / (overlord.TotalMilStrength() + 0.1f) * 75f)) + "%\n";
                }
            }
        }
        if (type.CountsOwnEconomy)
        {
            ld += "From Relative Economic Strength: " + Mathf.Round(MathF.Min(100f, (subject.GetTotalIncome() + 0.1f) / (overlord.GetTotalIncome() + 0.1f) * 75f)) + "%\n";
        }
        if (type.CountsOtherSubjectsEconomy)
        {
            foreach (var ally in overlord.subjects)
            {
                if (ally == subject.CivID) { continue; }
                Civilisation subCiv = Game.main.civs[ally];
                SubjectType subCivType = subCiv.subjectType > -1 ? Map.main.subjectTypes[subCiv.subjectType] : Map.main.subjectTypes[0];
                if (subCivType.CountsOtherSubjectsEconomy)
                {
                    ld += "From Relative Economic Strength of " + Game.main.civs[ally].civName + ": " + Mathf.Round(MathF.Min(100f, (subject.GetTotalIncome() + 0.1f) / (overlord.GetTotalIncome() + 0.1f) * 75f)) + "%\n";
                }
            }
        }


        ld += "From Overlord Diplomatic Reputation: " + Mathf.Round(-3f * overlord.diploRep.v) + "%\n";
        ld += "From Opinion of Overlord: " +Mathf.Round(-0.1f * subject.opinionOfThem[subject.overlordID].v) + "%\n";
        ld += (subject.diploTech - overlord.diploTech > 0)? "From Better Diplomatic Technology Than Overlord: " + Mathf.Max(0, subject.diploTech - overlord.diploTech) * 5f + "%\n" : "";
        ld += "From Total Development: " + Mathf.Round(subject.GetTotalDev() * 0.25f * (1f + overlord.libDesireFromDevForSubjects.v) * type.LibertyDesireFromDevelopment) + "%\n";
        ld += "Overlord Bonuses: " +overlord.libDesireInSubjects.ToString() + "\n\n";
        if (type.LibertyDesireFlat != 0)
        {
            ld += "Subject Type: " + Mathf.Round((type.LibertyDesireFlat) * 100f) + "%\n";
        }
        ld += "Temporary Bonuses: " + Mathf.Round(subject.libertyDesireTemp.v*100f)/100f;
        return ld;     
    }

}
