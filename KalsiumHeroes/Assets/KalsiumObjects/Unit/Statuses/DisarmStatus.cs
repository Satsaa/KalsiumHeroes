using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisarmStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.disarmed.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}

}
