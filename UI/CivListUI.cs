using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CivListUI : MonoBehaviour
{
    [SerializeField] GameObject civListItemPrefab;
    [SerializeField] Transform civListBack;
    List<GameObject> civList = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
        List<Civilisation> civs = Game.main.civs.ToList();
        civs.RemoveAll(i => !i.isActive());
        civs.Sort((x,y) => y.GetTotalDev().CompareTo(x.GetTotalDev()));
        while (civList.Count != civs.Count)
        {
            if (civList.Count > civs.Count)
            {
                int lastIndex = civList.Count - 1;
                Destroy(civList[lastIndex]);
                civList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(civListItemPrefab, civListBack);                
                civList.Add(item);
            }
        }
        for (int i = 0; i < civs.Count; i++)
        {
            Civilisation civ = civs[i];
            Image[] images = civList[i].GetComponentsInChildren<Image>();
            TextMeshProUGUI[] texts = civList[i].GetComponentsInChildren<TextMeshProUGUI>();
            images[1].color = civ.c;
            texts[0].text = civ.civName;
            texts[1].text = civ.GetTotalDev() + "";
            texts[2].text = Mathf.Round(civ.GetTotalIncome() * 100f)/100f + "<sprite index=0>";
            if(Player.myPlayer.myCivID > -1)
            {
                Civilisation myCiv = Player.myPlayer.myCiv;
                if(civ.CivID == Player.myPlayer.myCivID)
                {
                    images[0].color = Color.green;
                }
                else if (myCiv.rivals.Contains(civ.CivID))
                {
                    images[0].color = Color.red;
                }
                else if (myCiv.allies.Contains(civ.CivID))
                {
                    images[0].color = Color.blue;
                }
                else if (myCiv.subjects.Contains(civ.CivID))
                {
                    images[0].color = Color.magenta;
                }
                else
                {
                    images[0].color = Color.white;
                }
            }
            else
            {
                images[0].color = Color.white;
            }
        }
    }
}
