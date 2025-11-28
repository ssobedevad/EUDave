using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubjectsUI : MonoBehaviour
{
    [SerializeField] GameObject subjectPrefab,provincePrefab;
    [SerializeField] Transform  subjectTransform,provinceTransform;
    [SerializeField] GameObject[] subjectInteractions;
    [SerializeField] GameObject subjectInfo;
    [SerializeField] GameObject subjectPanel, provincePanel;
    public List<GameObject> subjects = new List<GameObject>();
    public List<GameObject> provinces = new List<GameObject>();
    Civilisation selected;
    bool grant;
    private void Start()
    {
        subjectInteractions[0].GetComponent<Button>().onClick.AddListener(SendGift);
        subjectInteractions[1].GetComponent<Button>().onClick.AddListener(GrantProvince);
        subjectInteractions[2].GetComponent<Button>().onClick.AddListener(SeizeLand);
        subjectInteractions[3].GetComponent<Button>().onClick.AddListener(PlacateRuler);
        subjectInteractions[4].GetComponent<Button>().onClick.AddListener(PayOffLoans);
        subjectInteractions[5].GetComponent<Button>().onClick.AddListener(BeginIntegration);
    }
    void BeginIntegration()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (civSubjects.Contains(selected) && selected.libertyDesire < 50f)
        {
            selected.integrating = !selected.integrating;
        }
    }
    void SendGift()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (civSubjects.Contains(selected))
        {
            float amount = selected.GetTotalIncome() * 30;
            if(civ.coins >= amount)
            {
                selected.coins += amount;
                civ.coins -= amount;
                selected.opinionOfThem[civ.CivID].AddModifier(new Modifier(25, 1, "Sent Gift", 4320));
                selected.SetLibertyDesire();
            }
        }
    }
    void SetupProvinceList()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (!civSubjects.Contains(selected)) { return; }
        List<TileData> tiles = civ.civTileDatas.ToList();
        if (grant)
        {
            tiles.Remove(Map.main.GetTile(civ.capitalPos));
            tiles.RemoveAll(i => !selected.CanCoreTile(i));
        }
        else
        {
            tiles = selected.civTileDatas.ToList();
            tiles.Remove(Map.main.GetTile(selected.capitalPos));
            tiles.RemoveAll(i => !civ.CanCoreTile(i));
        }
        while (provinces.Count != tiles.Count)
        {
            if (provinces.Count > tiles.Count)
            {
                int lastIndex = provinces.Count - 1;
                Destroy(provinces[lastIndex]);
                provinces.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(provincePrefab, provinceTransform);
                Button coreButton = item.GetComponent<Button>();
                int id = provinces.Count;
                coreButton.onClick.AddListener(delegate { ProvinceClick(id); });
                provinces.Add(item);
            }
        }
        for (int i = 0; i < tiles.Count; i++)
        {
            TileData tile = tiles[i];
            provinces[i].GetComponent<Image>().color = grant ? Color.green : Color.red;
            provinces[i].GetComponentInChildren<TextMeshProUGUI>().text = tile.Name;
            provinces[i].GetComponent<HoverText>().text = grant ? "Grant Province" : "Seize Land";
        }
    }
    void GrantProvince()
    {
        bool active = !subjectPanel.activeSelf;
        subjectPanel.SetActive(active); 
        provincePanel.SetActive(!active);
        grant = true;
        SetupProvinceList();
    }
    void SeizeLand()
    {
        bool active = !subjectPanel.activeSelf;
        subjectPanel.SetActive(active);
        provincePanel.SetActive(!active);
        grant = false;
        SetupProvinceList();
    }
    void PlacateRuler()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (civSubjects.Contains(selected))
        {
            if (civ.prestige >= 0)
            {
                civ.AddPrestige(-20);
                selected.libertyDesireTemp.IncreaseModifier("Placated Rulers" ,-10f, 1, Decay: true);
                selected.SetLibertyDesire();
            }
        }
    }
    void PayOffLoans()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (civSubjects.Contains(selected))
        {
            float amount = 0;
            int loanNum = selected.loans.Count;
            selected.loans.ForEach(i => amount += i.value);
            if (civ.coins >= amount)
            {
                selected.loans.Clear();
                civ.coins -= amount;
                selected.libertyDesireTemp.IncreaseModifier("Paid off our Loans", -5f * loanNum, 1, Decay: true);
                selected.SetLibertyDesire();
            }
        }
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
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
            texts[1].text = Mathf.Round(subject.opinionOfThem[civ.CivID].value) + "";
            texts[2].text = Mathf.Round(subject.libertyDesire ) + "%";
            float subjectIncome = subject.libertyDesire < 50f ? subject.GetTotalIncome() * (0.1f + civ.incomeFromSubjects.value) : 0f;
            texts[3].text = Mathf.Round(subjectIncome * 100f) /100f + "<sprite index=0>";
            texts[2].GetComponent<HoverText>().text = LibertyDesireText(subject);
        }
        TextMeshProUGUI[] subTexts = subjectInfo.GetComponentsInChildren<TextMeshProUGUI>();
        if (civSubjects.Contains(selected))
        {
            subTexts[0].text = selected.civName;
            subTexts[1].text = Mathf.Round(selected.GetBalance() * 100f)/100f + "<sprite index=0>";
            subTexts[2].text = Mathf.Round(selected.coins * 100f)/100f + "<sprite index=0>";
            subjectInteractions[5].GetComponentInChildren<TextMeshProUGUI>().text = (!selected.integrating ? "Begin Integration" : selected.annexationProgress + "/" + selected.GetIntegrationCost(civ) * 8f);
        }
        else
        {
            subTexts[0].text = "No Subject Selected";
            subTexts[1].text = "";
            subTexts[2].text = "";
            subjectInteractions[5].GetComponentInChildren<TextMeshProUGUI>().text = "Begin Integration";
        }
        
    }
    void SubjectClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        selected = civSubjects[id];
    }
    void ProvinceClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Civilisation> civSubjects = civ.subjects.ConvertAll(i => Game.main.civs[i]);
        if (!civSubjects.Contains(selected)) { return; }
        List<TileData> tiles = civ.civTileDatas.ToList();
        if (grant)
        {
            tiles.Remove(Map.main.GetTile(civ.capitalPos));
            tiles.RemoveAll(i => !selected.CanCoreTile(i));            
        }
        else
        {
            tiles = selected.civTileDatas.ToList();
            tiles.Remove(Map.main.GetTile(selected.capitalPos));
            tiles.RemoveAll(i => !civ.CanCoreTile(i));
        }
        TileData tile = tiles[id];
        if (grant)
        {
            tile.civID = selected.CivID;
            selected.libertyDesireTemp.IncreaseModifier("Granted Province", -1f * tile.totalDev, 1, Decay: true);
        }
        else if (selected.libertyDesire < 50f)
        {
            tile.civID = civ.CivID;
            selected.libertyDesireTemp.IncreaseModifier("Seized Land", 5f * tile.totalDev, 1, Decay: true);
        }
        SetupProvinceList();
        selected.Rebirth();
        selected.SetLibertyDesire();
        civ.updateBorders = true;
        selected.updateBorders = true;
    }
    string LibertyDesireText(Civilisation subject)
    {
        if (subject.overlordID == -1) { return "Not Valid Subject"; }
        Civilisation overlord = Game.main.civs[subject.overlordID];
        string ld = "Liberty Desire: " + Mathf.Round(subject.libertyDesire) + "%\n";
        ld += "From Relative Military Strength: " + Mathf.Round((subject.TotalMilStrength() + 1) / (overlord.TotalMilStrength() + 1) * 75f) + "%\n";
        foreach (var ally in subject.allies)
        {
            ld += "From Relative Military Strength of "+ Game.main.civs[ally].civName + ": " + Mathf.Round((Game.main.civs[ally].TotalMilStrength() + 1) / (overlord.TotalMilStrength() + 1) * 75f) + "%\n";
        }
        ld += "From Relative Economic Strength: " + Mathf.Round((subject.GetTotalIncome() + 1) / (overlord.GetTotalIncome() + 1) * 75f) + "%\n";
        ld += "From Overlord Diplomatic Reputation: " + Mathf.Round(-3f * overlord.diploRep.value) + "%\n";
        ld += "From Opinion of Overlord: " +Mathf.Round(-0.1f * subject.opinionOfThem[subject.overlordID].value) + "%\n";
        ld += (subject.diploTech - overlord.diploTech > 0)? "From Better Diplomatic Technology Than Overlord: " + Mathf.Max(0, subject.diploTech - overlord.diploTech) * 5f + "%\n" : "";
        ld += "From Total Development: " + Mathf.Round(subject.GetTotalDev() * 0.25f * (1f + overlord.libDesireFromDevForSubjects.value)) + "%\n";
        ld += "Overlord Bonuses: " +overlord.libDesireInSubjects.ToString() + "\n\n";
        ld += "Temporary Bonuses: " + Mathf.Round(subject.libertyDesireTemp.value*100f)/100f;
        return ld;
    }

}
