using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddDataComponentAbility : Ability {

	public new AddDataComponentAbilityData data => (AddDataComponentAbilityData)base.data;
	public override Type dataType => typeof(AddDataComponentAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					foreach (var component in data.components) {
						tile.unit.gameObject.AddDataComponent(component);
					}
				}
			}
		});
	}

}
