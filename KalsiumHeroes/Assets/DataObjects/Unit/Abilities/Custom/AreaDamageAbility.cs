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
			var aoe = GetAffectedArea(target);
			var primaryTarget = target.unit;
			if (target.unit != null) DealDamage(primaryTarget, data.primaryDamage.value, data.damageType);
			foreach (var tile in aoe) {
				if (tile.unit) DealDamage(tile.unit, data.secondaryDamage.value, data.damageType);
			}
		});
	}
}
