using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkedHunterPassive : Passive, IOnDealDamage_Unit {

	new public MarkedHunterPassiveData data => (MarkedHunterPassiveData)_data;
	public override Type dataType => typeof(MarkedHunterPassiveData);

	public void OnDealDamage(UnitModifier source, Unit target, float damage, DamageType damageType) {
		if (source is not HuntTheMarkAbility && target.data.health.current > 0 && !target.modifiers.Get<MarkOfPreyStatus>().Any()) {
			Create(target, data.markOfPreyModifier);
		}
	}
}
