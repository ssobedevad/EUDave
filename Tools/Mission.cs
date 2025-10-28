using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Mission
{
    public string name;
    public Sprite icon;
    public Effect[] effects;
    public Condition[] conditions;

}
