using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotTestStatus : StatusEffect {

	public DotTestStatusData dotTestData => (DotTestStatusData)data;
	public override Type dataType => typeof(DotTestStatusData);

	public override void OnTurnEnd() {
		unit.Damage(dotTestData.damage.value, dotTestData.damageType);
		base.OnTurnEnd();
	}
}
