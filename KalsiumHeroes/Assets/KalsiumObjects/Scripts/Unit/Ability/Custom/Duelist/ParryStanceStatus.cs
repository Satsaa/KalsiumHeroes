using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ParryStanceStatus), menuName = "KalsiumHeroes/Status/" + nameof(ParryStanceStatus))]
public class ParryStanceStatus : Status, IOnTakeDamage_Unit, IOnTurnStart_Unit {

	public Attribute<int> defenseChange;

	public Attribute<float> damage;
	public DamageType damageType;


	protected override void OnConfigureNonpersistent(bool add) {
		base.OnConfigureNonpersistent(add);
		unit.defense.current.ConfigureAlterer(add, this,
			applier: (v, a) => v + a,
			updater: () => defenseChange.current,
			updateEvents: new[] {
				defenseChange.current.onChanged
			}
		);
	}


	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.abilityType.current == AbilityType.WeaponSkill && Game.grid.Distance(ability.unit.tile, unit.tile) <= 1) {
				DealDamage(ability.unit, this.damage.current, damageType);
				Remove();
			}
		}
	}
}
