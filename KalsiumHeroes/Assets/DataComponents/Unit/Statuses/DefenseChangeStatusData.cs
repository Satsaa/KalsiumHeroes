using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DefenseChangeStatusData), menuName = "DataSources/Statuses/" + nameof(DefenseChangeStatusData))]
public class DefenseChangeStatusData : StatusData
{
    public override Type componentConstraint => typeof(DefenseChangeStatus);

    [Header("Defense Change Status Data")]
    public Attribute<int> defenseReduction;

}
