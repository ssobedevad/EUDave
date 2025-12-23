using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class NationalIdeas
{
    public string name;
    public IdeaStyle style;
    public Effect[] traditions;
    public Idea[] ideas;
}
public enum IdeaStyle
{
    Custom,
    Default1,
    Default2,
    Default3,
}
