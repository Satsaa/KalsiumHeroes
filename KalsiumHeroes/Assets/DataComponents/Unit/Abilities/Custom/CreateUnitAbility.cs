using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUnitAbility : Ability {

	public CreateUnitData createUnitData => (CreateUnitData)data;
	public override Type dataType => typeof(CreateUnitData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (!tile.unit) {
					var go = Instantiate(createUnitData.unitPrefab, tile.center, Quaternion.identity);
				}
			}
		});
	}

}
