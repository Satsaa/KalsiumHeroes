using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainModifierData), menuName = "DataSources/Units/Speedrunner/" + nameof(SpeedGainModifierData))]
public class SpeedGainModifierData : StatusData {

	public override Type componentTypeConstraint => typeof(SpeedGainStatus);

	[Header("Speed Gain Status Data")]

	public Attribute<int> speedGain;

	public Attribute<int> movementGain;
}
