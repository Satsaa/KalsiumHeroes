using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShrapnelAbility : Ability {

	public ShrapnelAbilityData shrapnelAbilityData => (ShrapnelAbilityData)data;
	public override Type dataType => typeof(ShrapnelAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.targets.First()];
			var modifier = ability.unit.gameObject.AddDataComponent<ShrapnelAbilityModifier>(shrapnelAbilityData.shrapnelModifierData);

			modifier.calculatedDamage = GetCalculatedDamage(shrapnelAbilityData.damage.value, shrapnelAbilityData.damageType);
			modifier.damageType = shrapnelAbilityData.damageType;
			modifier.target = target;
			modifier.aoe = base.GetAffectedArea(target).ToList();
		});
	}

}
