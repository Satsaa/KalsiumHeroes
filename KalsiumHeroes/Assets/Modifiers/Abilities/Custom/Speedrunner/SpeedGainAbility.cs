using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGainAbility : Ability {
	public SpeedGainAbilityData speedGainAbilityData => (SpeedGainAbilityData)data;
	public override Type dataType => typeof(SpeedGainAbilityData);
	[HideInInspector]
	public Attribute<int> unitsFound;
	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (Ability) => {
			unitsFound.value = 0;
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit && tile.unit != unit) unitsFound.value++;
			}
			unit.gameObject.AddEntityComponent(speedGainAbilityData.speedGainModifier);
		});
	}
}
