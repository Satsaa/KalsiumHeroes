using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkedHunterPassive), menuName = "KalsiumHeroes/Passive/" + nameof(MarkedHunterPassive))]
public class MarkedHunterPassive : Passive, IOnDealDamage_Unit {

	public MarkOfPreyStatus markOfPrey;


	public void OnDealDamage(UnitModifier source, Unit target, float damage, DamageType damageType) {
		if (source is not HuntTheMarkAbility && target.health.current > 0 && !target.modifiers.Get<MarkOfPreyStatus>().Any()) {
			Create(target, markOfPrey);
		}
	}
}
