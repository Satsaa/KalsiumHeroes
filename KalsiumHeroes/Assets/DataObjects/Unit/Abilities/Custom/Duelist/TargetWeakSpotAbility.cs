using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetWeakSpotAbility : UnitTargetAbility {

	public Attribute<float> damage;
	public DamageType damageType;

	public Attribute<float> defenseReduction;

	public Modifier defenseReductionModifier;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = base.GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					base.DealDamage(unit, this.damage.current, damageType);
					Modifier.Create(master, defenseReductionModifier);
				}
			}
		});
	}
}
