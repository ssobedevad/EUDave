using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarUI : MonoBehaviour
{
    [SerializeField] GameObject warMiniPrefab;
    [SerializeField] Transform warMiniTransform;
    public List<GameObject> wars = new List<GameObject>();

    private void OnGUI()
    {
        if(Player.myPlayer.myCivID == -1) { return; }
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<War> civWars = myCiv.GetWars();       
        while (wars.Count != civWars.Count)
        {
            if (wars.Count > civWars.Count)
            {
                int lastIndex = wars.Count - 1;
                Destroy(wars[lastIndex]);
                UIManager.main.UI.Remove(wars[lastIndex]);
                wars.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(warMiniPrefab, warMiniTransform);
                int index = wars.Count;
                item.GetComponent<Button>().onClick.AddListener(delegate { OpenWarUI(index); });
                wars.Add(item);
                UIManager.main.UI.Add(item);
            }
        }
        for(int i = 0; i < civWars.Count;i++)
        {
            War civWar = civWars[i];
            wars[i].GetComponentInChildren<Image>().color = civWar.GetOpposingLeader(myCiv.CivID).c;
            wars[i].GetComponentInChildren<TextMeshProUGUI>().text = Mathf.Round(civWar.warScore) * ((civWar.attackerCiv == myCiv || civWar.attackerAllies.Contains(myCiv)) ? 1f : -1f) + "%";
        }
    }
    void OpenWarUI(int index)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation myCiv = Player.myPlayer.myCiv;
        List<War> civWars = myCiv.GetWars();
        UIManager.main.WarUI.GetComponent<WarInfoUI>().war = civWars[index];
        UIManager.main.WarUI.SetActive(true);
    }
}
