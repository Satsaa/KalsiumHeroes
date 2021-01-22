using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = nameof(DisgracefulBehaviourStatusData), menuName = "DataSources/Units/Duelist/" + nameof(DisgracefulBehaviourStatusData))]
public class DisgracefulBehaviourStatusData : StatusData {
	public override Type createTypeConstraint => typeof(DisgracefulBehaviourStatus);
}
