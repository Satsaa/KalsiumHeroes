
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
		return Game.grid.Radius(tile, data.radius.value);
	}

	/// <summary> Returns a list of valid target Tiles. </summary>
	public virtual IEnumerable<Tile> GetTargets() {
		return GetDefaultRangeTiles();
	}

}