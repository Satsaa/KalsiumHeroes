using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MovementChangeStatus), menuName = "KalsiumHeroes/Status/" + nameof(MovementChangeStatus))]
public class MovementChangeStatus : Status {

	public Attribute<int> movementChange;


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.movement.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => movementChange.current,
			updateEvents: new[] { movementChange.current.onChanged }
		);
	}
}
