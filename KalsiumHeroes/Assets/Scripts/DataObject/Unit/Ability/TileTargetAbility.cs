
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileTargetAbility : TargetAbility {

	public new TileTargetAbilityData source => (TileTargetAbilityData)_source;
	public new TileTargetAbilityData data => (TileTargetAbilityData)_data;
	public override Type dataType => typeof(TileTargetAbilityData);

	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on tile.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Tile tile) {
		return Game.grid.Radius(tile, data.radius.current);
	}

	/// <summary> Returns a list of valid target Tiles. </summary>
	public virtual IEnumerable<Tile> GetTargets() {
		var targets = GetDefaultRangeTiles();

		var value = data.targetType.current.value;
		if (!value.HasFlag(TileTargetType.Any)) {
			if (value == TileTargetType.None) return Enumerable.Empty<Tile>();
			var ally = value.HasFlag(TileTargetType.Ally);
			var enemy = value.HasFlag(TileTargetType.Enemy);
			var ground = value.HasFlag(TileTargetType.Ground);
			var wall = value.HasFlag(TileTargetType.Wall);
			return targets.Where(v
				=> (ally && v.units.Any(u => u.team == unit.team))
				|| (enemy && v.units.Any(u => u.team != unit.team))
				|| (ground && v.data.passable.current)
				|| (wall && !v.data.passable.current));
		}

		return targets;
	}

}