using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastigationOfCastingAbility : UnitTargetAbility {

	new public CastigationOfCastingAbilityData data => (CastigationOfCastingAbilityData)_data;
	public override Type dataType => typeof(CastigationOfCastingAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					Modifier.Create(unit, data.markOfCastigationModifier);
				}
			}
		});
	}
}
