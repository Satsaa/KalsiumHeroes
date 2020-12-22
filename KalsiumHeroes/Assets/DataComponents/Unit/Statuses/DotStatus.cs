using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStatus : Status {

	public DotStatusData dotTestData => (DotStatusData)data;
	public override Type dataType => typeof(DotStatusData);

	public override void OnTurnEnd() {
		unit.Damage(dotTestData.damage.value, dotTestData.damageType);
		base.OnTurnEnd();
	}
}
