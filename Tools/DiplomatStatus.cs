using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class DiplomatStatus
{
    public int Distance;
    public DiplomatAction Action;
    public int targetCivId;

    public DiplomatStatus()
    {

    }
    public DiplomatStatus(Civilisation target,Civilisation source, DiplomatAction action)
    {
        Distance = TileData.evenr_distance(target.capitalPos, source.capitalPos);
        Action = action;
        if(action != DiplomatAction.Travelling)
        {
            targetCivId = target.CivID;
        }
    }
    public DiplomatStatus(Civilisation target, Civilisation source)
    {
        Distance = TileData.evenr_distance(target.capitalPos, source.capitalPos);
        Action = DiplomatAction.Travelling;
    }
}
public enum DiplomatAction
{
    Idle,
    Travelling,
    Establishing,
    Spying,
}
