using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsUI : MonoBehaviour
{
    [SerializeField] GameObject notifPrefab;
    List<Notification> notifications = new List<Notification>();
    List<GameObject> notifs = new List<GameObject>();
    public static NotificationsUI main;
    public Notification missingAdvisor;
    public Notification noHeir;
    public Notification nonCoreProvinces;
    public Notification canTakeTech;
    public Notification canTakeIdea;
    public Notification rebelFaction;
    public Notification canTakeReform;
    public Notification loans;
    public Notification lowStability;
    public Notification bankruptcy;
    public Notification greatProj;
    public Notification fullMonarchPoints;
    private void Awake()
    {
        main = this;      
    }
    public void ClearNotifications()
    {
        notifications.Clear();
    }
    public static void AddNotification(Notification notification)
    {
        if(main.notifications.Exists(i=>i.name == notification.name))
        { return; }
        main.notifications.Add(notification);
    }
    private void OnGUI()
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation myCiv = Player.myPlayer.myCiv;
        while (notifs.Count != notifications.Count)
        {
            if (notifs.Count > notifications.Count)
            {
                int lastIndex = notifs.Count - 1;
                Destroy(notifs[lastIndex]);
                UIManager.main.UI.Remove(notifs[lastIndex]);
                notifs.RemoveAt(lastIndex);
            }
            else
            {
                int lastIndex = notifs.Count;
                GameObject item = Instantiate(notifPrefab, transform);
                item.GetComponent<Button>().onClick.AddListener(delegate { Click(lastIndex); });
                notifs.Add(item);
                UIManager.main.UI.Add(item);
            }
        }
        for (int i = 0; i < notifications.Count; i++)
        {
            Notification notification = notifications[i];
            SetupNotif(notifs[i], notification);
        }
    }
    void SetupNotif(GameObject obj, Notification notification)
    {
        if (Player.myPlayer.myCivID == -1) { return; }
        Civilisation myCiv = Player.myPlayer.myCiv;
        obj.GetComponentsInChildren<Image>()[1].sprite = notification.icon;
        obj.GetComponent<HoverText>().text = notification.GetHoverText(myCiv);
    }
    void Click(int index)
    {
        notifications[index].OnClick();
        notifications.RemoveAt(index);
        UIManager.main.mouseText.SetActive(false);
    }

}
