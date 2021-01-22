using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DefenseChangeStatusData), menuName = "DataSources/Statuses/" + nameof(DefenseChangeStatusData))]
public class DefenseChangeStatusData : StatusData {

	public override Type ownerConstraint => typeof(DefenseChangeStatus);

	public Attribute<int> defenseReduction;

}
