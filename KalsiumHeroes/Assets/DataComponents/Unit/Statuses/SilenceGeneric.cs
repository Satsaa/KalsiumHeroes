using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceGeneric : StatusEffect {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.silenced.ConfigureAlterer(add, v => true);
	}

}