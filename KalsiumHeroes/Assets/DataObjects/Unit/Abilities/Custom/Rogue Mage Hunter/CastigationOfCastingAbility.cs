using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastigationOfCastingAbility : UnitTargetAbility {

	public new CastigationOfCastingAbilityData data => (CastigationOfCastingAbilityData)_data;
	public override Type dataType => typeof(CastigationOfCastingAbilityData);

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.index];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					Modifier.Create(unit, data.markOfCastigationModifier);
				}
			}
		});
	}
}
