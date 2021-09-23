using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementChangeStatus : Status {

	new public MovementChangeStatusData data => (MovementChangeStatusData)_data;
	public override Type dataType => typeof(MovementChangeStatusData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.movement.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.movementChange.current,
			updateEvents: new[] { data.movementChange.current.onChanged }
		);
	}
}
