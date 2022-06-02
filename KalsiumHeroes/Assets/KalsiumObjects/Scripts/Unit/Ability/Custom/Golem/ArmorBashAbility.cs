using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ArmorBashAbility), menuName = "KalsiumHeroes/Ability/" + nameof(ArmorBashAbility))]
public class ArmorBashAbility : UnitTargetAbility {

	public DamageType damageType;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, CalculateDamage(unit), damageType);
				}
			}
		});
	}

	float CalculateDamage(Unit target) {
		var damage = unit.defense.current - target.defense.current;
		damage = Mathf.Max(damage, 0);
		Debug.Log("Damage: " + damage + " (Golem Defense " + unit.defense.current + " - Target Defense " + target.defense.current + ")");
		return damage;
	}
}
