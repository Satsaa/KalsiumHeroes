using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGainStatus : Status, IOnGetEstimatedSpeed_Unit {

	public new SpeedGainStatusData data => (SpeedGainStatusData)_data;
	public override Type dataType => typeof(SpeedGainStatusData);

	public int unitsFound;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		var oldMt = unit.data.movement.value;
		unit.data.movement.ConfigureAlterer(add, v => v + data.movementGain.value * unitsFound);
		Debug.Log($"Old Movement: {oldMt} New Movement: {unit.data.movement.value}");
		var oldSpd = unit.data.speed.value;
		unit.data.speed.ConfigureAlterer(add, v => v + data.speedGain.value * unitsFound);
		Debug.Log($"Old Speed: {oldSpd} New Speed: {unit.data.speed.value}");
	}

	public virtual void OnGetEstimatedSpeed(int roundsAhead, ref int current) {
		if (!TurnDurationWouldHaveExpired(roundsAhead)) current += data.speedGain.value * unitsFound;
	}
}
