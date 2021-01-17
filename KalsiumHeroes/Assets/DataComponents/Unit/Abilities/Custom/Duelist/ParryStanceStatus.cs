using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status, IOnAbilityCastStart_Global, IOnDamage_Unit, IOnTurnStart_Unit {

	public new ParryStanceStatusData data => (ParryStanceStatusData)base.data;
	public override Type dataType => typeof(ParryStanceStatusData);

	[SerializeField, HideInInspector] Ability lastAbility;


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.ConfigureAlterer(add, v => v + data.defenseIncrease.value);
	}

	public void OnAbilityCastStart(Ability ability) {
		if (ability.unit == unit) lastAbility = null;
		else lastAbility = ability;
	}

	public void OnDamage(ref float damage, ref DamageType type) {
		if (lastAbility != null && lastAbility.data.range.value <= 1 && lastAbility.data.abilityType == AbilityType.WeaponSkill) {
			lastAbility.unit.DealStatusDamage(data.damage.value, this, data.damageType);
			Destroy(this);
		}
		return;
	}

	public void OnTurnStart() {
		Destroy(this);
	}
}
