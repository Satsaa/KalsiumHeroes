using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntTheMarkAbility : UnitTargetAbility {

	public new HuntTheMarkAbilityData data => (HuntTheMarkAbilityData)_data;
	public override Type dataType => typeof(HuntTheMarkAbilityData);

	public override IEnumerable<Unit> GetTargets() {
		return base.GetTargets().Where(v => v.modifiers.Contains<MarkOfPreyStatus>() && TargetHasFreeTiles(v));
	}

	public override bool IsReady() {
		return base.IsReady() && Game.dataObjects.Get<MarkOfPreyStatus>().Any(v => v.unit != unit && TargetHasFreeTiles(v.unit));
	}

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					if (unit.data.silenced.value) unit.DealCalculatedDamage(this, data.silenceDamage.value, data.damageType);
					else unit.DealCalculatedDamage(this, data.normalDamage.value, data.damageType);
					if (unit.modifiers.Get<MarkOfPreyStatus>().Any()) unit.modifiers.Get<MarkOfPreyStatus>().First().Remove();
					unit.SetTile(FindClosestTile(unit), true);
				}
			}
		});
	}

	bool TargetHasFreeTiles(Unit target) {
		var pather = unit.GetPather();
		var tile = target.tile;
		for (int i = 0; i < 6; i++) {
			if (tile.PathTest(i, pather, unit)) return true;
		}
		return false;
	}

	Tile FindClosestTile(Unit target) {
		var pather = unit.GetPather();
		Tile closestTile = null;
		float closestDist = float.PositiveInfinity;
		for (int i = 0; i < 6; i++) {
			if (target.tile.PathTest(i, pather, unit)) {
				var tile = target.tile.GetNeighbor(i);
				var distance = Game.grid.Distance(tile, unit.tile);
				if (distance < closestDist) {
					closestTile = tile;
					closestDist = distance;
				}
			}
		}
		return closestTile;
	}
}
