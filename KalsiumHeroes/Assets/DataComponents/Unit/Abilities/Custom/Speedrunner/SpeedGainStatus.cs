using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGainStatus : Status, IOnGetEstimatedSpeed_Unit {

	public SpeedGainStatusData speedGainModifierData => (SpeedGainStatusData)data;
	public override Type dataType => typeof(SpeedGainStatusData);

	public int unitsFound;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		var oldMt = unit.unitData.movement.value;
		unit.unitData.movement.ConfigureAlterer(add, v => v + speedGainModifierData.movementGain.value * unitsFound);
		print($"Old Movement: {oldMt} New Movement: {unit.unitData.movement.value}");
		var oldSpd = unit.unitData.speed.value;
		unit.unitData.speed.ConfigureAlterer(add, v => v + speedGainModifierData.speedGain.value * unitsFound);
		print($"Old Speed: {oldSpd} New Speed: {unit.unitData.speed.value}");
	}

	public virtual int OnGetEstimatedSpeed(int roundsAhead) {
		if (!TurnDurationWouldHaveExpired(roundsAhead)) return speedGainModifierData.speedGain.value * unitsFound;
		return 0;
	}
}
