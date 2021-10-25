using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AreaDamageAbility : TileTargetAbility {

	public Attribute<float> centerDamage;
	public Attribute<float> outerDamage;
	public DamageType damageType;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]];
			// Central damage
			foreach (var unit in target.units) {
				DealDamage(unit, centerDamage.current, damageType);
			}
			// Outer damage
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe.Where(v => v != target)) {
				foreach (var unit in tile.units) {
					DealDamage(unit, outerDamage.current, damageType);
				}
			}
		});
	}
}
