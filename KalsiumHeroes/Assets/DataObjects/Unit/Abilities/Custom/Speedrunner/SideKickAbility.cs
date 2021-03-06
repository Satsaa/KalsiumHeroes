﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideKickAbility : UnitTargetAbility {

	public new SideKickAbilityData data => (SideKickAbilityData)_data;
	public override Type dataType => typeof(SideKickAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var speed = unit.data.speed.current;
			var movement = unit.data.movement.current;
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			var movementDamage = movement * data.movementDamageMultiplier.current;
			var totalDamage = speed * data.speedDamageMultiplier.current + movementDamage;
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, totalDamage, data.damageType);
					Debug.Log("Raw Total Damage Dealt: " + (totalDamage) + " From Speed: " + speed * data.speedDamageMultiplier.current + " From Movement: " + movementDamage);
				}
			}
		});
	}
}
