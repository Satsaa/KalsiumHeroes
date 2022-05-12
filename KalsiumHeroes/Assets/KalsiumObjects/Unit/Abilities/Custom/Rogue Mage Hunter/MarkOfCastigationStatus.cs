using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkOfCastigationStatus), menuName = "KalsiumHeroes/Status/" + nameof(MarkOfCastigationStatus))]
public class MarkOfCastigationStatus : Status, IOnAbilityCastEnd_Unit {

	public Attribute<float> damage;
	public DamageType damageType;
	public UnitModifier silence;
	public MarkOfPreyStatus markOfPrey;

	public void OnAbilityCastEnd(Ability ability) {
		if (ability.abilityType.current == AbilityType.Spell) {
			Create(ability.unit, silence);
			Create(ability.unit, markOfPrey);
			ability.unit.DealCalculatedDamage(this, damage.current, damageType);
			Remove();
		}
	}
}
