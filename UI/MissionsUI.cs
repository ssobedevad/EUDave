using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Networking.Transport.Error;
using UnityEngine;
using UnityEngine.UI;

public class MissionsUI : MonoBehaviour
{
    [SerializeField] RectTransform target;
    [SerializeField] GameObject missionPrefab;
    List<GameObject> missionList = new List<GameObject>();

    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        while (missionList.Count != civ.missions.Length)
        {
            if (missionList.Count > civ.missions.Length)
            {
                int lastIndex = missionList.Count - 1;
                Destroy(missionList[lastIndex]);
                missionList.RemoveAt(lastIndex);
            }
            else
            {
                GameObject item = Instantiate(missionPrefab, target);
                Button button = item.GetComponentInChildren<Button>();
                int id = missionList.Count;
                button.onClick.AddListener(delegate { MissionClick(id); });
                missionList.Add(item);
            }
        }
        for (int i = 0; i < civ.missions.Length;i++)
        {
            Mission mission = civ.missions[i];
            GameObject missionObject = missionList[i];
            missionObject.GetComponentInChildren<TextMeshProUGUI>().text = mission.name;
            missionObject.GetComponentInChildren<HoverText>().text = mission.GetHoverText(civ);
            var images = missionObject.GetComponentsInChildren<Image>();
            images[2].sprite = mission.icon;
            images[1].color = mission.CanTake(civ) ? Color.green : Color.red;
            images[4].color = civ.missionProgress[i] ? Color.green : Color.red;
            images[4].sprite = civ.missionProgress[i] ? UIManager.main.icons[0] : UIManager.main.icons[1];
        }
    }
    void MissionClick(int id)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation civ = Player.myPlayer.myCiv;
        if (civ.missionProgress[id]) { return; }
        Mission mission = civ.missions[id];
        if (mission.CanTake(civ))
        {
            TakeMission(mission, civ);
            civ.missionProgress[id] = true;
        }
    }
    public static void TakeMission(Mission mission,Civilisation civ)
    {
        for (int i = 0; i < mission.effects.Length; i++)
        {
            civ.ApplyCivModifier(mission.effects[i].name, mission.effects[i].amount, mission.name, mission.effects[i].type, mission.effects[i].duration);
        }
    }
}
