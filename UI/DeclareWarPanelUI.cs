using System.Collections.Generic;
using System.Linq;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class DeclareWarPanelUI : MonoBehaviour
{
    [SerializeField] Image attacker, defender;
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] GameObject provincePrefab, civPrefab;
    [SerializeField] Transform provBack, allyBack, enemyBack;
    [SerializeField] Button confirm, cancel;
    public List<GameObject> allies, eAllies, provs;
    Vector3Int warGoal;
    CasusBelli casusBelli;
    private void Awake()
    {
        confirm.onClick.AddListener(Confirm);
        cancel.onClick.AddListener(Cancel);
        allies = new List<GameObject>();
        eAllies = new List<GameObject>();  
        provs = new List<GameObject>();
    }
    public void Open()
    {
        SelectWarGoal(0);
    }
    void Confirm()
    {
        if (DiplomacyUIPanel.main.diploCivID == -1) { return; }
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation Target = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation player = Player.myPlayer.myCiv;
        player.DeclareWar(Target.CivID,warGoal,casusBelli);
        if (casusBelli.Name == "")
        {
            player.AddStability(-2);
            player.ApplyAE(Map.main.GetTile(Target.capitalPos), Target, 20);
        }
        DiplomacyUIPanel.main.CancelWarDec();
    }
    void Cancel()
    {
        DiplomacyUIPanel.main.CancelWarDec();
    }
    public static List<WarGoal> GetPossibleWarGoals(Civilisation attacker,Civilisation defender)
    {
        List<WarGoal> goals = new List<WarGoal>();
        if (attacker.rivals.Contains(defender.CivID))
        {
            goals.Add(new WarGoal(Map.main.casusBellis[3], defender.capitalPos,attacker.CivID));
        }
        if(attacker.canHolyWar && attacker.religion != defender.religion && attacker.civNeighbours.Contains(defender.CivID))
        {
            goals.Add(new WarGoal(Map.main.casusBellis[2], defender.capitalPos, attacker.CivID));
        }
        foreach (var prov in defender.GetAllCivTiles())
        {
            if (attacker.government == 4 && attacker.religion != defender.religion && attacker.civNeighbours.Contains(defender.CivID) && Map.main.GetTile(prov).religion == attacker.religion)
            {
                goals.Add(new WarGoal(Map.main.casusBellis[2], prov, attacker.CivID));
            }
            if (attacker.claims.Contains(prov))
            {                
                goals.Add(new WarGoal(Map.main.casusBellis[0], prov, attacker.CivID));
            }
            if (attacker.government == 3 && attacker.civNeighbours.Contains(defender.CivID) && attacker.CanCoreTile(Map.main.GetTile(prov)))
            {
                goals.Add(new WarGoal(Map.main.casusBellis[4], prov, attacker.CivID));
            }
            if (attacker.cores.Contains(prov))
            {
                goals.Add(new WarGoal(Map.main.casusBellis[1], prov, attacker.CivID));
            }
            foreach(var subject in attacker.subjects.ConvertAll(i => Game.main.civs[i]))
            {
                if (subject.claims.Contains(prov))
                {
                    goals.Add(new WarGoal(Map.main.casusBellis[0], prov, subject.CivID));
                }
                if (subject.cores.Contains(prov))
                {
                    goals.Add(new WarGoal(Map.main.casusBellis[1], prov, subject.CivID));
                }
            }
        }
        return goals;
    }
    public string GetWarName(Civilisation attackerCiv ,Civilisation defenderCiv,CasusBelli cb,Vector3Int Wargoal)
    {
        if (cb.superiority)
        {
            return attackerCiv.civName + " " + cb.Name + " of " + defenderCiv.civName;
        }
        else if (cb.capital)
        {
            return attackerCiv.civName + " " + cb.Name + " of " + defenderCiv.civName;
        }
        else if (cb.province)
        {
            if (Wargoal != Vector3Int.zero)
            {
                return attackerCiv.civName + " " + cb.Name + " of " + Map.main.GetTile(Wargoal).Name;
            }
            else
            {
                return attackerCiv.civName + " " + cb.Name + " of " + defenderCiv.civName;
            }
        }       
        else
        {
            return attackerCiv.civName + " War Of Aggression\n" + " This will result in -2 Stability and 20 Aggressive Expansion";
        }
    }
    void SelectWarGoal(int id)
    {
        if (DiplomacyUIPanel.main.diploCivID == -1) { return; }
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation target = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation player = Player.myPlayer.myCiv;
        List<WarGoal> goals = GetPossibleWarGoals(player, target);
        if (goals.Count > 0)
        {
            WarGoal goal = goals[id];
            casusBelli = goal.cb;
            warGoal = goal.target;
        }
        else
        {
            casusBelli = new CasusBelli();
            warGoal = Vector3Int.zero;
        }
        title.text = GetWarName(player, target,casusBelli, warGoal);
    }
    public void SetupWar()
    {
        if (DiplomacyUIPanel.main.diploCivID == -1) { return; }
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation target = Game.main.civs[DiplomacyUIPanel.main.diploCivID];       
        Civilisation player = Player.myPlayer.myCiv;
        bool independence = target.CivID == player.overlordID;
        while (target.overlordID > -1)
        {
            target = Game.main.civs[target.overlordID];
        }
        if (player.atWarWith.Contains(target.CivID) ||
                target.CivID == player.CivID ||
                player.truces[target.CivID] > 0)
        {
            title.text = "Invalid War";
            return;
        }

        List<int> attackerAllies = player.allies.ToList();
        if (player.subjects.Count > 0)
        {
            attackerAllies.AddRange(player.subjects);
        }
        foreach (var attacker in attackerAllies.ToList())
        {
            Civilisation ally = Game.main.civs[attacker];
            if(ally.subjects.Count > 0)
            {
                attackerAllies.AddRange(ally.subjects);
            }
        }
        List<int> defenderAllies = target.allies.ToList();
        if (target.subjects.Count > 0)
        {
            foreach (var subject in target.subjects)
            {
                if ((target.CivID == player.CivID || player.allies.Contains(subject)))
                {
                    Civilisation sub = Game.main.civs[subject];
                    if (sub.overlordID > -1 && sub.libertyDesire >= 50f)
                    {

                    }
                    else
                    {
                        defenderAllies.Add(subject);
                    }
                }
                else
                {
                    defenderAllies.Add(subject);
                }
            }          
        }
        foreach (var defender in defenderAllies.ToList())
        {
            Civilisation ally = Game.main.civs[defender];
            if (ally.subjects.Count > 0)
            {
                defenderAllies.AddRange(ally.subjects);
            }
        }
        List<WarGoal> goals = GetPossibleWarGoals(player, target);
        while (provs.Count != goals.Count)
        {
            if (provs.Count > goals.Count)
            {
                int lastIndex = provs.Count - 1;
                Destroy(provs[lastIndex]);
                provs.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(provincePrefab, provBack);
                int index = provs.Count;
                provs.Add(item);
                item.GetComponent<Button>().onClick.AddListener(delegate { SelectWarGoal(index); });
            }
        }
        for (int i = 0; i < goals.Count; i++)
        {
            WarGoal goal = goals[i];
            Image[] images = provs[i].GetComponentsInChildren<Image>();
            images[1].GetComponentInChildren<Image>().color = Game.main.civs[goal.fromCivID].c;
            TextMeshProUGUI[] texts = provs[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = GetWarName(player,target,goal.cb,goal.target);
        }
        while (allies.Count != attackerAllies.Count)
        {
            if (allies.Count > attackerAllies.Count)
            {
                int lastIndex = allies.Count - 1;
                Destroy(allies[lastIndex]);
                allies.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(civPrefab, allyBack);
                allies.Add(item);
            }
        }
        for (int i = 0; i < attackerAllies.Count; i++)
        {
            Civilisation ally = Game.main.civs[attackerAllies[i]];
            string hoverText = "";
            hoverText = "<#00ff00>" + GetPositiveReasons(target, player, ally,false);
            hoverText += "\n<#ff0000>" + GetNegativeReasons(target, player, ally, false);
            float reasons = Mathf.Round(CallToArms(target, player, ally, false));
            hoverText += "\n" + (reasons > 0 ? "<#00ff00>" : "<#ff0000>") + "Total Reasons: " + reasons;
            Image[] images = allies[i].GetComponentsInChildren<Image>();
            images[1].GetComponentInChildren<Image>().color = ally.c;
            bool wouldAccept = CallToArms(target, player, ally, false) > 0;
            if (ally.overlordID == -1 || ally.overlordID == target.CivID)
            {
                images[3].GetComponentInChildren<Image>().color = wouldAccept ? Color.green : Color.red;
                images[3].GetComponentInChildren<Image>().sprite = wouldAccept ? UIManager.main.icons[0] : UIManager.main.icons[1];
                images[3].GetComponentInChildren<HoverText>().text = hoverText;
            }
            else
            {
                Civilisation overlord = Game.main.civs[ally.overlordID];
                images[3].GetComponentInChildren<Image>().color = Color.gray ;
                images[3].GetComponentInChildren<Image>().sprite = CallToArms(target, player, overlord, false) > 0 ? UIManager.main.icons[0] : UIManager.main.icons[1];
                images[3].GetComponentInChildren<HoverText>().text = "Will join if their overlord " + Game.main.civs[ally.overlordID].civName + " will join";
            }
            TextMeshProUGUI[] texts = allies[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = ally.civName;
        }
        while (eAllies.Count != defenderAllies.Count)
        {
            if (eAllies.Count > defenderAllies.Count)
            {
                int lastIndex = eAllies.Count - 1;
                Destroy(eAllies[lastIndex]);
                eAllies.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(civPrefab, enemyBack);
                eAllies.Add(item);
            }
        }
        for (int i = 0; i < defenderAllies.Count; i++)
        {
            Civilisation ally = Game.main.civs[defenderAllies[i]];
            string hoverText = "";
            hoverText = "<#00ff00>" + GetPositiveReasons(player, target, ally, true);
            hoverText += "\n<#ff0000>" + GetNegativeReasons(player, target, ally, true);
            float reasons = Mathf.Round(CallToArms(player, target, ally, true));
            hoverText += "\n" + (reasons > 0 ? "<#00ff00>" : "<#ff0000>") + "Total Reasons: " + reasons;
            Image[] images = eAllies[i].GetComponentsInChildren<Image>();
            images[1].GetComponentInChildren<Image>().color = ally.c;
            bool wouldAccept = CallToArms(player, target,ally, true) > 0;
            if (ally.overlordID == -1)
            {
                images[3].GetComponentInChildren<Image>().color = wouldAccept ? Color.green : Color.red;
                images[3].GetComponentInChildren<Image>().sprite = wouldAccept ? UIManager.main.icons[0] : UIManager.main.icons[1];
                images[3].GetComponentInChildren<HoverText>().text = hoverText;
            }
            else
            {
                Civilisation overlord = Game.main.civs[ally.overlordID];
                images[3].GetComponentInChildren<Image>().color = Color.gray;
                images[3].GetComponentInChildren<Image>().sprite = CallToArms(player, target, overlord, true) > 0 ? UIManager.main.icons[0] : UIManager.main.icons[1];
                images[3].GetComponentInChildren<HoverText>().text = "Will join if their overlord " + Game.main.civs[ally.overlordID].civName + " will join";
            }
            TextMeshProUGUI[] texts = eAllies[i].GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = ally.civName;
        }
    }
    private void OnGUI()
    {
        if(DiplomacyUIPanel.main.diploCivID == -1) { return; }
        if(Player.myPlayer.myCivID == -1) { return; }
        Civilisation Target = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation player = Player.myPlayer.myCiv;
        attacker.color = player.c;
        defender.color = Target.c;
        SetupWar();
    }
    public static string GetPositiveReasons(Civilisation target,Civilisation ally, Civilisation thisCiv,bool defensive = false)
    {
        string reasons = defensive ? "Defensive War: 30\n" : "";
        reasons += ally.diploRep.v > 0 ? "Diplo Rep: " +ally.diploRep.v * 5f + "\n" : "";
        reasons += thisCiv.opinionOfThem[target.CivID].v < 0 ? "Opinion of target: " + thisCiv.opinionOfThem[target.CivID].v * -0.25f + "\n" : "";
        reasons += thisCiv.opinionOfThem[ally.CivID].v > 0 ? "Opinion of us: " + thisCiv.opinionOfThem[ally.CivID].v * 0.25f + "\n" : "";
        reasons += ((thisCiv.GetTotalTilePopulation() + 1f) / (thisCiv.GetTotalMaxPopulation() + 1f)) > 0.5f ? "High population: " + 20f * ((thisCiv.GetTotalTilePopulation() + 1f) / (thisCiv.GetTotalMaxPopulation() + 1f) - 0.5f) + "\n" : "";
        return reasons;
    }
    public static string GetNegativeReasons(Civilisation target, Civilisation ally, Civilisation thisCiv, bool defensive = false)
    {
        string reasons = "";
        reasons += ally.diploRep.v < 0 ? "Diplo Rep: " + ally.diploRep.v * 5f + "\n" : "";
        reasons += thisCiv.GetWars().Count > 0 ? "Already in a war: -20\n" : "";
        reasons += thisCiv.opinionOfThem[target.CivID].v > 0 ? "Opinion of target: " + thisCiv.opinionOfThem[target.CivID].v * -0.25f + "\n" : "";
        reasons += thisCiv.opinionOfThem[ally.CivID].v < 0 ? "Opinion of us: " + thisCiv.opinionOfThem[ally.CivID].v * 0.25f + "\n" : "";
        reasons += Mathf.Max(0, thisCiv.MinimumDistTo(target) - 10) > 0 ? "Distance to target: " + Mathf.Max(0, thisCiv.MinimumDistTo(target) - 10) * -1f + "\n" : "";
        reasons += ((thisCiv.GetTotalTilePopulation() + 1f) / (thisCiv.GetTotalMaxPopulation() + 1f)) < 0.5f ? "Low population: " + 20f * ((thisCiv.GetTotalTilePopulation() + 1f) / (thisCiv.GetTotalMaxPopulation() + 1f) - 0.5f) + "\n" : "";
        reasons += thisCiv.loans.Count > 0 ? "Has Loans: " + thisCiv.loans.Count * (defensive ? -0.25f : -1f) + "\n" : "";
        reasons += thisCiv.truces[target.CivID] > 0 ? "Has a Truce with target: -1000 \n" : "";
        reasons += thisCiv.atWarWith.Contains(target.CivID) ? "Already at war with target: -1000 \n" : "";
        reasons += thisCiv.atWarWith.Contains(ally.CivID) ? "Already at war with you: -1000 \n" : "";        
        reasons += thisCiv.atWarTogether.Contains(target.CivID) ? "Fighting in another war with them: -1000 \n" : "";
        if (defensive)
        {
            reasons += thisCiv.overlordID > -1 ? "Is a subject -1000": "";
        }
        else
        {
            reasons += (thisCiv.overlordID > -1 && target.CivID != thisCiv.overlordID) ? "Is a subject -1000" : "";
        }
        if (thisCiv.allies.Contains(target.CivID) && !defensive)
        {
            reasons += thisCiv.CallToArms(ally, target, true) ? "Would join defender instead -1000\n" : "";
        }
        return reasons;
    }
    public static float CallToArms(Civilisation target, Civilisation ally,Civilisation thisCiv, bool defensive = false)
    {
        float choice = defensive ? 30 : 0;
        if (defensive) 
        {
            choice += thisCiv.overlordID > -1 ? -1000f : 0f;
        }
        else
        {
            choice += (thisCiv.overlordID > -1 && target.CivID != thisCiv.overlordID) ? -1000f : 0f;
        }
        choice += 5f * ally.diploRep.v;
        choice += thisCiv.GetWars().Count > 0 ? -20f : 0f;
        choice += thisCiv.opinionOfThem[target.CivID].v * -0.25f;
        choice += thisCiv.opinionOfThem[ally.CivID].v * 0.25f ;
        choice += Mathf.Max(0, thisCiv.MinimumDistTo(target) - 10) > 0 ? Mathf.Max(0, thisCiv.MinimumDistTo(target) - 10) * -1f : 0f;
        choice += 20f * ((thisCiv.GetTotalTilePopulation() + 1f) / (thisCiv.GetTotalMaxPopulation() + 1f) - 0.5f);
        choice += thisCiv.loans.Count > 0 ? thisCiv.loans.Count * (defensive ? -0.25f : -1f) : 0f;
        choice += thisCiv.truces[target.CivID] > 0 ? -1000 : 0;
        choice += thisCiv.atWarWith.Contains(ally.CivID) ? -1000 : 0;
        choice += thisCiv.atWarWith.Contains(target.CivID) ? -1000 : 0;
        choice += thisCiv.atWarTogether.Contains(target.CivID) ? -1000 : 0;
        if (thisCiv.allies.Contains(target.CivID) && !defensive)
        {
            choice += thisCiv.CallToArms(ally, target, true) ? -1000 : 0;
        }
        return choice;
    }
}
public class WarGoal
{
    public CasusBelli cb;
    public Vector3Int target;
    public int fromCivID;

    public WarGoal(CasusBelli cb, Vector3Int target, int fromCivID)
    {
        this.cb = cb;
        this.target = target;
        this.fromCivID = fromCivID;
    }
}
