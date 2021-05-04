using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseChangeStatus : Status {

	public new DefenseChangeStatusData data => (DefenseChangeStatusData)data;
	public override Type dataType => typeof(DefenseChangeStatusData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseReduction.value,
			updateEvents: new[] { data.defenseReduction.onValueChanged }
		);
	}
}
