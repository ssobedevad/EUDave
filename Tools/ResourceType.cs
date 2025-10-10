using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ResourceType
{
    public string name;
    public float Value = 0f;
    public string localTileEffect;
    public float localTileEffectStrength;
    public Sprite sprite;
    public Color mapColor;
    public override string ToString()
    {
        return name;
    }

}
