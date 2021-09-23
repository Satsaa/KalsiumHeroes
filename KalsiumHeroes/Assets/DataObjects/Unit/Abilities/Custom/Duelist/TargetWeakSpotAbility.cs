using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetWeakSpotAbility : UnitTargetAbility {

	new public TargetWeakSpotAbilityData data => (TargetWeakSpotAbilityData)_data;
	public override Type dataType => typeof(TargetWeakSpotAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var damage = data.damage.current;
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, data.damage.current, data.damageType);
					Modifier.Create(master, data.defenseReductionModifier);
				}
			}
		});
	}
}
