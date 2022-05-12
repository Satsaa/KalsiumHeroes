using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DefenseChangeStatus), menuName = "KalsiumHeroes/Status/" + nameof(DefenseChangeStatus))]
public class DefenseChangeStatus : Status {

	public Attribute<int> defenseChange;

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => defenseChange.current,
			updateEvents: new[] { defenseChange.current.onChanged }
		);
	}
}
