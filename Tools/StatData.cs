using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class StatData
{    
    public string name;
    public Stat s;


    public StatData()
    {

    }
    public StatData(string n, Stat s)
    {
        this.s = s;
        this.name = n;
    }
}
