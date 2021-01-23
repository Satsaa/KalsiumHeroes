using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntTheMarkAbility : Ability {

	public new HuntTheMarkAbilityData data => (HuntTheMarkAbilityData)_data;
	public override Type dataType => typeof(HuntTheMarkAbilityData);

    public override IEnumerable<Tile> GetTargets() {
        return base.GetTargets().Where(v => v.unit && TargetHasFreeTiles(v) && v.unit.modifiers.Get<MarkOfPreyStatus>().Any());
    }

    public override bool IsReady() {
		return base.IsReady() && Game.dataObjects.Get<MarkOfPreyStatus>().Where(v => v.unit != unit && TargetHasFreeTiles(v.unit.tile)).Any();
	}

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				if (tile.unit) {
					if (tile.unit.silenced.value) tile.unit.DealCalculatedDamage(this, data.silenceDamage.value, data.damageType);
					else tile.unit.DealCalculatedDamage(this, data.normalDamage.value, data.damageType);
					if (tile.unit.modifiers.Get<MarkOfPreyStatus>().Any()) tile.unit.modifiers.Get<MarkOfPreyStatus>().First().Remove();
					unit.MoveTo(FindClosestTile(tile.unit), true);
				}
			}
		});
	}

	bool TargetHasFreeTiles(Tile tile) {
		bool freeTile = false;
		var radius = Game.grid.Ring(tile, 1);
		foreach (var hex in radius) {
			if (!hex.unit && hex.data.passable.value) freeTile = true;
		}
		return freeTile;
	}

	Tile FindClosestTile(Unit target) {
		var targetTile = target.tile;
		var radius = Game.grid.Ring(targetTile, 1);
		var closestTile = radius.First();
		foreach (var tile in radius) {
			if (!tile.unit && tile.data.passable.value) {
				var distanceA = Game.grid.Distance(closestTile, unit.tile);
				var distanceB = Game.grid.Distance(tile, unit.tile);
				if (distanceB < distanceA) closestTile = tile;
				if (distanceA == distanceB && (closestTile.unit || closestTile.data.passable.value)) closestTile = tile;
			}
			if (!tile.unit && (closestTile.unit || !closestTile.data.passable.value)) closestTile = tile;
		}
		return closestTile;
	}
}
