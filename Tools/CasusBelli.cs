using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class CasusBelli
{
    public string Name;
    public bool capital;
    public bool province;
    public bool superiority;
    public float warScoreCost;
    public float aeCost;
    public bool canTakeProvinces;

}
