using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootGeneric : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.rooted.ConfigureAlterer(add, v => true);
	}

}
