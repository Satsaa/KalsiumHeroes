using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisarmStatus : Status {
	public DisarmStatusData disarmGenericData => (DisarmStatusData)data;

	public override Type dataType => typeof(DisarmStatusData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.disarmed.ConfigureAlterer(add, v => true);
	}

}
