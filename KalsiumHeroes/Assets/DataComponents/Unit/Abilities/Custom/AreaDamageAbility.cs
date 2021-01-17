using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDamageAbility : Ability {

	public new AreaDamageAbilityData data => (AreaDamageAbilityData)base.data;
	public override Type dataType => typeof(AreaDamageAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			var primaryTarget = target.unit;
			if (target.unit != null) primaryTarget.DealAbilityDamage(data.primaryDamage.value, this, data.damageType);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.DealAbilityDamage(data.secondaryDamage.value, this, data.damageType);
			}
		});
	}
}
