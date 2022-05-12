using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DisarmStatus), menuName = "KalsiumHeroes/Status/" + nameof(DisarmStatus))]
public class DisarmStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.disarmed.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}

}
