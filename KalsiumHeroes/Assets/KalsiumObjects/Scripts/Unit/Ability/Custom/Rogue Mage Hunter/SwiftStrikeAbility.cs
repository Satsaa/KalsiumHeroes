using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SwiftStrikeAbility), menuName = "KalsiumHeroes/Ability/" + nameof(SwiftStrikeAbility))]
public class SwiftStrikeAbility : UnitTargetAbility {

	public Attribute<float> damage;

	public DamageType damageType;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = base.GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, damage.current, damageType);
				}
			}
		});
	}
}
