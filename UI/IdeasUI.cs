using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdeasUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI trad1, trad2;
    [SerializeField] TextMeshProUGUI trad1V,trad2V;
    [SerializeField] Image[] nationalIdeas,ideaGroups;
    [SerializeField] Image nationalIdeasFill;
    [SerializeField] Button closeIdeaShop;
    [SerializeField] GameObject ideaShop,ideaBack,ideasPrefab;
    [SerializeField] Transform adminT, diploT, milT;
    [SerializeField] List<GameObject> adminTList = new List<GameObject>(), diploTList = new List<GameObject>(), milTList = new List<GameObject>();
    int SelectedIndex;

    private void OnDisable()
    {
        CloseIdeaShop();
    }
    private void Start()
    {
        for (int i = 0; i < ideaGroups.Length; i++)
        {
            int index = i;
            ideaGroups[i].GetComponent<Button>().onClick.AddListener(delegate { OpenIdeaShop(index); });
            Button[] buttons =  ideaGroups[i].GetComponentsInChildren<Button>();
            for (int j = 1; j < buttons.Length; j++)
            {
                if (j == buttons.Length - 1) 
                {
                    buttons[j].onClick.AddListener(delegate { AbandonIdeaGroup(index); });
                }
                else
                {
                    int level = j - 1;
                    buttons[j].onClick.AddListener(delegate { BuyIdeaLocal(index, level); });
                }
            }
        }
        closeIdeaShop.onClick.AddListener(CloseIdeaShop);
    }
    public static int GetIdeaCost(Civilisation civ)
    {
        int baseCost = 400;
        int cost = baseCost;        
        cost += (int)(baseCost * (civ.ideaCosts.value));        
        return Mathf.Max(1, cost);
    }
    void BuyIdeaLocal(int index, int level)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        BuyIdea(index, level, civ);
    }
    public static void BuyIdea(int index,int level,Civilisation civ)
    {       
        IdeaGroupData ideaGroup = civ.ideaGroups[index];
        int points = ideaGroup.type == 0 ? civ.adminPower : ideaGroup.type == 1 ? civ.diploPower : civ.milPower;
        if (points < GetIdeaCost(civ))
        {
            return;
        }
        if (ideaGroup.type == 0) { civ.adminPower -= GetIdeaCost(civ); }
        else if (ideaGroup.type == 1) { civ.diploPower -= GetIdeaCost(civ); }
        else { civ.milPower -= GetIdeaCost(civ); }
        IdeaGroup group = ideaGroup.type == 0 ? Map.main.IdeasA[ideaGroup.id] : ideaGroup.type == 1 ? Map.main.IdeasD[ideaGroup.id] : Map.main.IdeasM[ideaGroup.id];
        Idea idea = group.ideas[level];
        for(int i = 0; i < idea.effects.Length; i++)
        {
            civ.ApplyCivModifier(idea.effects[i].name, idea.effects[i].amount, idea.name, idea.effects[i].type);
        }
        ideaGroup.unlockedLevel++;
        if ((civ.totalIdeas + 1) % 3 == 0)
        {
            int ideaIndex = (int)(civ.totalIdeas - 2) / 3;
            
            if (civ.nationalIdeas.ideas.Length <= ideaIndex) { return; }
            
            Idea natI = civ.nationalIdeas.ideas[ideaIndex];
            if (civ.CivID == Player.myPlayer.myCivID)
            {
                Debug.Log(natI.name + " Unlock");
            }
            for (int i = 0; i < natI.effects.Length; i++)
            {
                civ.ApplyCivModifier(natI.effects[i].name, natI.effects[i].amount, natI.name, natI.effects[i].type);
            }
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        trad1.text = civ.nationalIdeas.traditions[0].name;
        trad2.text = civ.nationalIdeas.traditions[1].name;
        trad1V.text = "<#00ff00>" + Modifier.ToString(civ.nationalIdeas.traditions[0].amount, civ.GetStat(civ.nationalIdeas.traditions[0].name), civ.nationalIdeas.traditions[1].type == 2 || civ.nationalIdeas.traditions[1].type == 0, civ.nationalIdeas.traditions[1].type == 3);
        trad2V.text = "<#00ff00>" + Modifier.ToString(civ.nationalIdeas.traditions[1].amount, civ.GetStat(civ.nationalIdeas.traditions[1].name), civ.nationalIdeas.traditions[1].type == 2 || civ.nationalIdeas.traditions[1].type == 0, civ.nationalIdeas.traditions[1].type == 3);
        int unlocked = 0;
        foreach(var civIdea in civ.ideaGroups)
        {
            if (civIdea != null && civIdea.active)
            {
                unlocked += civIdea.unlockedLevel;
            }
        }
        nationalIdeasFill.fillAmount = (float)unlocked / 21f;
        for(int i = 0; i < 7; i++)
        {
            nationalIdeas[i].GetComponentsInChildren<Image>()[1].sprite = civ.nationalIdeas.ideas[i].icon;
            nationalIdeas[i].GetComponent<HoverText>().text = civ.nationalIdeas.ideas[i].GetHoverText(civ);
        }
        for(int i = 0; i < ideaGroups.Length; i++)
        {
            List<Image> images = ideaGroups[i].GetComponentsInChildren<Image>().ToList();
            images.Remove(images[0]);
            if (civ.unlockedIdeaGroupSlots > i)
            {               
                if (civ.ideaGroups[i] != null && civ.ideaGroups[i].active)
                {
                    ideaGroups[i].GetComponent<Button>().interactable = false;
                    images.ForEach(i => i.enabled = true);
                    SetupIdeaGroup(ideaGroups[i].gameObject, civ.ideaGroups[i]);
                }
                else
                {
                    ideaGroups[i].GetComponent<Button>().interactable = true;
                    images.ForEach(i => i.enabled = false);
                    ideaGroups[i].GetComponentInChildren<TextMeshProUGUI>().text = "Click To Select Idea Group";
                }
            }
            else
            {
                ideaGroups[i].GetComponent<Button>().interactable = false;
                images.ForEach(i => i.enabled = false);
                ideaGroups[i].GetComponentInChildren<TextMeshProUGUI>().text = "Locked";
            }
        }
    }
    void SetupIdeaGroup(GameObject ideaGO, IdeaGroupData ideaGroup)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        HoverText[] ideas = ideaGO.GetComponentsInChildren<HoverText>();
        IdeaGroup group = ideaGroup.type == 0 ? Map.main.IdeasA[ideaGroup.id] : ideaGroup.type == 1 ? Map.main.IdeasD[ideaGroup.id] : Map.main.IdeasM[ideaGroup.id];
        for (int i = 0; i < ideas.Length; i++)
        {
            ideas[i].GetComponent<Button>().interactable = i == ideaGroup.unlockedLevel;
            ideas[i].gameObject.SetActive(true);
            Image image = ideas[i].GetComponentsInChildren<Image>()[1];
            Idea idea = group.ideas[i];
            ideas[i].text = idea.GetHoverText(civ);
            ideas[i].text += "Cost " + IdeasUI.GetIdeaCost(civ) + (ideaGroup.type == 0 ? "<sprite index=1>" : ideaGroup.type == 1 ?"<sprite index=2>" : "<sprite index=3>");
            image.sprite = idea.icon;
            bool canAfford = true;
            int points = ideaGroup.type == 0 ? civ.adminPower : ideaGroup.type == 1 ? civ.diploPower : civ.milPower;
            if (points < GetIdeaCost(civ))
            {
                canAfford = false;
            }
            image.color = ideaGroup.unlockedLevel < i ? Color.gray : ideaGroup.unlockedLevel == i? (canAfford ?Color.green:Color.red) : Color.white;            
        }
        ideaGO.GetComponentInChildren<TextMeshProUGUI>().text = group.name + (group.type == 0 ? "<sprite index=1>" : group.type == 1 ? "<sprite index=2>" : "<sprite index=3>");
    }
    void AbandonIdeaGroup(int index)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        IdeaGroupData ideaGroup = civ.ideaGroups[index];
        if (ideaGroup.active)
        {
            ideaGroup.active = false;
            int points = ideaGroup.unlockedLevel * 40;
            if (ideaGroup.type == 0) { civ.adminPower += points; } 
            else if (ideaGroup.type == 1) { civ.diploPower += points; } 
            else { civ.milPower += points; }
        }
    }
    void OpenIdeaShop(int selectedIndex)
    {
        SelectedIndex = selectedIndex;
        ideaShop.SetActive(true);
        ideaBack.SetActive(false);
        while (adminTList.Count != Map.main.IdeasA.Length)
        {
            if (adminTList.Count > Map.main.IdeasA.Length)
            {
                int lastIndex = adminTList.Count - 1;
                Destroy(adminTList[lastIndex]);
                UIManager.main.UI.Remove(adminTList[lastIndex]);
                adminTList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(ideasPrefab, adminT);
                adminTList.Add(item);
            }
        }
        for (int i = 0; i < adminTList.Count; i++)
        {
            IdeaGroup idea = Map.main.IdeasA[i];
            SetupIdeaGroupShop(adminTList[i], idea);
        }
        while (diploTList.Count != Map.main.IdeasD.Length)
        {
            if (diploTList.Count > Map.main.IdeasD.Length)
            {
                int lastIndex = diploTList.Count - 1;
                Destroy(diploTList[lastIndex]);
                UIManager.main.UI.Remove(diploTList[lastIndex]);
                diploTList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(ideasPrefab, diploT);
                diploTList.Add(item);
            }
        }
        for (int i = 0; i < diploTList.Count; i++)
        {
            IdeaGroup idea = Map.main.IdeasD[i];
            SetupIdeaGroupShop(diploTList[i], idea);
        }
        while (milTList.Count != Map.main.IdeasM.Length)
        {
            if (milTList.Count > Map.main.IdeasM.Length)
            {
                int lastIndex = milTList.Count - 1;
                Destroy(milTList[lastIndex]);
                UIManager.main.UI.Remove(milTList[lastIndex]);
                milTList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(ideasPrefab, milT);
                milTList.Add(item);
            }
        }
        for (int i = 0; i < milTList.Count; i++)
        {
            IdeaGroup idea = Map.main.IdeasM[i];
            SetupIdeaGroupShop(milTList[i], idea);
        }
    }
    void CloseIdeaShop()
    {
        ideaShop.SetActive(false);
        ideaBack.SetActive(true);
    }
    void SelectIdeaGroup(IdeaGroup ideaGroup)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        int index = ideaGroup.type == 0 ? Map.main.IdeasA.ToList().IndexOf(ideaGroup) : ideaGroup.type == 1 ? Map.main.IdeasD.ToList().IndexOf(ideaGroup) : Map.main.IdeasM.ToList().IndexOf(ideaGroup);
        foreach(var idea in civ.ideaGroups)
        {
            if(idea != null && idea.active &&idea.id == index && idea.type == ideaGroup.type ) { return; }
        }
        civ.ideaGroups[SelectedIndex] = new IdeaGroupData(index,ideaGroup.type,0);
        ideaShop.SetActive(false);
        ideaBack.SetActive(true);
    }
    void SetupIdeaGroupShop(GameObject ideaGO, IdeaGroup ideaGroup)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        HoverText[] ideas = ideaGO.GetComponentsInChildren<HoverText>();
        for(int i = 0; i < ideas.Length; i++)
        {
            ideas[i].GetComponent<Button>().interactable = false;
            ideas[i].gameObject.SetActive(true);
            Image image = ideas[i].GetComponentsInChildren<Image>()[1];
            Idea idea = ideaGroup.ideas[i];
            ideas[i].text = idea.GetHoverText(civ);
            image.sprite = idea.icon;
        }       
        ideaGO.GetComponentInChildren<TextMeshProUGUI>().text = ideaGroup.name + (ideaGroup.type == 0 ? "<sprite index=1>" : ideaGroup.type == 1 ? "<sprite index=2>" :"<sprite index=3>");
        ideaGO.GetComponent<Button>().onClick.RemoveAllListeners();
        ideaGO.GetComponent<Button>().onClick.AddListener(delegate { SelectIdeaGroup(ideaGroup); });
    }
}
