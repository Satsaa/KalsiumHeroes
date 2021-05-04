using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status, IOnTakeDamage_Unit, IOnTurnStart_Unit {

	public new ParryStanceStatusData data => (ParryStanceStatusData)_data;
	public override Type dataType => typeof(ParryStanceStatusData);

	protected Alterer<int> defenseAlterer;
	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		var alt = unit.data.defense.ConfigureValueAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => data.defenseIncrease.value,
			updateEvents: new[] {
				data.defenseIncrease.onValueChanged
			}
		);
	}


	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.data.abilityType == AbilityType.WeaponSkill && Game.grid.Distance(ability.unit.tile, this.unit.tile) <= 1) {
				DealDamage(ability.unit, data.damage.value, data.damageType);
				Remove();
			}
		}
	}
}
