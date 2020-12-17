using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceGeneric : StatusEffect {
	public SilenceGenericData silenceGenericData => (SilenceGenericData)data;

	public override Type dataType => typeof(SilenceGenericData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.silenced.ConfigureAlterer(add, v => true);
	}

}