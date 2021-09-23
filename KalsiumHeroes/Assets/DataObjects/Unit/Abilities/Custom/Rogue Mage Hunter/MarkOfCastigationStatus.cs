using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkOfCastigationStatus : Status, IOnAbilityCastEnd_Unit {

	new public MarkOfCastigationStatusData data => (MarkOfCastigationStatusData)_data;
	public override Type dataType => typeof(MarkOfCastigationStatusData);

	public void OnAbilityCastEnd(Ability ability) {
		if (ability.data.abilityType.current == AbilityType.Spell) {
			Modifier.Create(ability.unit, data.silenceModifier);
			Modifier.Create(ability.unit, data.markOfPreyModifier);
			ability.unit.DealCalculatedDamage(this, data.damage.current, data.damageType);
			Remove();
		}
	}
}
