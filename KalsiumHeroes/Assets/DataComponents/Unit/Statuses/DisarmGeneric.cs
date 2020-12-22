using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisarmGeneric : Status {
	public DisarmGenericData disarmGenericData => (DisarmGenericData)data;

	public override Type dataType => typeof(DisarmGenericData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.disarmed.ConfigureAlterer(add, v => true);
	}

}
