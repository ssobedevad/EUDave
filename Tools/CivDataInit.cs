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
    [SerializeField] public int techLevel,startReform;
    [SerializeField] public int religion,government;
    [SerializeField] public int overlordID = -1;
    [SerializeField] public int subjectType = -1;
    [SerializeField] public MissionStyle missionStyle;
    [SerializeField] public Mission[] missions;
}
public enum MissionStyle
{
    Default,
    Custom,
}
