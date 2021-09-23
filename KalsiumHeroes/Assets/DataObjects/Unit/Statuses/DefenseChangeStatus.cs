using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseChangeStatus : Status {

	new public DefenseChangeStatusData data => (DefenseChangeStatusData)_data;
	public override Type dataType => typeof(DefenseChangeStatusData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseReduction.current,
			updateEvents: new[] { data.defenseReduction.current.onChanged }
		);
	}
}
