using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickOffAbility : UnitTargetAbility {

	public new PickOffAbilityData data => (PickOffAbilityData)_data;
	public override Type dataType => typeof(PickOffAbilityData);

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.index];
			var aoe = GetAffectedArea(target);
			var finalDamage = data.damage.value * GetMultiplier(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, finalDamage, data.damageType);
				}
			}
		});
	}

	float GetMultiplier(Unit target) {
		for (int i = 0; i < data.multipliers.Length; i++) {
			var mult = data.multipliers[i];
			var ring = Game.grid.Ring(target.tile, i);
			foreach (var tile in ring) {
				if (tile.units.Any(v => data.multiplyingTypes.TargetIsCompatible(target, v))) {
					return mult;
				}
			}
		}
		return data.multipliers.Last();
	}

}
