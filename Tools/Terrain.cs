using System;
using UnityEngine;

[Serializable]
public class Terrain
{
    public Color c;
    public string name;
    public float devCostMod;
    public float moveSpeedMod;
    public int supplyLimitBonus;
    public bool isSea;
    public int attackerDiceRoll;
    public Sprite sprite;

    public string GetHoverText()
    {
        string text = name + "\n\n";
        text += "Development Cost: " + (devCostMod > 0f ? "+" : "" )+ Mathf.Round(devCostMod * 100f) + "%\n";
        if (moveSpeedMod != 0)
        {
            text += "Movement Speed: " + (moveSpeedMod > 0f ? "+" : "") + Mathf.Round(moveSpeedMod * 100f) + "%\n";
        }
        if (attackerDiceRoll != 0)
        {
            text += "Dice Roll for Attacker: " + (attackerDiceRoll > 0f ? "+" : "") + attackerDiceRoll + "\n";
        }
        return text;
    }
}
