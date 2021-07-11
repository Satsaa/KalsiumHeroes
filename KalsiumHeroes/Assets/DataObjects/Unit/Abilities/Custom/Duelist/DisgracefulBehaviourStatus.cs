using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgracefulBehaviourStatus : Status {
	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.silenced.value.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
		unit.data.disarmed.value.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}
}
