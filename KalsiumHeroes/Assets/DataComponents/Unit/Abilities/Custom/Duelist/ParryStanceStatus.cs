using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status, IOnTakeDamage_Unit, IOnTurnStart_Unit {

	public new ParryStanceStatusData data => (ParryStanceStatusData)_data;
	public override Type dataType => typeof(ParryStanceStatusData);

	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.ConfigureAlterer(add, v => v + data.defenseIncrease.value);
	}


	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.data.abilityType == AbilityType.WeaponSkill && Game.grid.Distance(ability.unit.tile, this.unit.tile) <= 1) {
				DealDamage(ability.unit, data.damage.value, data.damageType);
				Remove();
			}
		}
	}

	public override void OnTurnStart() {
		base.OnTurnStart();
		Remove();
	}
}
