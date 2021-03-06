using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkedHunterPassive : Passive, IOnDealDamage_Unit {

	public new MarkedHunterPassiveData data => (MarkedHunterPassiveData)_data;
	public override Type dataType => typeof(MarkedHunterPassiveData);

	public void OnDealDamage(UnitModifier source, Unit targetUnit, float damage, DamageType damageType) {
		if (!(source is HuntTheMarkAbility)) {
			if (targetUnit.data.health.current > 0 && !targetUnit.modifiers.Get<MarkOfPreyStatus>().Any())
				Modifier.Create(targetUnit, data.markOfPreyModifier);
		}
	}
}
