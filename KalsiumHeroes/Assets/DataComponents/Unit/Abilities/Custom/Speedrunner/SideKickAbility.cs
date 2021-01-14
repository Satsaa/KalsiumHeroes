using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideKickAbility : Ability {

	public SideKickAbilityData sideKickAbilityData => (SideKickAbilityData)data;
	public override Type dataType => typeof(SideKickAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var speed = unit.unitData.speed.value;
			var movement = unit.unitData.movement.value;
			var target = Game.grid.tiles[data.targets.First()];
			var aoe = GetAffectedArea(target);
			var movementDamage = movement * sideKickAbilityData.movementDamageMultiplier.value;
			var totalDamage = speed * sideKickAbilityData.speedDamageMultiplier.value + movementDamage;
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.DealAbilityDamage(totalDamage, this, sideKickAbilityData.damageType);
				print("Total Damage Dealt: " + (totalDamage) + " From Speed: " + speed * sideKickAbilityData.speedDamageMultiplier.value + " From Movement: " + movementDamage);
			}
		});
	}
}
