using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourtUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rulerName, heirName, advisorA, advisorD, advisorM;
    [SerializeField] Button advisorAB, advisorDB, advisorMB;
    [SerializeField] GameObject advisorShopPanel;
    [SerializeField] Transform advisorShopBack;
    [SerializeField] GameObject advisorPrefab;
    int advisorShopType = -1;
    List<GameObject> advisorList = new List<GameObject>();


    private void Start()
    {
        advisorAB.onClick.AddListener(delegate { OpenShop(0); });
        advisorDB.onClick.AddListener(delegate { OpenShop(1); });
        advisorMB.onClick.AddListener(delegate { OpenShop(2); });
    }
    void OpenShop(int id)
    {
        advisorShopPanel.SetActive(true);
        advisorShopType = id;
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.ruler.active)
        {
            rulerName.text = civ.ruler.Name + " "+civ.ruler.age.ToString(true, true) + civ.ruler.adminSkill + "<sprite index=1> " + civ.ruler.diploSkill + "<sprite index=2> " + civ.ruler.milSkill + "<sprite index=3>";
        }
        else
        {
            rulerName.text = "No Ruler";
        }
        if (civ.heir.active)
        {
            heirName.text = civ.heir.Name + " " + civ.heir.age.ToString(true, true) + civ.heir.adminSkill + "<sprite index=1> " + civ.heir.diploSkill + "<sprite index=2> " + civ.heir.milSkill + "<sprite index=3>";
        }
        else
        {
            heirName.text = "No Heir";
        }
        if (civ.advisorA.active)
        {
            advisorA.text = civ.advisorA.Name + " " + civ.advisorA.age.ToString(true,true) + civ.advisorA.skillLevel + "<sprite index=1> "  + civ.advisorA.effect + " " + Mathf.Round(civ.advisorA.effectStrength * 100f) + "%";
        }
        else
        {
            advisorA.text = "No Advisor";
        }
        if (civ.advisorD.active)
        {
            advisorD.text = civ.advisorD.Name + " " + civ.advisorD.age.ToString(true, true) + civ.advisorD.skillLevel + "<sprite index=2> "  + civ.advisorD.effect + " " + Mathf.Round(civ.advisorD.effectStrength * 100f) + "%";
        }
        else
        {
            advisorD.text = "No Advisor";
        }
        if (civ.advisorM.active)
        {
            advisorM.text = civ.advisorM.Name + " " + civ.advisorM.age.ToString(true, true) + civ.advisorM.skillLevel + "<sprite index=3> " + civ.advisorM.effect + " " + Mathf.Round(civ.advisorM.effectStrength * 100f) + "%";
        }
        else
        {
            advisorM.text = "No Advisor";
        }
        if (advisorShopType > -1 && advisorShopPanel.activeSelf)
        {
            List<Advisor> advisors = advisorShopType == 0 ? civ.advisorsA : advisorShopType == 1 ? civ.advisorsD : civ.advisorsM;
            advisors.RemoveAll(i => !i.active);
            string type = advisorShopType == 0 ? "<sprite index=1>" : advisorShopType == 1 ? "<sprite index=2>" : "<sprite index=3>";
            while (advisorList.Count != advisors.Count)
            {
                if (advisorList.Count > advisors.Count)
                {
                    int lastIndex = advisorList.Count - 1;
                    Destroy(advisorList[lastIndex]);
                    UIManager.main.UI.Remove(advisorList[lastIndex]);
                    advisorList.RemoveAt(lastIndex);
                }
                else
                {
                    GameObject item = Instantiate(advisorPrefab, advisorShopBack);
                    advisorList.Add(item);
                    item.GetComponentInChildren<Button>().onClick.AddListener(delegate { SelectAdvisor(advisorList.IndexOf(item)); });
                    UIManager.main.UI.Add(item);
                }
            }
            for (int i = 0; i < advisors.Count; i++)
            {
                Advisor advisor = advisors[i];
                advisorList[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = advisor.age.ToString() + " "+ type+" " + advisor.skillLevel + " " + advisor.effect + " " + Mathf.Round(advisor.effectStrength * 100f) + "%";
                advisorList[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Hire Cost: " +Mathf.Round(advisor.HireCost(civ)*100f)/100f + "<sprite index=0>. Salary: " + Mathf.Round(advisor.Salary(civ) * 100f) / 100f + "<sprite index=0> per day.";
            }
        }
       
    }
    void SelectAdvisor(int advisorID)
    {
        if (Player.myPlayer.myCivID == -1 || advisorShopType == -1 || !advisorShopPanel.activeSelf) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        List<Advisor> advisors = advisorShopType == 0 ? civ.advisorsA : advisorShopType == 1 ? civ.advisorsD : civ.advisorsM;
        if (civ.coins >= advisors[advisorID].HireCost(civ))
        {
            civ.AssignAdvisor(advisors[advisorID]);
            advisorShopPanel.SetActive(false);
        }
    }
}
