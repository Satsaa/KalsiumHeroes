using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickOffAbility : Ability {

	public PickOffAbilityData pickOffAbilityData => (PickOffAbilityData)data;
	public override Type dataType => typeof(PickOffAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new InstantAbilityHandler(data, this, (ability) => {
			var damage = pickOffAbilityData.damage.value;
			var target = Game.grid.tiles[data.target];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) tile.unit.Damage(CalculateDamage(damage), pickOffAbilityData.damageType);
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
				multiplier = pickOffAbilityData.bonusDamageMultipliers[0];
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
					multiplier = pickOffAbilityData.bonusDamageMultipliers[1];
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
						multiplier = pickOffAbilityData.bonusDamageMultipliers[2];
						print("Unit found within range of 4. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
						return i * multiplier;
					} else {
						multiplier = pickOffAbilityData.bonusDamageMultipliers[3];
						print("No units found within range of 4. Damage dealt " + i * multiplier + " Multiplier was " + multiplier);
						return i * multiplier;
					}
				}
			}
		}
	}

}
