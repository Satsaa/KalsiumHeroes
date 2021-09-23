using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotStatus : Status, IOnTurnEnd_Unit {

	new public DotStatusData data => (DotStatusData)_data;
	public override Type dataType => typeof(DotStatusData);

	public override void OnTurnEnd() {
		DealDamage(unit, data.damage.current, data.damageType);
		base.OnTurnEnd();
	}
}
