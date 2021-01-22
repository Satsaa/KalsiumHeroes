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
					tile.unit.MoveTo(GetTargetTile(tile), true);
				}
			}
		});
	}

	Tile GetTargetTile(Tile tile) {
		var dir = (TileDir)((IList<Tile>)unit.tile.neighbors).IndexOf(tile);
		Tile nbr = null;
		while ((nbr = tile.GetNeighbor(dir)) != null && !nbr.unit && nbr.data.passable.value) {
			tile = nbr;
		}
		return tile;
	}
}
