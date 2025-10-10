using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class IdeaGroupData
{
    public int id;
    public int type;
    public int unlockedLevel;
    public bool active;

    public IdeaGroupData(int id, int type, int unlockedLevel)
    {
        this.id = id;
        this.type = type;
        this.unlockedLevel = unlockedLevel;
        active = true;
    }
}
