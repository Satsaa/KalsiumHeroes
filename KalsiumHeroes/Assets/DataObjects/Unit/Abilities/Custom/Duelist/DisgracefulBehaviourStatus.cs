using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgracefulBehaviourStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.silenced.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
		unit.disarmed.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}

}
