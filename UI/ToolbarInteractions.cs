using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolbarInteractions : MonoBehaviour
{
    [SerializeField] Button menu, civList, notifs;
    [SerializeField]
    Image[] images;
    [SerializeField] GameObject civListObj,notifsBack;

    private void Start()
    {
        menu.onClick.AddListener(MainMenu);
        civList.onClick.AddListener(CivListToggle);
        notifs.onClick.AddListener(Notifs);
    }
    private void Update()
    {
        images[0].color = Color.white;
        images[1].color = civListObj.activeSelf ? Color.white : Color.gray;
        images[2].color = notifsBack.activeSelf ? Color.white : Color.gray;
    }

    void MainMenu()
    {

    }

    void CivListToggle()
    {
        civListObj.SetActive(!civListObj.activeSelf);
    }

    void Notifs()
    {
        notifsBack.SetActive(!notifsBack.activeSelf);
    }
}
