using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickOffAbility : Ability {

	public new PickOffAbilityData data => (PickOffAbilityData)base.data;
	public override Type dataType => typeof(PickOffAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var damage = data.damage.value;
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			var finalDamage = CalculateDamage(damage);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.DealAbilityDamage(finalDamage, this, data.damageType);
			}
		});
	}

	float CalculateDamage(float i) {
		var multiplier = 1f;
		var h = unit.tile;
		var radius = Game.grid.Ring(h, 1);
		bool foundUnit = false;
		foreach (var tile in radius) {
			if (tile.unit && tile.unit != this.unit) {
				foundUnit = true;
			}
		}
		if (foundUnit) {
			print("Unit found within range of 1. Damage dealt: " + i * multiplier + " Multiplier was " + multiplier);
			return i * multiplier;
		} else {
			radius = Game.grid.Ring(h, 2);
			foreach (var tile in radius) {
				if (tile.unit && tile.unit != this.unit) {
					foundUnit = true;
				}
			}
			if (foundUnit) {
				multiplier = data.bonusDamageMultipliers[0];
				print("Unit found within range of 2. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
				return i * multiplier;
			} else {
				radius = Game.grid.Ring(h, 3);
				foreach (var tile in radius) {
					if (tile.unit && tile.unit != this.unit) {
						foundUnit = true;
					}
				}
				if (foundUnit) {
					multiplier = data.bonusDamageMultipliers[1];
					print("Unit found within range of 3. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
					return i * multiplier;
				} else {
					radius = Game.grid.Ring(h, 4);
					foreach (var tile in radius) {
						if (tile.unit && tile.unit != this.unit) {
							foundUnit = true;
						}
					}
					if (foundUnit) {
						multiplier = data.bonusDamageMultipliers[2];
						print("Unit found within range of 4. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
						return i * multiplier;
					} else {
						multiplier = data.bonusDamageMultipliers[3];
						print("No units found within range of 4. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
						return i * multiplier;
					}
				}
			}
		}
	}

}
