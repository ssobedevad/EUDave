using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseTextPanelManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;

    private void OnGUI()
    {
        if (!Player.myPlayer.isHoveringUI)
        {
            gameObject.SetActive(false);
        }
        else
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(600, text.textBounds.size.y + 40);
        }       
    }
}
