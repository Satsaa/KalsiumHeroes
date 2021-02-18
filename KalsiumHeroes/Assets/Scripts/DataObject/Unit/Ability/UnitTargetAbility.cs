
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitTargetAbility : TargetAbility {

	public new UnitTargetAbilityData source => (UnitTargetAbilityData)_source;
	public new UnitTargetAbilityData data => (UnitTargetAbilityData)_data;
	public override Type dataType => typeof(UnitTargetAbilityData);

	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on tile.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Unit unit) {
		if (unit == null) return new List<Tile>();
		return new List<Tile>() { unit.tile };
	}

	/// <summary> Returns a list of valid target Units. </summary>
	public virtual IEnumerable<Unit> GetTargets() {

		bool self = (data.targetType & UnitTargetType.Self) != 0;
		bool ally = (data.targetType & UnitTargetType.Ally) != 0;
		bool enemy = (data.targetType & UnitTargetType.Enemy) != 0;

		var tiles = GetDefaultRangeTiles();
		tiles = tiles.Where(h => {
			if (!h.hasUnits) return false;
			// TODO: Unit based targeting
			var tileUnit = h.units.First();
			if (self && tileUnit == unit) return true;
			if (ally && tileUnit.team == unit.team && tileUnit != this.unit) return true;
			if (enemy && tileUnit.team != unit.team) return true;
			return false;
		});

		return tiles.Select(v => v.units.First());
	}
}