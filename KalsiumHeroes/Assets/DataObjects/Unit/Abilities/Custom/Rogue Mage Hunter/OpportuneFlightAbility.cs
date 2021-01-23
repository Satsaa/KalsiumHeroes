using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpportuneFlightAbility : Ability {

	public new OpportuneFlightAbilityData data => (OpportuneFlightAbilityData)_data;
	public override Type dataType => typeof(OpportuneFlightAbilityData);

	public override IEnumerable<Tile> GetTargets() {
		UnitPather pather = UnitPathers.Unphased;
		return base.GetTargets().Where(v => Game.grid.Distance(unit.tile, v) == 1 && pather(unit, unit.tile, unit.tile.EdgeBetween(v), v));
	}

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				DoMove(tile);
			}
		});
	}

	void DoMove(Tile tile) {
		var target = unit;
		var dir = unit.tile.GetDir(tile);
		for (int i = 0; i < data.moveDistance.value; i++) {
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
