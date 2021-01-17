using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DefenseReductionStatusData), menuName = "DataSources/Statuses/" + nameof(DefenseReductionStatusData))]
public class DefenseReductionStatusData : StatusData
{
    public override Type componentConstraint => typeof(DefenseReductionStatus);

    [Header("Defense Reduction Status Data")]
    public Attribute<int> defenseReduction;

}
