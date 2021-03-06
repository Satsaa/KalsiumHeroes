using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiftStrikeAbility : UnitTargetAbility {

	public new SwiftStrikeAbilityData data => (SwiftStrikeAbilityData)_data;
	public override Type dataType => typeof(SwiftStrikeAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var damage = data.damage.current;
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					DealDamage(unit, data.damage.current, data.damageType);
				}
			}
		});
	}
}
