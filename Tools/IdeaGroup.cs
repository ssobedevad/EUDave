using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class IdeaGroup
{
    public string name;
    public int type;
    public Idea[] ideas;
    public IdeaGroup()
    {
        name = "";
        type = 0;
        ideas = new Idea[7];
    }
}
