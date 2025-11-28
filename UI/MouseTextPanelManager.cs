using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseTextPanelManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    void Start()
    {
        m_Raycaster = UIManager.main.GetComponent<GraphicRaycaster>();
        m_EventSystem = UIManager.main.GetComponent<EventSystem>();
    }
    private void OnGUI()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(600, text.textBounds.size.y + 40);
        if (!CheckMouseHover())
        {
            gameObject.SetActive(false);
        }     
    }
    bool CheckMouseHover()
    {
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        m_Raycaster.Raycast(m_PointerEventData, results);

        return results.Count > 0 && results[0].gameObject.GetComponent<HoverText>() != null;
    }
}
