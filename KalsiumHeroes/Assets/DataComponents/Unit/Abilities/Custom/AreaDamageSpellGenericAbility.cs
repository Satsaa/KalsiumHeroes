using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageSpellGenericAbility : Ability {

	public AreaDamageSpellGenericData areaDamageSpellGenericData => (AreaDamageSpellGenericData)data;
	public override Type dataType => typeof(AreaDamageSpellGenericData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			var primaryTarget = target.unit;
			if (target.unit != null) primaryTarget.Damage(areaDamageSpellGenericData.primaryDamage.value, areaDamageSpellGenericData.damageType);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.Damage(areaDamageSpellGenericData.secondaryDamage.value, areaDamageSpellGenericData.damageType);
			}
		});
	}
}
