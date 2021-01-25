using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddModifierAbility : UnitTargetAbility {

	public new AddModifierAbilityData data => (AddModifierAbilityData)_data;
	public override Type dataType => typeof(AddModifierAbilityData);

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].unit;
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					foreach (var modifierData in data.addedModifiers) {
						UnitModifier.Create(tile.unit, modifierData);
					}
				}
			}
		});
	}

}
