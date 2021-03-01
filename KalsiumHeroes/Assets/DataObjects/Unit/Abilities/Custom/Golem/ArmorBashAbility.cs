using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBashAbility : UnitTargetAbility {

	public new ArmorBashAbilityData data => (ArmorBashAbilityData)_data;
	public override Type dataType => typeof(ArmorBashAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, CalculateDamage(unit), data.damageType);
				}
			}
		});
	}

	float CalculateDamage(Unit target) {
		float damage = unit.data.defense.value - target.data.defense.value;
		damage = Mathf.Max(damage, 0);
		Debug.Log("Damage: " + damage + " (Golem Defense " + unit.data.defense.value + " - Target Defense " + target.data.defense.value + ")");
		return damage;
	}
}
