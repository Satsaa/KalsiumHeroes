using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageAbility : Ability {

	public DamageAbilityData meleeRangeDamageData => (DamageAbilityData)data;
	public override Type dataType => typeof(DamageAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.DealAbilityDamage(meleeRangeDamageData.damage.value, this, meleeRangeDamageData.damageType);
			}
		});
	}

}
