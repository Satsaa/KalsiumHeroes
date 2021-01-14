using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStatus : Status, IOnTurnEnd_Unit {

	public DotStatusData dotStatusData => (DotStatusData)data;
	public override Type dataType => typeof(DotStatusData);

	public override void OnTurnEnd() {
		unit.DealStatusDamage(dotStatusData.damage.value, this, dotStatusData.damageType);
		base.OnTurnEnd();
	}
}
