using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageAbility : UnitTargetAbility {

	public new DamageAbilityData data => (DamageAbilityData)_data;
	public override Type dataType => typeof(DamageAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, data.damage.value, data.damageType);
				}
			}
		});
	}

}
