using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SilenceStatus), menuName = "KalsiumHeroes/Status/" + nameof(SilenceStatus))]
public class SilenceStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.silenced.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}

}