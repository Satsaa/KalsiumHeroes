using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AddModifierAbility : UnitTargetAbility {

	[Tooltip("Added modifiers. (Hint: Try adding a dot status effect!)")]
	public UnitModifier[] modifiers;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					foreach (var modifierData in modifiers) {
						Create(unit, modifierData);
					}
				}
			}
		});
	}

}
