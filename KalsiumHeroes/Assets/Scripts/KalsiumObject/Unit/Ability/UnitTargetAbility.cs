
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitTargetAbility : TargetAbility {

	[Tooltip("Types of valid targets.")]
	public UnitTargetType targetType;


	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on the Tile of unit.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Unit unit) {
		if (unit == null) return new List<Tile>();
		return new List<Tile>() { unit.tile };
	}

	/// <summary> Returns a list of valid target Units. </summary>
	public virtual IEnumerable<Unit> GetTargets() {

		bool self = (targetType & UnitTargetType.Self) != 0;
		bool ally = (targetType & UnitTargetType.Ally) != 0;
		bool enemy = (targetType & UnitTargetType.Enemy) != 0;

		var tiles = GetDefaultRangeTiles();
		tiles = tiles.Where(h => {
			if (!h.hasUnits) return false;
			// TODO: Unit based targeting
			var tileUnit = h.units[0];
			if (self && tileUnit == unit) return true;
			if (ally && tileUnit.team == unit.team && tileUnit != this.unit) return true;
			if (enemy && tileUnit.team != unit.team) return true;
			return false;
		});

		return tiles.Select(v => v.units[0]);
	}

	public override string CombatLog(GameEvents.Ability msg) {
		return $"{Lang.GetStr($"{unit.identifier}_DisplayName")} casted {Lang.GetStr($"{identifier}_DisplayName")} on {Lang.GetStr($"{Game.grid.GetTile(msg.targets[0]).units[0].identifier}_DisplayName")}.";
	}
}