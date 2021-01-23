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
		UnitPather pather = UnitPathers.Unphased;
		Tile prev = tile;
		while (true) {
			var current = tile.GetNeighbor(dir);
			if (current == null) break;
			if (prev.CanMoveTo(current, pather, target)) {
				ExecuteMoveOff(target, prev);
				ExecuteMoveOver(target, prev, current);
				ExecuteMoveOn(target, current);
				tile = current;
			} else {
				break;
			}
			prev = current;
		}
		target.MoveTo(tile, true);
	}
}
