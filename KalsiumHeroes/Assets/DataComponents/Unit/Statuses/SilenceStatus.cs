using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.silenced.ConfigureAlterer(add, v => true);
	}

}