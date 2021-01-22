using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetWeakSpotAbility : Ability {

	public new TargetWeakSpotAbilityData data => (TargetWeakSpotAbilityData)_data;
	public override Type dataType => typeof(TargetWeakSpotAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var damage = data.damage.value;
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					DealDamage(tile.unit, data.damage.value, data.damageType);
					Modifier.Create(master, data.defenseReductionModifier);
				}
			}
		});
	}
}
