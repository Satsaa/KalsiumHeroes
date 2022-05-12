using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HuntTheMarkAbility), menuName = "KalsiumHeroes/Ability/" + nameof(HuntTheMarkAbility))]
public class HuntTheMarkAbility : UnitTargetAbility {

	public Attribute<float> normalDamage;

	public Attribute<float> silenceDamage;

	public DamageType damageType;


	public override IEnumerable<Unit> GetTargets() {
		return base.GetTargets().Where(v => v.modifiers.Contains<MarkOfPreyStatus>() && TargetHasFreeTiles(v));
	}

	public override bool IsReady() {
		return base.IsReady() && Game.dataObjects.Get<MarkOfPreyStatus>().Any(v => v.unit != unit && TargetHasFreeTiles(v.unit));
	}

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					if (unit.silenced.current) unit.DealCalculatedDamage(this, silenceDamage.current, damageType);
					else unit.DealCalculatedDamage(this, normalDamage.current, damageType);
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
