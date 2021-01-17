using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStatus : Status, IOnTurnEnd_Unit {

	public new DotStatusData data => (DotStatusData)base.data;
	public override Type dataType => typeof(DotStatusData);

	public override void OnTurnEnd() {
		unit.DealStatusDamage(data.damage.value, this, data.damageType);
		base.OnTurnEnd();
	}
}
