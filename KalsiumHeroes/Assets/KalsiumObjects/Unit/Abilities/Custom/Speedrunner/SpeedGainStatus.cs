using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainStatus), menuName = "KalsiumHeroes/Status/" + nameof(SpeedGainStatus))]
public class SpeedGainStatus : Status, IOnGetEstimatedSpeed_Unit {

	public Attribute<int> speedGain;

	public Attribute<int> movementGain;


	protected int unitsFound = -1;

	public void Init(int unitsFound) {
		if (this.unitsFound != -1) throw new InvalidOperationException("Yall can't call Init twice.");
		this.unitsFound = unitsFound;
	}


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.movement.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => movementGain.current * unitsFound,
			updateEvents: new[] { movementGain.current.onChanged }
		);
		unit.speed.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => speedGain.current * unitsFound,
			updateEvents: new[] { speedGain.current.onChanged }
		);
	}

	public virtual void OnGetEstimatedSpeed(int roundsAhead, ref int current) {
		if (!WouldHaveExpired(roundsAhead)) current += speedGain.current * unitsFound;
	}
}
