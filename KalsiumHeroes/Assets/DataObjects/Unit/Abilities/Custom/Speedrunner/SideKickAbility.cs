using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SideKickAbility : UnitTargetAbility {

	public Attribute<float> movementDamageMultiplier;

	public Attribute<float> speedDamageMultiplier;

	public DamageType damageType;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var speed = unit.speed.current;
			var movement = unit.movement.current;
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			var movementDamage = movement * movementDamageMultiplier.current;
			var totalDamage = speed * speedDamageMultiplier.current + movementDamage;
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, totalDamage, damageType);
					Debug.Log("Raw Total Damage Dealt: " + totalDamage + " From Speed: " + speed * speedDamageMultiplier.current + " From Movement: " + movementDamage);
				}
			}
		});
	}
}
