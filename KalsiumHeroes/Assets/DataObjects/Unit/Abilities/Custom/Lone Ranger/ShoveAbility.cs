using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAbility : Ability {

	public new ShoveAbilityData data => (ShoveAbilityData)_data;
	public override Type dataType => typeof(ShoveAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					UnitModifier.Create(master, data.rootModifier);
					Shove(tile);
				}
			}
		});
	}

	void Shove(Tile tile) {
		var target = tile.unit;
		var dir = unit.tile.GetDir(tile);
		Tile prev = tile;
		for (int i = 0; i < data.shoveDist.value; i++) {
			if (target.CanMoveInDir(dir, out Tile next)) {
				ExecuteMoveOff(target, prev);
				ExecuteMoveOver(target, prev, next);
				ExecuteMoveOn(target, next);
				prev = tile = next;
			} else {
				break;
			}
		}
		target.SetPosition(tile, true);
	}
}
