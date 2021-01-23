﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideKickAbility : Ability {

	public new SideKickAbilityData data => (SideKickAbilityData)_data;
	public override Type dataType => typeof(SideKickAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var speed = unit.data.speed.value;
			var movement = unit.data.movement.value;
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			var movementDamage = movement * data.movementDamageMultiplier.value;
			var totalDamage = speed * data.speedDamageMultiplier.value + movementDamage;
			foreach (var tile in aoe) {
				if (tile.unit) DealDamage(tile.unit, totalDamage, data.damageType);
				Debug.Log("Raw Total Damage Dealt: " + (totalDamage) + " From Speed: " + speed * data.speedDamageMultiplier.value + " From Movement: " + movementDamage);
			}
		});
	}
}