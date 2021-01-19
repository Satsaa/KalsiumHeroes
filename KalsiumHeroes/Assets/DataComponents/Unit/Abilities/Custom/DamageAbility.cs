using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageAbility : Ability {

	public new DamageAbilityData data => (DamageAbilityData)base.data;
	public override Type dataType => typeof(DamageAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) DealDamage(tile.unit, data.damage.value, data.damageType);
			}
		});
	}

}
