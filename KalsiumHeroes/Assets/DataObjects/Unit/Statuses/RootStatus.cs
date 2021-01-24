using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.rooted.ConfigureAlterer(add, v => true);
	}

}
