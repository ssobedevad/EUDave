using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    public string display;
    public string currentPhase;
    private void OnGUI()
    {
        int numDots = (int)Mathf.Round(Time.time) % 6;
        string displayText = display;
        if (currentPhase.Length > 0)
        {
            displayText += " (" + currentPhase + ")";
        }
        text.text = displayText + new String('.',numDots);
    }
}
