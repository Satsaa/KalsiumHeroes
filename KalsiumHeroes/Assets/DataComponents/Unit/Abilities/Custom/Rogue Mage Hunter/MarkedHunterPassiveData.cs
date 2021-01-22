using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkedHunterPassiveData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(MarkedHunterPassiveData))]
public class MarkedHunterPassiveData : PassiveData
{
    public override Type componentConstraint => typeof(MarkedHunterPassive);

    [Header("Marked Hunter Passive Attributes")]
    public DataComponentData markOfPreyModifier;
}
