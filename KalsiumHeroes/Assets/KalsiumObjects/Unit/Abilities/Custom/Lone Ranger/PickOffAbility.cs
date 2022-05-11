using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickOffAbility : UnitTargetAbility {

	public Attribute<float> damage;

	public DamageType damageType;

	[Tooltip("Only units matching any of these unit target types affect the multiplier.")]
	public UnitTargetType multiplyingTypes;

	[Tooltip("Damage multipliers based on the distance of the nearest unit of the target.")]
	public float[] multipliers;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			var finalDamage = damage.current * GetMultiplier(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, finalDamage, damageType);
				}
			}
		});
	}

	float GetMultiplier(Unit target) {
		for (int i = 0; i < multipliers.Length; i++) {
			var mult = multipliers[i];
			var ring = Game.grid.Ring(target.tile, i);
			foreach (var tile in ring) {
				if (tile.units.Any(v => multiplyingTypes.TargetIsCompatible(target, v))) {
					return mult;
				}
			}
		}
		return multipliers.LastOrDefault();
	}

}
