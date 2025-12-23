using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PoliciesUI : MonoBehaviour
{
    [SerializeField] GameObject policyListItem,statListItem;
    [SerializeField] Transform policyListBack,statListBack;
    List<GameObject> policyList = new List<GameObject>();
    List<GameObject> statList = new List<GameObject>();
    List<Decision> decisions = new List<Decision>();    
    List<Decision> GetDecisions(Civilisation civ)
    {
        List<Decision> decisions = Map.main.decisions.ToList();
        foreach (Decision decision in decisions.ToList())
        {
            if (!decision.CanAppear(civ))
            {
                decisions.Remove(decision);
            }
        }
        return decisions;
    }
    private void Update()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        decisions = GetDecisions(civ);
        int amount = decisions.Count;
        while (policyList.Count != amount)
        {
            if (policyList.Count > amount)
            {
                int lastIndex = policyList.Count - 1;
                Destroy(policyList[lastIndex]);
                policyList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(policyListItem, policyListBack);
                int index = policyList.Count;
                policyList.Add(item);

                Button[] buttons = item.GetComponentsInChildren<Button>();
                buttons[0].onClick.AddListener(delegate { TakeDecision(index); });                
            }
        }
        for (int i = 0; i < decisions.Count; i++)
        {
            Decision decision = decisions[i];
            Image[] images = policyList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = policyList[i].GetComponentsInChildren<TextMeshProUGUI>();
            HoverText hoverText = policyList[i].GetComponentInChildren<HoverText>();
            texts[0].text = decision.name;
            bool wouldAccept = decision.CanTake(civ);
            images[1].color = wouldAccept ? Color.green : Color.red;
            images[1].sprite = wouldAccept ? UIManager.main.icons[0] : UIManager.main.icons[1];
            hoverText.text = decision.GetHoverText(civ);
        }

        
        KeyValuePair<string,Stat>[] stats = civ.stats.ToArray();
        amount = stats.Length;
        while (statList.Count != amount)
        {
            if (statList.Count > amount)
            {
                int lastIndex = statList.Count - 1;
                Destroy(statList[lastIndex]);
                statList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(statListItem, statListBack);               
                statList.Add(item);
            }
        }
        for (int i = 0; i < amount; i++)
        {
            KeyValuePair<string, Stat> stat = stats[i];
            Image[] images = statList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = statList[i].GetComponentsInChildren<TextMeshProUGUI>();
            HoverText hoverText = statList[i].GetComponentInChildren<HoverText>();
            texts[0].text = stat.Key;
            texts[1].text = Modifier.ToString(stat.Value.v,stat.Value);
            hoverText.text = stat.Value.ToString();
        }
    }
    
    void TakeDecision(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        Decision decision = decisions[id];
        if (decision.CanTake(civ))
        {
            decision.Take(civ);
        }
    }
}
