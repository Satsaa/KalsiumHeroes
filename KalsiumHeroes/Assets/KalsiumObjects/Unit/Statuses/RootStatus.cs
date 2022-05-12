using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RootStatus), menuName = "KalsiumHeroes/Status/" + nameof(RootStatus))]
public class RootStatus : Status {

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.rooted.current.ConfigureAlterer(add, this,
			applier: (v, a) => a,
			updater: () => true
		);
	}

}
