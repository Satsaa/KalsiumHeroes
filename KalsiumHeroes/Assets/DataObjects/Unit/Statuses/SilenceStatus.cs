using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.silenced.ConfigureAlterer(add, v => true);
	}

}