using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiplomacyUIPanel : MonoBehaviour
{
    [SerializeField] Button[] diplomaticActions;
    [SerializeField] Image civIcon, civReligion;
    [SerializeField] TextMeshProUGUI civName, civGovRankName, civRulerName;
    [SerializeField] TextMeshProUGUI adminTech, diploTech, milTech;
    [SerializeField] GameObject diploMiniPrefab;
    [SerializeField] Transform warTransform,truceTransform;
    List<GameObject> wars,truces;
    public static DiplomacyUIPanel main;
    public int diploCivID;

    private void Awake()
    {
        main = this;
        diplomaticActions[0].onClick.AddListener(DeclareWar);
        wars = new List<GameObject>();
        truces = new List<GameObject>();
    }
    private void OnGUI()
    {
        if (diploCivID == -1) { return; }
        Civilisation civ = Game.main.civs[diploCivID];
        civIcon.color = civ.c;
        civName.text = civ.civName;
        adminTech.text = civ.adminTech + "<sprite index=1><sprite index=8>";
        diploTech.text = civ.diploTech + "<sprite index=2><sprite index=8>";
        milTech.text = civ.milTech + "<sprite index=3><sprite index=8>";
        if (civ.ruler.active)
        {
            civRulerName.text = civ.ruler.Name + " ("+civ.ruler.age.years +") " + civ.ruler.adminSkill + "<sprite index=1> " + civ.ruler.diploSkill + "<sprite index=2> " + civ.ruler.milSkill +  "<sprite index=3>";
        }
        else
        {
            civRulerName.text = "No Ruler";
        }
        if (Player.myPlayer.myCivID > -1)
        {
            diplomaticActions[0].GetComponentInChildren<TextMeshProUGUI>().text = Player.myPlayer.myCiv.atWarWith.Contains(diploCivID)? "Sue for Peace" : "Declare War";
        }
        List<War> civWars = civ.GetWars();
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
                UIManager.main.UI.Remove(truces[lastIndex]);
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
    }
    void DeclareWar()
    {
        if (diploCivID == -1 || Player.myPlayer.myCivID == -1) { return; }
        if(diploCivID != Player.myPlayer.myCivID)
        {
            if (!Player.myPlayer.myCiv.atWarWith.Contains(diploCivID))
            {
                Player.myPlayer.myCiv.DeclareWar(diploCivID);
            }
            else
            {
                UIManager.main.PeaceDealUI.SetActive(true);
                UIManager.main.CivUI.SetActive(false);
                Player.myPlayer.mapMode = -1;
            }
        }
    }
}
