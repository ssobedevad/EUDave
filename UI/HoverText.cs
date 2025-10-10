using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverText : MonoBehaviour,IPointerExitHandler
{
    public string text;

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.main.mouseText.SetActive(false);
    }

    private void OnGUI()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (UIManager.IsMouseOverUI(rectTransform))
        {
            RectTransform rect = UIManager.main.mouseText.GetComponent<RectTransform>();
            var MainCanvas = UIManager.main.gameObject;
            var canvasS = MainCanvas.GetComponent<CanvasScaler>();

            var halfWidth = canvasS.referenceResolution.x;
            var halfHeight = canvasS.referenceResolution.y;

            Vector2 apos = Input.mousePosition;
            float xpos = apos.x;
            xpos = Mathf.Clamp(xpos + 20, 0, halfWidth - rect.sizeDelta.x);
            apos.x = xpos;
            float ypos = apos.y;
            ypos = Mathf.Clamp(ypos, 0 + (rect.sizeDelta.x / 2), halfHeight - (rect.sizeDelta.x / 2));
            apos.y = ypos;
            rect.anchoredPosition = apos;
            UIManager.main.mouseText.SetActive(true);
            UIManager.main.mouseText.GetComponentInChildren<TextMeshProUGUI>().text = text;
        }
    }
}
