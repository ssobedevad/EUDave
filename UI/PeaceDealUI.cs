using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PeaceDealUI : MonoBehaviour
{
    [SerializeField] GameObject peaceOptionPrefab;
    List<GameObject> peaceList = new List<GameObject>();
    [SerializeField] Transform listBack;
    [SerializeField] Button clear, suggest, send;
    [SerializeField] Image attacker, defender;
    [SerializeField] TextMeshProUGUI summary, resources;
    public PeaceDeal peaceDeal;
    public bool refresh;
    private void Start()
    {
        clear.onClick.AddListener(Clear);
        send.onClick.AddListener(Send);
        suggest.onClick.AddListener(Suggest);
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
        War war = Game.main.ongoingWars.Find(i => i.Between(civ.CivID,myCiv.CivID));
        if(war == null) { gameObject.SetActive(false); return; }
        if(peaceDeal == null || peaceDeal.war != war)
        {
            peaceDeal = new PeaceDeal(war,war.attackerCiv == myCiv);
            refresh = true;
        }
        if (myCiv.atWarWith.Contains(civ.CivID))
        {
            List<Vector3Int> provs = civ.GetAllCivTiles();
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
            if (refresh)
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
                    text[2].text = tileData.tileResource.name;
                    image[0].color = peaceDeal.provinces.Contains(province) ? Color.green : Color.red;
                    image[1].color = civ.c;
                }
                resources.text = Mathf.Round(peaceDeal.warScore) + "% War Score";                    
                refresh = false;
            }
            summary.text = "<#00ff00>" + GetPositiveReasons(peaceDeal,civ,war);
            summary.text += "\n<#ff0000>" + GetNegativeReasons(peaceDeal, civ, war);
            float reasons = Mathf.Round(Reasons(peaceDeal, civ, war));
            summary.text += "\n"+(reasons > 0 ? "<#00ff00>" : "<#ff0000>") +"Total Reasons: " + reasons;
        }
        
    }
    void ToggleProvince(int id)
    {

        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = Game.main.ongoingWars.Find(i => i.Between(civ.CivID, myCiv.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
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
        War war = Game.main.ongoingWars.Find(i => i.Between(civ.CivID, myCiv.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
        {
            peaceDeal = Suggested(civ, war);
            refresh = true;
        }
    }
    void Clear()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = Game.main.ongoingWars.Find(i => i.Between(civ.CivID, myCiv.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
        {
            peaceDeal = new PeaceDeal(war, war.attackerCiv == myCiv);
        }
        refresh = true;
    }

    void Send()
    {
        Civilisation civ = Game.main.civs[DiplomacyUIPanel.main.diploCivID];
        Civilisation myCiv = Player.myPlayer.myCiv;
        War war = Game.main.ongoingWars.Find(i => i.Between(civ.CivID, myCiv.CivID));
        if (peaceDeal != null || peaceDeal.war == war)
        {
            if (WillAccept(peaceDeal,civ,war))
            { 
                civ.AcceptPeaceDeal(peaceDeal);
                war.EndWar();
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
        PeaceDeal PeaceDeal = new PeaceDeal(war, war.attackerCiv == other);
        PriorityQueue<Vector3Int,float> possible = new PriorityQueue<Vector3Int,float>();
        List<Vector3Int> visited = PeaceDeal.possible.ToList();
        visited.ForEach(i => possible.Enqueue(i, ProvinceValue(i,other.capitalPos)));
        int loops = 0;
        while(possible.Count > 0 && loops < 100)
        {
            TileData tileData = Map.main.GetTile(possible.Dequeue());
            if (PeaceDeal.warScore + tileData.GetWarScore(other.CivID) < war.warScore)
            {
                PeaceDeal.AddProvince(tileData.pos);
                List<Vector3Int> neighbors = tileData.GetNeighbors();
                foreach (var neighbor in neighbors)
                {
                    TileData n = Map.main.GetTile(neighbor);
                    if(n.civID != target.CivID) { continue; }
                    if (n.occupied && n.occupiedByID != other.CivID) { continue; }
                    if (!visited.Contains(neighbor))
                    {
                        possible.Enqueue(neighbor, ProvinceValue(neighbor, other.capitalPos));
                        visited.Add(neighbor);
                    }
                }
            }
            loops ++;
        }              
        return PeaceDeal;        
    }

    public static string GetPositiveReasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = war.attackerCiv == target;
        float LengthOfWar = 45f - war.lengthOfWar / 30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength()/1000f - war.defenderCiv.TotalMilStrength()/1000f) * 12f * (isAttacker ? 1f : -1f);
        string reasons = "";
        if (LengthOfWar < 0) { reasons += "Length Of War: " + Mathf.Round(LengthOfWar); }
        if (relMilStrength < 0) { reasons += "\nRelative Military Strength: " + Mathf.Round(-relMilStrength); }
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        if (realWarScore > 0)
        {
            reasons += "\nWar Score: " + Mathf.Round(realWarScore);
        }       
        if (realWarScore == 100f && peaceDeal.warScore <= 100f) { reasons += "\nSurrender: " + 1000; }
        if(reasons.Length == 0) { reasons = "No Positive Reasons"; }
        return reasons;
    }
    public static string GetNegativeReasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = war.attackerCiv == target;
        float LengthOfWar = 45f - war.lengthOfWar / 30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength()/1000f - war.defenderCiv.TotalMilStrength()/1000f) * 12f * (isAttacker ? 1f : -1f);
        string reasons = "";
        if (LengthOfWar > 0) { reasons += "Length Of War: " + Mathf.Round(LengthOfWar); }
        if (relMilStrength > 0) { reasons += "\nRelative Military Strength: " + Mathf.Round(relMilStrength); }
        if (peaceDeal.warScore > 0) { reasons += "\nPeace Deal: " + Mathf.Round(peaceDeal.warScore); }
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        if (realWarScore < 0)
        {
            reasons += "\nWar Score: " + Mathf.Round(realWarScore);
        }
        if (peaceDeal.fullAnnexation) { reasons += "\nDemands Full Annexation: " + 50; }
        if (realWarScore < 10f && peaceDeal.warScore > 0f) { reasons += "\nWar Score Lower Than 10%: " + 1000; }
        if (!Map.main.GetTile(target.capitalPos).occupied) { reasons += "\nOwns Capital: " + 5; }
        if (peaceDeal.warScore > realWarScore) { reasons += "\nUnreasonable Demands: " + Mathf.Round(peaceDeal.warScore - realWarScore); }
        if (peaceDeal.warScore > 100f) { reasons += "\nUnreasonable Demands: " + 1000; }
        if (reasons.Length == 0) { reasons = "No Negative Reasons"; }
        return reasons;
    }
    public static float Reasons(PeaceDeal peaceDeal, Civilisation target, War war)
    {
        bool isAttacker = war.attackerCiv == target;
        float LengthOfWar = 45f - war.lengthOfWar/30f * 0.75f;
        float relMilStrength = Mathf.Atan(war.attackerCiv.TotalMilStrength() / 1000f - war.defenderCiv.TotalMilStrength() / 1000f) * 12f * (isAttacker ? 1f : -1f);
        float realWarScore = war.warScore * (isAttacker ? -1f : 1f);
        float positiveReasons = realWarScore;
        float negativeReasons = peaceDeal.warScore;
        if (!Map.main.GetTile(target.capitalPos).occupied) { negativeReasons += 5f; }
        if (LengthOfWar > 0) { negativeReasons += LengthOfWar; }
        else if (LengthOfWar < 0) { positiveReasons -= LengthOfWar; }

        if (relMilStrength > 0) { negativeReasons += relMilStrength; }
        else if (relMilStrength < 0) { positiveReasons -= relMilStrength; }

        if (realWarScore < 10f && peaceDeal.warScore > 0f) { negativeReasons += 1000f; }

        if (peaceDeal.warScore > 100f) { negativeReasons += 1000f; }
        if (peaceDeal.fullAnnexation) { negativeReasons += 50f; }
        if (peaceDeal.warScore > realWarScore) { negativeReasons += peaceDeal.warScore - realWarScore; }

        if (realWarScore == 100f && peaceDeal.warScore <= 100f) { positiveReasons += 1000f; }

        return positiveReasons - negativeReasons;
    }

    public static bool WillAccept(PeaceDeal peaceDeal,Civilisation target,War war)
    {
        return Reasons(peaceDeal, target, war) > 0;
    }
}
