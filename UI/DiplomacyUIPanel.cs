using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyUIPanel : MonoBehaviour
{
    [SerializeField] Button[] diplomaticActions;
    [SerializeField] Button[] rivals;
    [SerializeField] Button closeRivalsList,toggleSubjectList;
    [SerializeField] Image civIcon, civReligion;
    [SerializeField] TextMeshProUGUI civName, civGovRankName, civRulerName,opinion,diprep,diprel;
    [SerializeField] TextMeshProUGUI adminTech, diploTech, milTech,ideas;
    [SerializeField] GameObject diploMiniPrefab,diploBack,rivalBack,rivalPrefab,warDecBack,releaseSubjectBack;
    [SerializeField] Transform warTransform,truceTransform,alianceTransform,rivalTransform,subjectTransform;
    List<GameObject> wars,truces,allies,rivalList,subjects;
    public static DiplomacyUIPanel main;
    public int diploCivID;
    int rivalSlot = 0;

    private void OnDisable()
    {
        CancelWarDec();
        CloseRivalsList();
    }
    private void Awake()
    {
        main = this;
        diplomaticActions[0].onClick.AddListener(DeclareWar);
        diplomaticActions[1].onClick.AddListener(SendAlliance);
        diplomaticActions[2].onClick.AddListener(SendMilAccess);
        closeRivalsList.onClick.AddListener(CloseRivalsList);
        toggleSubjectList.onClick.AddListener(ToggleSubjectList);
        for(int i = 0; i < 3;i++)
        {
            int id = i;
            rivals[i].onClick.AddListener(delegate { SelectRival(id); });
        }
        wars = new List<GameObject>();
        truces = new List<GameObject>();
        allies = new List<GameObject>();
        rivalList = new List<GameObject>();
        subjects = new List<GameObject>();
        releaseSubjectBack.SetActive(false);
    }
    void CloseRivalsList()
    {
        diploBack.SetActive(true);
        rivalBack.SetActive(false);
    }
    void ToggleSubjectList()
    {
        if (diploCivID == -1) { diploCivID = Player.myPlayer.myCivID; return; }
        Civilisation civ = Game.main.civs[diploCivID];
        if (diploCivID != Player.myPlayer.myCivID)
        {
            diploCivID = Player.myPlayer.myCivID;
            return;
        }    
        bool setActive = !releaseSubjectBack.activeSelf;
        rivalBack.SetActive(false);
        releaseSubjectBack.SetActive(setActive);
        if (setActive)
        {
            releaseSubjectBack.GetComponent<ReleaseSubjectUI>().SetupList();
        }
    }
    void SelectRival(int id)
    {
        if(diploCivID == -1) { return; }
        Civilisation civ = Game.main.civs[diploCivID];
        if (civ.rivals[id] > -1)
        {
            diploCivID = civ.rivals[id];
        }
        else if (Player.myPlayer.myCivID > -1)
        {
            diploBack.SetActive(false);
            rivalBack.SetActive(true);
            rivalSlot = id;
            rivalSlot = id;
            SetupRivalList();
        }
    }
    void SetupRivalList()
    {
        Civilisation myCiv = Player.myPlayer.myCiv;
        float myStrength = 0f;
        foreach(var army in myCiv.armies)
        {
            myStrength += army.ArmyStrength();
        }
        List<int> possible = myCiv.GetPossibleRivals();        
        while (rivalList.Count != possible.Count)
        {
            if (rivalList.Count > possible.Count)
            {
                int lastIndex = rivalList.Count - 1;
                Destroy(rivalList[lastIndex]);
                rivalList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(rivalPrefab, rivalTransform);
                int index = rivalList.Count;
                item.GetComponent<Button>().onClick.AddListener(delegate { SelectRivalFromList(index); });
                rivalList.Add(item);
            }
        }
        for (int i = 0; i < possible.Count; i++)
        {
            Civilisation rival = Game.main.civs[possible[i]];
            TextMeshProUGUI[] texts = rivalList[i].GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = rivalList[i].GetComponentsInChildren<Image>();
            texts[0].text = rival.civName;
            images[1].color = rival.c;
            float strength = 0f;
            foreach (var army in rival.armies)
            {
                strength += army.ArmyStrength();
            }
            float percent = 9999f;
            if(myStrength > 0 && strength > 0)
            {
                percent = strength/ myStrength;
            }
            texts[2].text = "" + Mathf.Round(percent*100f) + "%";
        }
    }
    void SelectRivalFromList(int id)
    {
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<int> possible = myCiv.GetPossibleRivals();
        myCiv.rivals[rivalSlot] = possible[id];
        myCiv.opinionOfThem[myCiv.rivals[rivalSlot]].AddModifier(new Modifier(-100, ModifierType.Flat, "Rival"));
        Game.main.civs[myCiv.rivals[rivalSlot]].opinionOfThem[myCiv.CivID].AddModifier(new Modifier(-50, ModifierType.Flat, "Rivals Us"));
        CloseRivalsList();
    }
    private void OnGUI()
    {
        if (diploCivID == -1) { return; }
        Civilisation civ = Game.main.civs[diploCivID];
        if(civ.rivals != null)
        {
            for(int i = 0; i < civ.rivals.Length; i++)
            {
                if (civ.rivals[i] > -1)
                {
                    Civilisation rival = Game.main.civs[civ.rivals[i]];
                    rivals[i].GetComponent<Image>().color = rival.c;
                }
                else
                {
                    rivals[i].GetComponent<Image>().color = Color.white;
                }
            }
        }
        if(diploCivID != Player.myPlayer.myCivID && Player.myPlayer.myCivID > -1)
        {
            Civilisation myCiv = Player.myPlayer.myCiv;
            string hoverText = "";
            hoverText = "<#00ff00>" + GetPositiveReasons(civ, myCiv);
            hoverText += "\n<#ff0000>" + GetNegativeReasons(civ, myCiv);
            float reasons = Mathf.Round(AllianceOffer(civ, myCiv));
            bool wouldAccept = reasons > 0;
            hoverText += "\n" + (wouldAccept ? "<#00ff00>" : "<#ff0000>") + "Total Reasons: " + reasons;
            diplomaticActions[1].GetComponentsInChildren<Image>()[1].color = wouldAccept ? Color.green : Color.red;
            diplomaticActions[1].GetComponentsInChildren<Image>()[1].sprite = wouldAccept ? UIManager.main.icons[0] : UIManager.main.icons[1];
            diplomaticActions[1].GetComponentInChildren<HoverText>().text = hoverText;
            bool milAccessAccept = civ.AccessOffer(myCiv);
            diplomaticActions[2].GetComponentsInChildren<Image>()[1].color = milAccessAccept ? Color.green : Color.red;
            diplomaticActions[2].GetComponentsInChildren<Image>()[1].sprite = milAccessAccept ? UIManager.main.icons[0] : UIManager.main.icons[1];
        }
        else
        {
            diplomaticActions[1].GetComponentsInChildren<Image>()[1].color = Color.red;
            diplomaticActions[1].GetComponentsInChildren<Image>()[1].sprite = UIManager.main.icons[1];
        }
        civIcon.color = civ.c;
        civName.text = civ.civName + (civ.overlordID > -1 ? " (" + Game.main.civs[civ.overlordID].civName+")" : "");
        civGovRankName.text = Map.main.governmentTypes[civ.government].name;
        ideas.text = "Ideas: ("+civ.totalIdeas+") <sprite index=7>";
        string ideasText = "";
        for (int i = 0; i < civ.ideaGroups.Length; i++)
        {
            IdeaGroupData idea = civ.ideaGroups[i];
            if (idea != null)
            {
                if (idea.active)
                {
                    IdeaGroup group = idea.type == 0 ? Map.main.IdeasA[idea.id] : idea.type == 1 ? Map.main.IdeasD[idea.id] : Map.main.IdeasM[idea.id];
                    ideasText += group.name + "(" + idea.unlockedLevel + ")\n";
                }
            }
        }
        ideas.GetComponent<HoverText>().text = ideasText;
        diprep.text = "Diplo Rep: " + Mathf.Round(civ.diploRep.v * 10f)/10f + " <sprite index=2>";
        diprep.GetComponent<HoverText>().text = civ.diploRep.ToString();
        diprel.text = "Diplomatic Capacity: "+ Mathf.Round(civ.diplomaticCapacity) +"/"+ Mathf.Round(civ.diplomaticCapacityMax.v) + " <sprite index=2>";
        diprel.GetComponent<HoverText>().text = civ.diplomaticCapacityMax.ToString();
        civReligion.sprite = Map.main.religions[civ.religion].sprite;
        civReligion.GetComponent<HoverText>().text = Map.main.religions[civ.religion].GetHoverText(civ);
        if (Player.myPlayer.myCivID > -1 && Game.main.Started)
        {
            opinion.text = Mathf.Round(civ.opinionOfThem[Player.myPlayer.myCivID].v) + "";
            opinion.GetComponent<HoverText>().text = civ.opinionOfThem[Player.myPlayer.myCivID].ToString();
        }
        adminTech.text = civ.adminTech + "<sprite index=1><sprite index=8>";
        diploTech.text = civ.diploTech + "<sprite index=2><sprite index=8>";
        milTech.text = civ.milTech + "<sprite index=3><sprite index=8>";
        if (civ.ruler.active)
        {
            civRulerName.text = civ.ruler.Name + " ("+civ.ruler.age.y +") " + civ.ruler.adminSkill + "<sprite index=1> " + civ.ruler.diploSkill + "<sprite index=2> " + civ.ruler.milSkill +  "<sprite index=3>";
        }
        else
        {
            civRulerName.text = "No Ruler";
        }
        if (Player.myPlayer.myCivID > -1)
        {
            diplomaticActions[0].GetComponentInChildren<TextMeshProUGUI>().text = Player.myPlayer.myCiv.atWarWith.Contains(diploCivID)? "Sue for Peace" : "Declare War";
            diplomaticActions[1].GetComponentInChildren<TextMeshProUGUI>().text = Player.myPlayer.myCiv.allies.Contains(diploCivID) ? "Break Alliance" : "Offer Alliance";
            diplomaticActions[2].GetComponentInChildren<TextMeshProUGUI>().text = Player.myPlayer.myCiv.militaryAccess.Contains(diploCivID) ? "Cancel Access" : "Request Access";
        }
        List<War> civWars = civ.GetWars();
        while (wars.Count != civWars.Count)
        {
            if (wars.Count > civWars.Count)
            {
                int lastIndex = wars.Count - 1;
                Destroy(wars[lastIndex]);
                wars.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(diploMiniPrefab, warTransform);
                wars.Add(item);
            }
        }
        for (int i = 0; i < civWars.Count; i++)
        {
            War civWar = civWars[i];
            wars[i].GetComponentInChildren<Image>().color = civWar.GetOpposingLeader(civ.CivID).c;
            wars[i].GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Round(civWar.warScore) * ((civWar.attackerCiv == civ) ? 1f : -1f) + "%";
        }
        int numTruces = civ.truces.ToList().FindAll(i => i > 0).Count;
        while (truces.Count != numTruces)
        {
            if (truces.Count > numTruces)
            {
                int lastIndex = truces.Count - 1;
                Destroy(truces[lastIndex]);
                truces.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(diploMiniPrefab, truceTransform);
                truces.Add(item);
            }
        }
        int index = 0;
        for (int i = 0; i < civ.truces.Length; i++)
        {
            if (civ.truces[i] > 0)
            {
                truces[index].GetComponentInChildren<Image>().color = Game.main.civs[i].c;
                truces[index].GetComponentInChildren<TextMeshProUGUI>().text = civ.truces[i] + "";
                index++;
            }
        }       
        while (allies.Count != civ.allies.Count)
        {
            if (allies.Count > civ.allies.Count)
            {
                int lastIndex = allies.Count - 1;
                Destroy(allies[lastIndex]);
                allies.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(diploMiniPrefab, alianceTransform);
                allies.Add(item);
            }
        }
        for (int i = 0; i < civ.allies.Count; i++)
        {
            Civilisation ally = Game.main.civs[civ.allies[i]];
            allies[i].GetComponentInChildren<Image>().color = ally.c;
            TextMeshProUGUI[] texts = allies[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = Mathf.Round(ally.opinionOfThem[civ.CivID].v) + "";        
        }
        while (subjects.Count != civ.subjects.Count)
        {
            if (subjects.Count > civ.subjects.Count)
            {
                int lastIndex = subjects.Count - 1;
                Destroy(subjects[lastIndex]);
                subjects.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(diploMiniPrefab, subjectTransform);
                subjects.Add(item);
            }
        }
        for (int i = 0; i < civ.subjects.Count; i++)
        {
            Civilisation subject = Game.main.civs[civ.subjects[i]];
            subjects[i].GetComponentInChildren<Image>().color = subject.c;
            TextMeshProUGUI[] texts = subjects[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = Mathf.Round(subject.libertyDesire) + "";
        }
    }
    void SendAlliance()
    {
        if (diploCivID == -1 || Player.myPlayer.myCivID == -1) { return; }
        if (diploCivID != Player.myPlayer.myCivID)
        {
            if (!Player.myPlayer.myCiv.allies.Contains(diploCivID))
            {
                Player.myPlayer.myCiv.OfferAlliance(diploCivID);
            }
            else
            {
                Player.myPlayer.myCiv.BreakAlliance(diploCivID);
            }
        }
    }
    void SendMilAccess()
    {
        if (diploCivID == -1 || Player.myPlayer.myCivID == -1) { return; }
        if (diploCivID != Player.myPlayer.myCivID)
        {
            if (!Player.myPlayer.myCiv.militaryAccess.Contains(diploCivID))
            {
                Player.myPlayer.myCiv.AccessRequest(diploCivID);
            }
            else
            {
                Player.myPlayer.myCiv.RemoveAccess(diploCivID);
            }
        }
    }
    void DeclareWar()
    {
        if (diploCivID == -1 || Player.myPlayer.myCivID == -1) { return; }
        if(diploCivID != Player.myPlayer.myCivID)
        {
            if (!Player.myPlayer.myCiv.atWarWith.Contains(diploCivID))
            {
                warDecBack.SetActive(true);
                diploBack.SetActive(false); 
                warDecBack.GetComponent<DeclareWarPanelUI>().Open();
                warDecBack.GetComponent<DeclareWarPanelUI>().SetupWar();
                //Player.myPlayer.myCiv.DeclareWar(diploCivID);
            }
            else
            {
                UIManager.main.PeaceDealUI.SetActive(true);
                UIManager.main.CivUI.SetActive(false);
                Player.myPlayer.mapMode = -1;
            }
        }
    }
    public void CancelWarDec()
    {
        warDecBack.SetActive(false);
        diploBack.SetActive(true);
    }

    public static string GetPositiveReasons(Civilisation target, Civilisation fromCiv)
    {
        string reasons = 0.25f * target.opinionOfThem[fromCiv.CivID].v > 0? "Opinion: " + 0.25f * target.opinionOfThem[fromCiv.CivID].v +"\n": "";
        reasons += fromCiv.diploRep.v > 0 ? "Diplo Rep: " + fromCiv.diploRep.v * 5f  + "\n": "";
        reasons += Mathf.Clamp(20f * ((1f + fromCiv.TotalMilStrength()) / (1f + target.TotalMilStrength()) - 1f), -20f, 20f) > 0f ? "Relative Military Strength: " + Mathf.Clamp(50f * ((1f + fromCiv.TotalMilStrength()) / (1f + target.TotalMilStrength()) - 1f), -20f, 20f) : "";       
        return reasons;
    }
    public static string GetNegativeReasons(Civilisation target, Civilisation fromCiv)
    {
        string reasons = 0.25f * target.opinionOfThem[fromCiv.CivID].v < 0 ? "Opinion: " + -0.25f * target.opinionOfThem[fromCiv.CivID].v + "\n" : "";
        reasons += fromCiv.diploRep.v < 0 ? "Diplo Rep: " + fromCiv.diploRep.v * -5f + "\n": "";
        reasons += Mathf.Clamp(20f * ((1f + fromCiv.TotalMilStrength()) / (1f + target.TotalMilStrength()) - 1f), -20f, 20f) < 0f ? "Relative Military Strength: " + Mathf.Clamp(-50f * ((1f + fromCiv.TotalMilStrength()) / (1f + target.TotalMilStrength()) - 1f), -20f, 20f) + "\n" : "";
        reasons += Mathf.Max(0, target.MinimumDistTo(fromCiv) - 10) > 0 ? "Distance Between Borders: " + Mathf.Max(0, target.MinimumDistTo(fromCiv) - 10) + "\n" : "";
        reasons += fromCiv.atWarWith.Count > 0 ? "You are at War: 1000\n" : "";
        reasons += (fromCiv.CivID == target.overlordID) ? "They are a your subject: 1000\n" : "";
        reasons += (fromCiv.overlordID == target.CivID) ? "They are a your overlord: 1000\n" : "";
        reasons += (target.overlordID > -1 && target.libertyDesire < 50f) ? "They are a Loyal Subject: 1000\n" : "";
        reasons += (fromCiv.overlordID > -1 && fromCiv.libertyDesire < 50f) ? "You are a Loyal Subject: 1000\n" : "";
        reasons += (target.diplomaticCapacity + 25 + fromCiv.governingCapacity * 0.5f) > target.diplomaticCapacityMax.v ? "Target Would Be Over Diplomatic Capacity: " + (target.diplomaticCapacity + 25 +fromCiv.governingCapacity * 0.5f - target.diplomaticCapacityMax.v) / target.diplomaticCapacityMax.v * -100f + "\n" : "";
        return reasons;
    }
    public float AllianceOffer(Civilisation target, Civilisation fromCiv)
    {
        float choice = 0.25f * target.opinionOfThem[fromCiv.CivID].v;
        choice += 5f * fromCiv.diploRep.v;
        choice += Mathf.Clamp(20f * ((1f + fromCiv.TotalMilStrength()) / (1f + target.TotalMilStrength()) - 1f), -20f, 20f);
        choice -= Mathf.Max(0, target.MinimumDistTo(fromCiv) - 10);
        choice += fromCiv.atWarWith.Count > 0 ? -1000 : 0;
        choice += (target.overlordID == fromCiv.CivID) ? -1000 : 0;
        choice += (fromCiv.overlordID == target.CivID) ? -1000 : 0;
        choice += (target.overlordID > -1 && target.libertyDesire < 50f) ? -1000 : 0;
        choice += (fromCiv.overlordID > -1 && fromCiv.libertyDesire < 50f) ? -1000 : 0;
        choice += ((target.diplomaticCapacity +25f+ fromCiv.governingCapacity * 0.5f) > target.diplomaticCapacityMax.v ? (target.diplomaticCapacity +25f+ fromCiv.governingCapacity * 0.5f - target.diplomaticCapacityMax.v) / target.diplomaticCapacityMax.v * -100f : 0f);
        return choice;
    }
}
