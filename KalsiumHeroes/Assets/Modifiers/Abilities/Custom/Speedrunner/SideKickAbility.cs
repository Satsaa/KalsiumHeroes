using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideKickAbility : Ability {
	public SideKickAbilityData sideKickAbilityData => (SideKickAbilityData)data;
	public override Type dataType => typeof(SideKickAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var speed = unit.unitData.speed.value;
			var movement = unit.unitData.movement.value;
			var target = Game.grid.hexes[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var hex in aoe) {
				if (hex.unit) hex.unit.Damage(speed * sideKickAbilityData.speedDamageMultiplier.value + movement * sideKickAbilityData.movementDamageMultiplier.value, sideKickAbilityData.damageType);
				print("Total Damage Dealt: " + (speed * sideKickAbilityData.speedDamageMultiplier.value + movement * sideKickAbilityData.movementDamageMultiplier.value) + " From Speed: " + speed * sideKickAbilityData.speedDamageMultiplier.value + " From Movement: " + movement * sideKickAbilityData.movementDamageMultiplier.value);
			}
		});
	}
}
