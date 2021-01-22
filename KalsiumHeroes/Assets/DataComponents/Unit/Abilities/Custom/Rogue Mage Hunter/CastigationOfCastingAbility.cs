using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastigationOfCastingAbility : Ability {

	public new CastigationOfCastingAbilityData data => (CastigationOfCastingAbilityData)_data;
	public override Type dataType => typeof(CastigationOfCastingAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) Modifier.Create(tile.unit, data.markOfCastigationModifier);
			}
		});
	}
}
