using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpportuneFlightAbility : TileTargetAbility {

	public Attribute<int> moveDistance;


	public override IEnumerable<Tile> GetTargets() {
		UnitPather pather = UnitPathers.Unphased;
		return base.GetTargets().Where(v => Game.grid.Distance(unit.tile, v) == 1 && pather(unit, unit.tile, unit.tile.EdgeBetween(v), v));
	}

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				DoMove(tile);
			}
		});
	}

	void DoMove(Tile tile) {
		var dir = unit.tile.GetDir(tile);
		unit.SetDir(dir, true);
		for (int i = 0; i < moveDistance.current; i++) {
			if (unit.CanMoveInDir(dir, out Tile next)) {
				ExecuteMoveOff(unit, unit.tile);
				ExecuteMoveOver(unit, unit.tile, next);
				unit.SetTile(next, true);
				ExecuteMoveOn(unit, next);
			} else {
				break;
			}
		}
	}

}
