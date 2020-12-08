using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEntityComponentAbility : Ability {

	public AddEntityComponentData createUnitAbilityData => (AddEntityComponentData)data;
	public override Type dataType => typeof(AddEntityComponentData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.hexes[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var hex in aoe) {
				if (hex.unit) {
					foreach (var component in createUnitAbilityData.components) {
						hex.unit.gameObject.AddEntityComponent(component);
					}
				}
			}
		});
	}

}
