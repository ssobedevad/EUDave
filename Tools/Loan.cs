using MessagePack;
using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
[MessagePackObject(keyAsPropertyName: true)]
public class Loan
{
    public float value;

    public Loan(float Value) 
    {
        value = Value;
    }
    public float GetInterestValue(Civilisation civilisation)
    {
        return value * (civilisation.interestPerMonth.v / 3000f);
    }
}
