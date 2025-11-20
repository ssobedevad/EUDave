using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileText : MonoBehaviour
{
    TextMeshProUGUI text;
    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Color c = Color.clear;
        text.color = c;
        CameraController.main.camChange.AddListener(CamChange);       
    }
    void CamChange()
    {
        Color c = CameraController.main.cam.orthographicSize <= 8 ? Color.white: Color.clear;
        if (!Game.main.Equal(c, text.color))
        {
            text.color = c;
        }
    }
}
