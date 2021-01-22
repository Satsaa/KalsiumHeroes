using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkOfCastigationStatusData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(MarkOfCastigationStatusData))]
public class MarkOfCastigationStatusData : StatusData {
    
    public override Type componentConstraint => typeof(MarkOfCastigationStatus);

    [Header("Mark of Castigation Status Attributes")]
    public Attribute<float> damage;
    public DamageType damageType;
    public DataComponentData silenceModifier;
}
