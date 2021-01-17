using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoveAbility : Ability {

	public new ShoveAbilityData data => (ShoveAbilityData)base.data;
	public override Type dataType => typeof(ShoveAbilityData);

	[HideInInspector]
	bool dontShove = false;

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			dontShove = false;
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					tile.unit.gameObject.AddDataComponent(data.rootModifier);
					tile.unit.MoveTo(GetTargetTile(tile), true);
				}
			}
		});
	}

	Tile GetTargetTile(Tile tile) {
		var dir = (TileDir)((IList<Tile>)unit.tile.neighbors).IndexOf(tile);
		if (!dontShove) {
			Tile nbr = null;
			while ((nbr = tile.GetNeighbor(dir)) != null && !nbr.unit && nbr.data.passable.value) {
				tile = nbr;
			}
		}
		return tile;
	}
}
