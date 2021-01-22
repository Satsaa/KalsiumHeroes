using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MovementChangeStatusData), menuName = "DataSources/Statuses/" + nameof(MovementChangeStatusData))]
public class MovementChangeStatusData : StatusData {
	public override Type createTypeConstraint => typeof(MovementChangeStatus);

	public Attribute<int> movementChange;
}
