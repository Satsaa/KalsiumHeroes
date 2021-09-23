using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGainStatus : Status, IOnGetEstimatedSpeed_Unit {

	new public SpeedGainStatusData data => (SpeedGainStatusData)_data;
	public override Type dataType => typeof(SpeedGainStatusData);

	protected int unitsFound = -1;

	public void Init(int unitsFound) {
		if (this.unitsFound != -1) throw new InvalidOperationException("Yall can't call Init twice.");
		this.unitsFound = unitsFound;
	}


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.movement.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.movementGain.current * unitsFound,
			updateEvents: new[] { data.movementGain.current.onChanged }
		);
		unit.data.speed.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.speedGain.current * unitsFound,
			updateEvents: new[] { data.speedGain.current.onChanged }
		);
	}

	public virtual void OnGetEstimatedSpeed(int roundsAhead, ref int current) {
		if (!WouldHaveExpired(roundsAhead)) current += data.speedGain.current * unitsFound;
	}
}
