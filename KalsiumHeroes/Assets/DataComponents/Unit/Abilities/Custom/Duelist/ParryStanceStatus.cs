using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status/*, IOnAbilityCastStart_Global*/, IOnTakeDamage_Unit, IOnTurnStart_Unit {

	public new ParryStanceStatusData data => (ParryStanceStatusData)base.data;
	public override Type dataType => typeof(ParryStanceStatusData);

	//[SerializeField, HideInInspector] Ability lastAbility;


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.data.defense.ConfigureAlterer(add, v => v + data.defenseIncrease.value);
	}

	//public void OnAbilityCastStart(Ability ability) {
	//	if (ability.unit == unit) lastAbility = null;
	//	else lastAbility = ability;
	//}

	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		//if (lastAbility != null && lastAbility.data.range.value <= 1 && lastAbility.data.abilityType == AbilityType.WeaponSkill) {
		//	DealDamage(lastAbility.unit, data.damage.value, data.damageType);
		//	this.Destroy();
		//}
        if (source is Ability ability) {
            if (ability.data.abilityType == AbilityType.WeaponSkill && Game.grid.Distance(ability.unit.tile, this.unit.tile) <= 1) {
                DealDamage(ability.unit, data.damage.value, data.damageType);
                this.Destroy();
            }
        }
	}

	public void OnTurnStart() {
		this.Destroy();
	}
}
