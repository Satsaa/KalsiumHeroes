﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGenericAbility : Ability {

	public HealGenericData healGenericData => (HealGenericData)data;
	public override Type dataType => typeof(HealGenericData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.Heal(healGenericData.heal.value);
			}
		});
	}

}
