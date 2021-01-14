using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddDataComponentAbility : Ability {

	public AddDataComponentAbilityData addDataComponentAbilityData => (AddDataComponentAbilityData)data;
	public override Type dataType => typeof(AddDataComponentAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					foreach (var component in addDataComponentAbilityData.components) {
						tile.unit.gameObject.AddDataComponent(component);
					}
				}
			}
		});
	}

}
