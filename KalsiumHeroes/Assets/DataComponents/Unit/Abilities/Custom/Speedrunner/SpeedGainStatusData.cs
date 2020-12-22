using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainStatusData), menuName = "DataSources/Units/Speedrunner/" + nameof(SpeedGainStatusData))]
public class SpeedGainStatusData : StatusData {

	public override Type componentConstraint => typeof(SpeedGainStatus);

	[Header("Speed Gain Status Data")]

	public Attribute<int> speedGain;

	public Attribute<int> movementGain;
}
