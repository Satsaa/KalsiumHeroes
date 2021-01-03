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
			var aoe = GetAffectedArea(target);

			var unit = ability.unit;

			var modifier = ability.unit.gameObject.AddDataComponent<ShrapnelAbilityModifier>(shrapnelAbilityData.shrapnelModifierData);
			modifier.casterData = ScriptableObject.Instantiate<ShrapnelAbilityData>(shrapnelAbilityData);
			modifier.target = target;
			modifier.aoe = aoe.ToList();
		});
	}

}
