using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddDataComponentAbility : Ability {

	public AddDataComponentData createUnitAbilityData => (AddDataComponentData)data;
	public override Type dataType => typeof(AddDataComponentData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					foreach (var component in createUnitAbilityData.components) {
						tile.unit.gameObject.AddDataComponent(component);
					}
				}
			}
		});
	}

}
