using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgracefulBehaviourStatus : Status {
	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.silenced.ConfigureAlterer(add, v => true);
		unit.data.disarmed.ConfigureAlterer(add, v => true);
	}
}
