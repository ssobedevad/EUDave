using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class GovernmentReformTier
{
    public string name;
    public TierType type;
    public GovernmentReform[] Reforms;
}
public enum TierType
{
    Custom,
    DefaultTier1,
    DefaultTier2,
    DefaultTier3,
    DefaultTier4,
}
