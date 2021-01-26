using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDamageAbility : TileTargetAbility {

	public new AreaDamageAbilityData data => (AreaDamageAbilityData)_data;
	public override Type dataType => typeof(AreaDamageAbilityData);

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			// Central damage
			foreach (var unit in target.units) {
				DealDamage(unit, data.centerDamage.value, data.damageType);
			}
			// Outer damage
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe.Where(v => v != target)) {
				foreach (var unit in tile.units) {
					DealDamage(unit, data.outerDamage.value, data.damageType);
				}
			}
		});
	}
}
