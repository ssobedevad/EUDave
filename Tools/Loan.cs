using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Loan
{
    public float value;

    public Loan(float Value) 
    {
        value = Value;
    }
    public float GetInterestValue(Civilisation civilisation)
    {
        return value * civilisation.interestPerMonth.value/ 30f;
    }
}
