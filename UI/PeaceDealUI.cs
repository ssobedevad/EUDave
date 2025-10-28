using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PeaceDealUI : MonoBehaviour
{
    [SerializeField] GameObject peaceOptionPrefab,peaceTermPrefab;
    List<GameObject> peaceList = new List<GameObject>();
    List<GameObject> diploList = new List<GameObject>();
    [SerializeField] Transform listBack,diploListBack;
    [SerializeField] Button clear, suggest, send ,addLoan,removeLoan;
    [SerializeField] Image attacker, defender,acceptTick;
    [SerializeField] TextMeshProUGUI summary, resources,loanValue,title;
    [SerializeField] Button[] buttons;
    [SerializeField] GameObject[] panels;
    public int menuMode;
    public PeaceDeal peaceDeal;
    public bool refresh;
    private void Start()
    {
        clear.onClick.AddListener(Clear);
        send.onClick.AddListener(Send);
        suggest.onClick.AddListener(Suggest);
        addLoan.onClick.AddListener(AddLoan);
        removeLoan.onClick.AddListener(RemoveLoan);
        for(int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(delegate { MenuMode(index); });
        }
        MenuMode(0);
    }
    void MenuMode(int mode)
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(mode == i);
        }
        menuMode = mode;
        refresh = true;
    }
    void AddLoan()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
        {
            peaceDeal.AddLoan();
        }
        refresh = true;
    }
    void RemoveLoan()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
        {
            peaceDeal.RemoveLoan();
        }
        refresh = true;
    }
    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Player.myPlayer.mapMode = 0;
            gameObject.SetActive(false);
        }
        if (DiplomacyUIPanel.main.diploCivID == -1) { gameObject.SetActive(false);return; }
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if(war == null) { gameObject.SetActive(false); return; }
        if(peaceDeal == null || peaceDeal.war != war || peaceDeal.target != civ || peaceDeal.taker != myCiv)
        {
            peaceDeal = new PeaceDeal(war, civ, myCiv);
            refresh = true;
        }
        bool isPrimary = (civ == war.attackerCiv || civ == war.defenderCiv);
        bool isAttacker = (war.attackerCiv == civ || war.attackerAllies.Contains(civ));
        if (isPrimary)
        {
            title.text = "War Score: " + war.warScore * (isAttacker? 1 : -1);
        }
        else
        {

            title.text = "War Score: " + war.IndividualWarScore(civ, isAttacker) * (isAttacker ? 1 : -1);
        }
        if (myCiv.atWarWith.Contains(civ.CivID))
        {
            List<Vector3Int> provs = civ.GetAllCivTiles();
            List<string> peaceTerms = new List<string>() {"Subjugation" };
            if (menuMode == 0)
            {             
                while (peaceList.Count != provs.Count)
                {
                    if (peaceList.Count > provs.Count)
                    {
                        int lastIndex = peaceList.Count - 1;
                        Destroy(peaceList[lastIndex]);
                        peaceList.RemoveAt(lastIndex);
                    }
                    else
                    {
                        GameObject item = Instantiate(peaceOptionPrefab, listBack);
                        peaceList.Add(item);
                        item.GetComponent<Button>().onClick.AddListener(delegate { ToggleProvince(peaceList.IndexOf(item)); });
                    }
                    refresh = true;
                }
            }
            else
            {
                while (diploList.Count != peaceTerms.Count)
                {
                    if (diploList.Count > peaceTerms.Count)
                    {
                        int lastIndex = diploList.Count - 1;
                        Destroy(diploList[lastIndex]);
                        diploList.RemoveAt(lastIndex);
                    }
                    else
                    {
                        GameObject item = Instantiate(peaceTermPrefab, diploListBack);
                        diploList.Add(item);
                        item.GetComponent<Button>().onClick.AddListener(delegate { ToggleTreaty(diploList.IndexOf(item)); });
                    }
                    refresh = true;
                }
            }
            if (refresh)
            {
                if (menuMode == 0)
                {
                    for (int i = 0; i < provs.Count; i++)
                    {
                        GameObject peaceItem = peaceList[i];
                        Vector3Int province = provs[i];
                        TileData tileData = Map.main.GetTile(province);
                        var text = peaceItem.GetComponentsInChildren<TextMeshProUGUI>();
                        var image = peaceItem.GetComponentsInChildren<Image>();
                        text[0].text = tileData.Name;
                        text[1].text = Mathf.Round(tileData.GetWarScore(myCiv.CivID) * 100f) / 100f + "%";
                        text[2].text = Mathf.Round(tileData.totalDev * 0.8f * 100f) / 100f + "%";
                        image[0].color = peaceDeal.provinces.Contains(province) ? Color.green : Color.red;
                        image[1].color = civ.c;
                    }                                       
                }
                else
                {
                    for (int i = 0; i < peaceTerms.Count; i++)
                    {
                        GameObject peaceItem = diploList[i];                        
                        var text = peaceItem.GetComponentsInChildren<TextMeshProUGUI>();
                        var image = peaceItem.GetComponentsInChildren<Image>();
                        text[0].text = peaceTerms[i];
                        text[1].text = Mathf.Round(peaceDeal.WarScoreForSubjugation() * 100f) / 100f + "%";                        
                        image[0].color = peaceDeal.subjugation ? Color.green : Color.red;
                        image[1].color = civ.c;
                    }
                }
                refresh = false;
                loanValue.text = peaceDeal.numLoans * civ.GetLoanSize() + "<sprite index=0>";
                resources.text = Mathf.Round(peaceDeal.warScore) + " Cost";
            }
            if (civ.overlordID == -1)
            {
                summary.text = "<#00ff00>" + GetPositiveReasons(peaceDeal, civ, war);
                summary.text += "\n<#ff0000>" + GetNegativeReasons(peaceDeal, civ, war);
                float reasons = Mathf.Round(Reasons(peaceDeal, civ, war));
                summary.text += "\n" + (reasons > 0 ? "<#00ff00>" : "<#ff0000>") + "Total Reasons: " + reasons;
                acceptTick.sprite = reasons > 0 ? UIManager.main.icons[0] : UIManager.main.icons[1];
                acceptTick.color = reasons > 0 ? Color.green : Color.red;
                acceptTick.GetComponent<HoverText>().text = summary.text;
            }
            else
            {
                summary.text = "<#ff0000>Subjects cannot sign peace treaties";
                acceptTick.sprite = UIManager.main.icons[1];
                acceptTick.color = Color.red;
                acceptTick.GetComponent<HoverText>().text = summary.text;
            }
        }
        
    }
    void ToggleTreaty(int id)
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null && peaceDeal.war == war && peaceDeal.target == civ && peaceDeal.taker == myCiv)
        {
            if (id == 0)
            {
                if (!peaceDeal.subjugation)
                {
                    peaceDeal.RequestSubjugation();
                }
                else
                {
                    peaceDeal.RemoveSubjugation();
                }
            }
            refresh = true;
        }
    }
    void ToggleProvince(int id)
    {

        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null && peaceDeal.war == war && peaceDeal.target == civ && peaceDeal.taker == myCiv)
        {
            List<Vector3Int> provs = civ.GetAllCivTiles();
            if (peaceDeal.provinces.Contains(provs[id]))
            {
                peaceDeal.RemoveProvince(provs[id]);
            }
            else if (peaceDeal.possible.Contains(provs[id]))
            {
                peaceDeal.AddProvince(provs[id]);
            }
            refresh = true;
        }
    }
    void Suggest()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null && peaceDeal.war == war && peaceDeal.target == civ && peaceDeal.taker == myCiv)
        {
            peaceDeal = Suggested(civ, war);
            refresh = true;
        }
    }
    void Clear()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        if (peaceDeal != null && war != null)
        {
            peaceDeal = new PeaceDeal(war, civ, myCiv);
        }
        refresh = true;
    }

    void Send()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        if (civ.overlordID > -1) { return; }
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = myCiv.GetWars().Find(i => i.Involving(civ.CivID));
        bool mainTarget = (war.defenderCiv == civ && war.attackerCiv == myCiv) || (war.defenderCiv == myCiv && war.attackerCiv == civ);
        if (peaceDeal != null && peaceDeal.war == war && peaceDeal.target == civ && peaceDeal.taker == myCiv)
        {
            if (WillAccept(peaceDeal,civ,war))
            {                 
                if (mainTarget)
                {
                    war.EndWar();
                }
                else if (war.attackerCiv == myCiv || war.defenderCiv == myCiv)
                {
                    war.LeaveWar(civ.CivID);
                }
                else if (war.defenderCiv == civ || war.attackerCiv == civ)
                {
                    war.LeaveWar(myCiv.CivID);
                }
                civ.AcceptPeaceDeal(peaceDeal, mainTarget);
                Player.myPlayer.mapMode = 0;
            }
        }
    }
    public static float ProvinceValue(Vector3Int pos, Vector3Int fromPos)
    {
        TileData data = Map.main.GetTile(pos);
        if (data == null) { return 0; }
        float score = TileData.evenr_distance(pos, fromPos) * 10f;
        score /= (data.totalDev * data.tileResource.Value + data.fortLevel * 5f);
        return score;
    }
    public static PeaceDeal Suggested(Civilisation target, War war)
    {
        Civilisation other = war.GetOpposingLeader(target.CivID);
        bool forAttacker = (war.defenderCiv == target || war.defenderAllies.Contains(target));
        bool isPrimary = (target == war.attackerCiv || target == war.defenderCiv);
        PeaceDeal PeaceDeal = new PeaceDeal(war, target,other);
        if (!isPrimary)
        {
            if (PeaceDeal.warScore < (war.IndividualWarScore(target,forAttacker) - 5f))
            {
                for (int i = 0; i < Mathf.Min((100f - PeaceDeal.warScore) / 5f, 5); i++)
                {
                    PeaceDeal.AddLoan();
                }
            }
            return PeaceDeal;
        }
        PriorityQueue<Vector3Int,float> possible = new PriorityQueue<Vector3Int,float>();
        List<Vector3Int> visited = PeaceDeal.possible.ToList();
        visited.ForEach(i => possible.Enqueue(i, ProvinceValue(i,other.capitalPos)));
        int loops = 0;
        if(target.GetTotalWarScore(other.CivID) < 100)
        {
            if(other.remainingDiploRelations > 0 && other.subjects.Count == 0)
            {
                PeaceDeal.RequestSubjugation();
                Debug.Log("Subject");
            }
        }
        if (PeaceDeal.warScore < 95f)
        {
            while (possible.Count > 0 && loops < 100)
            {
                TileData tileData = Map.main.GetTile(possible.Dequeue());
                if (PeaceDeal.warScore + tileData.GetWarScore(other.CivID) < (war.warScore * (forAttacker ? 1f : -1f)))
                {
                    PeaceDeal.AddProvince(tileData.pos);
                    List<Vector3Int> neighbors = tileData.GetNeighbors();
                    foreach (var neighbor in neighbors)
                    {
                        TileData n = Map.main.GetTile(neighbor);
                        if (n.civID != target.CivID) { continue; }
                        if (n.occupied && n.occupiedByID != other.CivID) { continue; }
                        if (!visited.Contains(neighbor))
                        {
                            possible.Enqueue(neighbor, ProvinceValue(neighbor, other.capitalPos));
                            visited.Add(neighbor);
                        }
                    }
                }
                loops++;
            }
        }
        if (PeaceDeal.warScore < ((war.warScore - 5f) * (forAttacker ? 1f : -1f)))
        {
            for (int i = 0; i < Mathf.Min((100f - PeaceDeal.warScore) / 5f, 5); i++)
            {
                PeaceDeal.AddLoan();
            }
        }
        return PeaceDeal;        
    }

    public static string GetPositiveReasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = (war.attackerCiv == target || war.attackerAllies.Contains(target));
        bool isPrimary = (target == war.attackerCiv || target == war.defenderCiv);
        float LengthOfWar = 45f - war.lengthOfWar / 30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength()/1000f - war.defenderCiv.TotalMilStrength()/1000f) * 12f * (isAttacker ? 1f : -1f);
        string reasons = "Positive Reasons: \n";
        if (LengthOfWar < 0) { reasons += "Length Of War: " + Mathf.Round(LengthOfWar) + "\n"; }
        if (relMilStrength < 0) { reasons += "Relative Military Strength: " + Mathf.Round(-relMilStrength) + "\n"; }
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        if (!isPrimary)
        {
            realWarScore = war.IndividualWarScore(target, isAttacker);
        }
        if (realWarScore > 0)
        {
            reasons += "War Score: " + Mathf.Round(realWarScore) + "\n";
        }       
        float occupied = war.OccupiedAndBesiegedProvs(target);
        if(occupied > 0)
        {
            reasons += "Occupied and Besieged provinces: " + Mathf.Round(occupied) + "\n";
        }
        float pop = ((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) - 0.5f);
        if (pop < 0)
        {
            reasons += "Low Population: " + Mathf.Round(-20f * pop) + "\n";
        }

        if (realWarScore == 100f && peaceDeal.warScore <= 100f && isPrimary) { reasons += "Surrender: " + 1000 + "\n"; }
        if(reasons.Length == 0) { reasons = "No Positive Reasons"; }
        return reasons;
    }
    public static string GetNegativeReasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = (war.attackerCiv == target || war.attackerAllies.Contains(target));
        bool isPrimary = (target == war.attackerCiv || target == war.defenderCiv);
        bool hasAlly = isAttacker ? war.attackerAllies.Count > 0 : war.defenderAllies.Count > 0;
        float LengthOfWar = 45f - war.lengthOfWar / 30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength()/1000f - war.defenderCiv.TotalMilStrength()/1000f) * 12f * (isAttacker ? 1f : -1f);
        string reasons = "Negative Reasons: \n";
        if (LengthOfWar > 0) { reasons += "Length Of War: " + Mathf.Round(LengthOfWar) + "\n"; }
        if (relMilStrength > 0) { reasons += "Relative Military Strength: " + Mathf.Round(relMilStrength) + "\n"; }
        if (peaceDeal.warScore > 0) { reasons += "Peace Deal: " + Mathf.Round(peaceDeal.warScore) + "\n"; }
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        if (!isPrimary)
        {
            realWarScore = war.IndividualWarScore(target, isAttacker);
        }
        if (realWarScore < 0)
        {
            reasons += "War Score: " + Mathf.Round(realWarScore) + "\n";
        }
        float pop = ((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) - 0.5f);
        if(pop > 0)
        {
            reasons += "High Population: " + Mathf.Round(20f * pop) + "\n";
        }
        if (hasAlly) { reasons += "Ally in War: " + 20 + "\n"; }
        if (peaceDeal.fullAnnexation || peaceDeal.subjugation) { reasons += "Demands Full Annexation: " + 50 + "\n"; }
        if (realWarScore < 10f && peaceDeal.warScore > 0f) { reasons += "War Score Lower Than 10%: " + 1000 + "\n"; }
        if (!Map.main.GetTile(target.capitalPos).occupied) { reasons += "Owns Capital: " + 5 + "\n"; }
        if (peaceDeal.warScore > realWarScore && isPrimary && peaceDeal.warScore > 0) { reasons += "Unreasonable Demands: " + Mathf.Round(peaceDeal.warScore - realWarScore) + "\n"; }
        if (peaceDeal.warScore > 100f) { reasons += "Unreasonable Demands: " + 1000 + "\n"; }
        if (reasons.Length == 0) { reasons = "No Negative Reasons"; }
        return reasons;
    }
    public static float Reasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = (war.attackerCiv == target || war.attackerAllies.Contains(target));
        bool hasAlly = isAttacker ? war.attackerAllies.Count > 0 : war.defenderAllies.Count > 0;
        bool isPrimary = (target == war.attackerCiv || target == war.defenderCiv);
        float LengthOfWar = 45f - war.lengthOfWar/30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength() / 1000f - war.defenderCiv.TotalMilStrength() / 1000f) * 12f * (isAttacker ? 1f : -1f);
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        if (!isPrimary)
        {
            realWarScore = war.IndividualWarScore(target, isAttacker);
        }
        float positiveReasons = realWarScore;
        float negativeReasons = peaceDeal.warScore;
        if (hasAlly) { negativeReasons += 20f; }
        if (!Map.main.GetTile(target.capitalPos).occupied) { negativeReasons += 5f; }
        if (LengthOfWar > 0) { negativeReasons += LengthOfWar; }
        else if (LengthOfWar < 0) { positiveReasons -= LengthOfWar; }

        if((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) < 0.5f) { positiveReasons += 20f * ((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) - 0.5f); }
        if ((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) > 0.5f) { negativeReasons += 20f * ((target.GetTotalTilePopulation() + 1f) / (target.GetTotalMaxPopulation() + 1f) - 0.5f); }
        
        positiveReasons += war.OccupiedAndBesiegedProvs(target);

        if (relMilStrength > 0) { negativeReasons += relMilStrength; }
        else if (relMilStrength < 0) { positiveReasons -= relMilStrength; }

        if (realWarScore < 10f && peaceDeal.warScore > 0f) { negativeReasons += 1000f; }

        if (peaceDeal.warScore > 100f) { negativeReasons += 1000f; }
        if (peaceDeal.fullAnnexation || peaceDeal.subjugation) { negativeReasons += 50f; }

        if (peaceDeal.warScore > realWarScore && isPrimary && peaceDeal.warScore > 0) { negativeReasons += peaceDeal.warScore - realWarScore; }
        

        if (realWarScore == 100f && peaceDeal.warScore <= 100f && isPrimary) { positiveReasons += 1000f; }

        return positiveReasons - negativeReasons;
    }

    public static bool WillAccept(PeaceDeal peaceDeal,Civilisation target,War war)
    {
        return Reasons(peaceDeal, target, war) > 0;
    }
}
