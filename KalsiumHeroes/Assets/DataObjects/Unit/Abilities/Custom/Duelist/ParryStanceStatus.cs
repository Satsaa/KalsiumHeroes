using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status, IOnTakeDamage_Unit, IOnTurnStart_Unit {

	new public ParryStanceStatusData data => (ParryStanceStatusData)_data;
	public override Type dataType => typeof(ParryStanceStatusData);

	protected Alterer<int> defenseAlterer;
	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		var alt = unit.data.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseIncrease.current,
			updateEvents: new[] {
				data.defenseIncrease.current.onChanged
			}
		);
	}


	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.data.abilityType.current == AbilityType.WeaponSkill && Game.grid.Distance(ability.unit.tile, this.unit.tile) <= 1) {
				DealDamage(ability.unit, data.damage.current, data.damageType);
				Remove();
			}
		}
	}
}
