using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public class CivDataInit
{
    [SerializeField] public string Name;
    [SerializeField] public Color c;
    [SerializeField] public NationalIdeas ideas;
    [SerializeField] public Ruler ruler,heir;
    [SerializeField] public int techLevel;

}
