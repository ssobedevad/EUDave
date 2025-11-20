using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CourtUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rulerName, heirName, advisorA, advisorD, advisorM,admin,diplo,mil,salaryA,salaryD,salaryM;
    [SerializeField] TextMeshProUGUI rulerSkills, heirSkills, rulerAge, heirAge;
    [SerializeField] Button advisorAB, advisorDB, advisorMB,disinherit,abdicate,focusA,focusD,focusM,advisorAUpgrade,advisorAFire, advisorDUpgrade, advisorDFire, advisorMUpgrade, advisorMFire;
    [SerializeField] GameObject advisorShopPanel,courtPanel;
    [SerializeField] GameObject[] rulerTraits,heirTraits;
    [SerializeField] Transform advisorShopBack;
    [SerializeField] GameObject advisorPrefab;
    [SerializeField] Sprite blank;
    int advisorShopType = -1;
    List<GameObject> advisorList = new List<GameObject>();

    void SetFocus(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if(civ.focusCD > 0) { return; }
        civ.focus = id;
        civ.focusCD = 6;
    }
    private void OnDisable()
    {
        advisorShopPanel.SetActive(false);
        courtPanel.SetActive(true);
    }
    private void Start()
    {
        advisorAB.onClick.AddListener(delegate { OpenShop(0); });
        advisorDB.onClick.AddListener(delegate { OpenShop(1); });
        advisorMB.onClick.AddListener(delegate { OpenShop(2); });
        focusA.onClick.AddListener(delegate { SetFocus(0); });
        focusD.onClick.AddListener(delegate { SetFocus(1); });
        focusM.onClick.AddListener(delegate { SetFocus(2); });
        advisorAUpgrade.onClick.AddListener(delegate { Promote(0); });
        advisorDUpgrade.onClick.AddListener(delegate { Promote(1); });
        advisorMUpgrade.onClick.AddListener(delegate { Promote(2); });
        advisorAFire.onClick.AddListener(delegate { Fire(0); });
        advisorDFire.onClick.AddListener(delegate { Fire(1); });
        advisorMFire.onClick.AddListener(delegate { Fire(2); });
        disinherit.onClick.AddListener(DisInherit);
    }
    void Promote(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        Advisor advisor = id == 0 ? civ.advisorA : id == 1 ? civ.advisorD : civ.advisorM;
        if (!advisor.active) { return; }
        float PromoteCost = advisor.HireCost(civ) * 10f;
        if(civ.coins >= PromoteCost)
        {
            civ.coins -= PromoteCost;
            advisor.skillLevel++;
        }
    }
    void Fire (int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        Advisor advisor = id == 0? civ.advisorA : id == 1? civ.advisorD : civ.advisorM;
        if (!advisor.active) { return; }
        advisor.active = false;
        civ.RemoveAdvisor(advisor);
    }
    void DisInherit()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.prestige >= 0 && civ.heir.active)
        {
            civ.heir.active = false;
            civ.AddPrestige(-50);
        }
    }
    void OpenShop(int id)
    {
        advisorShopPanel.SetActive(true);
        courtPanel.SetActive(false);
        advisorShopType = id;
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        focusA.interactable = civ.focusCD == 0;
        focusD.interactable = civ.focusCD == 0;
        focusM.interactable = civ.focusCD == 0;
        focusA.image.color = civ.focus == 0 ? Color.green : Color.white;
        focusD.image.color = civ.focus == 1 ? Color.green : Color.white;
        focusM.image.color = civ.focus == 2 ? Color.green : Color.white;
        admin.text = "";
        diplo.text = "";
        mil.text = "";
        for (int i = 0; i < rulerTraits.Length; i++)
        {
            if (civ.ruler.active && civ.ruler.traits.Count > i)
            {
                rulerTraits[i].GetComponent<Image>().sprite = civ.ruler.traits[i].icon;
                rulerTraits[i].GetComponent<HoverText>().text = civ.ruler.traits[i].GetHoverText(civ);
            }
            else
            {
                rulerTraits[i].GetComponent<Image>().sprite = UIManager.main.icons[2];
                rulerTraits[i].GetComponent<HoverText>().text = "";
            }
            if (civ.heir.active && civ.heir.traits.Count > i)
            {
                heirTraits[i].GetComponent<Image>().sprite = civ.heir.traits[i].icon;
                heirTraits[i].GetComponent<HoverText>().text = civ.heir.traits[i].GetHoverText(civ);
            }
            else
            {
                heirTraits[i].GetComponent<Image>().sprite = UIManager.main.icons[2];
                heirTraits[i].GetComponent<HoverText>().text = "";
            }
        }
        if (civ.ruler.active)
        {
            rulerName.text = civ.ruler.Name;
            rulerAge.text = "Age: " + civ.ruler.age.ToString(true);
            rulerSkills.text = civ.ruler.adminSkill + "<sprite index=1> " + civ.ruler.diploSkill + "<sprite index=2> " + civ.ruler.milSkill + "<sprite index=3>";
            admin.text = "" + (3 + civ.ruler.adminSkill + (civ.advisorA.active ? civ.advisorA.skillLevel : 0) + (civ.focus > -1 ? (civ.focus == 0 ? 2 : -1) : 0));
            diplo.text = "" + (3 + civ.ruler.diploSkill + (civ.advisorD.active ? civ.advisorD.skillLevel : 0) + (civ.focus > -1 ? (civ.focus == 1 ? 2 : -1) : 0));
            mil.text = "" + (3 + civ.ruler.milSkill + (civ.advisorM.active ? civ.advisorM.skillLevel : 0) + (civ.focus > -1 ? (civ.focus == 2 ? 2 : -1) : 0));
            
        }
        else
        {
            rulerName.text = "No Ruler";
        }
        if (civ.heir.active)
        {
            heirName.text = civ.heir.Name;
            heirAge.text = "Age: " + civ.heir.age.ToString(true);
            heirSkills.text = civ.heir.adminSkill + "<sprite index=1> " + civ.heir.diploSkill + "<sprite index=2> " + civ.heir.milSkill + "<sprite index=3>";
        }
        else
        {
            heirName.text = "No Heir";
        }
        if (civ.advisorA.active)
        {
            advisorA.text = civ.advisorA.Name + " " + civ.advisorA.age.ToString(true,true) + civ.advisorA.skillLevel + "<sprite index=1> "  + civ.advisorA.effect + " " + Modifier.ToString(civ.advisorA.effectStrength, civ.GetStat(civ.advisorA.effect)) ;
            advisorAB.GetComponentsInChildren<Image>()[0].sprite = civ.advisorA.icon;
            string hoverText = "Promote this Advisor for: " + Mathf.Round(civ.advisorA.HireCost(civ) * 1000f)/100f + "<sprite index=0>\n";
            hoverText += "Salary Will Increase To: " + Mathf.Round(civ.advisorA.Salary(civ, 1) * 100f) / 100f + "<sprite index=0>\n";
            advisorAUpgrade.GetComponent<HoverText>().text = hoverText;
            salaryA.text ="Salary: " + Mathf.Round(civ.advisorA.Salary(civ, 0) * 100f) / 100f + "<sprite index=0>";
        }
        else
        {
            advisorA.text = "No <sprite index=1> Advisor";
            advisorAB.GetComponentsInChildren<Image>()[0].sprite = blank;
            advisorAUpgrade.GetComponent<HoverText>().text = "No Advisor";
            salaryA.text = "";
        }
        if (civ.advisorD.active)
        {
            advisorD.text = civ.advisorD.Name + " " + civ.advisorD.age.ToString(true, true) + civ.advisorD.skillLevel + "<sprite index=2> "  + civ.advisorD.effect + " " + Modifier.ToString(civ.advisorD.effectStrength, civ.GetStat(civ.advisorD.effect));
            advisorDB.GetComponentsInChildren<Image>()[0].sprite = civ.advisorD.icon;
            string hoverText = "Promote this Advisor for: " + Mathf.Round(civ.advisorD.HireCost(civ) * 1000f) / 100f + "<sprite index=0>\n";
            hoverText += "Salary Will Increase To: " + Mathf.Round(civ.advisorD.Salary(civ, 1) * 100f) / 100f + "<sprite index=0>\n";
            advisorDUpgrade.GetComponent<HoverText>().text = hoverText;
            salaryD.text = "Salary: " + Mathf.Round(civ.advisorD.Salary(civ, 0) * 100f) / 100f + "<sprite index=0>";
        }
        else
        {
            advisorD.text = "No <sprite index=2> Advisor";
            advisorDB.GetComponentsInChildren<Image>()[0].sprite = blank;
            advisorDUpgrade.GetComponent<HoverText>().text = "No Advisor";
            salaryD.text = "";
        }
        if (civ.advisorM.active)
        {
            advisorM.text = civ.advisorM.Name + " " + civ.advisorM.age.ToString(true, true) + civ.advisorM.skillLevel + "<sprite index=3> " + civ.advisorM.effect + " " + Modifier.ToString(civ.advisorM.effectStrength, civ.GetStat(civ.advisorM.effect));
            advisorMB.GetComponentsInChildren<Image>()[0].sprite = civ.advisorM.icon;
            string hoverText = "Promote this Advisor for: " + Mathf.Round(civ.advisorM.HireCost(civ) * 1000f) / 100f + "<sprite index=0>\n";
            hoverText += "Salary Will Increase To: " + Mathf.Round(civ.advisorM.Salary(civ, 1) * 100f) / 100f + "<sprite index=0>\n";
            advisorMUpgrade.GetComponent<HoverText>().text = hoverText;
            salaryM.text = "Salary: " + Mathf.Round(civ.advisorM.Salary(civ, 0) * 100f) / 100f + "<sprite index=0>";
        }
        else
        {
            advisorM.text = "No <sprite index=3> Advisor";
            advisorMB.GetComponentsInChildren<Image>()[0].sprite = blank;
            advisorMUpgrade.GetComponent<HoverText>().text = "No Advisor";
            salaryM.text = "";
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
                advisorList[i].GetComponentsInChildren<TextMeshProUGUI>()[0].text = advisor.age.ToString() + " "+ type+" " + advisor.skillLevel + " " + advisor.effect + " " + Modifier.ToString(advisor.effectStrength, civ.GetStat(advisor.effect));
                advisorList[i].GetComponentsInChildren<TextMeshProUGUI>()[1].text = "Hire Cost: " +Mathf.Round(advisor.HireCost(civ)*100f)/100f + "<sprite index=0>. Salary: " + Mathf.Round(advisor.Salary(civ) * 100f) / 100f + "<sprite index=0> per day.";
                advisorList[i].GetComponentsInChildren<Image>()[2].sprite = advisor.active ?advisor.icon : blank;
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
            courtPanel.SetActive(true);
        }
    }
}
