using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeSpeedAbility : UnitTargetAbility {

	public new ChangeSpeedAbilityData data => (ChangeSpeedAbilityData)_data;
	public override Type dataType => typeof(ChangeSpeedAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					unit.data.speed.value.value += data.speedChange.value;
				}
			}
		});
	}

}
