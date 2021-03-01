using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAbility : UnitTargetAbility {

	public new ShoveAbilityData data => (ShoveAbilityData)_data;
	public override Type dataType => typeof(ShoveAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			UnitModifier.Create(target, data.rootModifier);
			Shove(target);
		});
	}

	void Shove(Unit target) {
		var dir = unit.tile.GetDir(target.tile);
		for (int i = 0; i < data.shoveDist.value; i++) {
			if (target.CanMoveInDir(dir, out Tile next)) {
				ExecuteMoveOff(target, target.tile);
				ExecuteMoveOver(target, target.tile, next);
				ExecuteMoveOn(target, next);
				target.SetTile(next, true);
			} else {
				break;
			}
		}
	}
}
