using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Slider aiAggressiveness;
    [SerializeField] TextMeshProUGUI aiAggText;
    [SerializeField] Toggle replaceSave;

    private void Awake()
    {
        aiAggressiveness.onValueChanged.AddListener(UpdateAiAggro);
        replaceSave.onValueChanged.AddListener(UpdateReplaceSave);

    }
    private void Start()
    {
        aiAggressiveness.value = Game.main.AI_MAX_AGGRESSIVENESS;
        replaceSave.isOn = Game.main.replaceSave;
        UpdateAiAggro(Game.main.AI_MAX_AGGRESSIVENESS);
        UpdateReplaceSave(Game.main.replaceSave);
    }
    void UpdateAiAggro(float val)
    {
        Game.main.AI_MAX_AGGRESSIVENESS = val;
        foreach(var civ in Game.main.civs)
        {
            civ.SetAILevels();
        }
        aiAggText.text = "Max AI Aggressiveness: " + Mathf.Round(val);
        PlayerPrefs.SetFloat("AIAGGRO",val);
    }
    void UpdateReplaceSave(bool val)
    {
        Game.main.replaceSave = val;
        PlayerPrefs.SetInt("ReplaceSave", val? 0 : 1);
    }
}
