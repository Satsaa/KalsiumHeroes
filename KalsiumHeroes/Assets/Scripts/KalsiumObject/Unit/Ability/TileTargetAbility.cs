
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileTargetAbility : TargetAbility {

	[Tooltip("Types of valid targets.")]
	public TargetType targetType;

	[Tooltip("Radius of the affected tiles around the target.")]
	public Radius radius;

	[Serializable]
	public class TargetType : Attribute<TileTargetType> {
		private TargetType() : base(TileTargetType.Any) { }
		public override string identifier => "Attribute_TileTargetAbility_TargetType";
		public override string TooltipText(IAttribute source) => current == TileTargetType.Any ? null : DefaultTooltip(source, Lang.GetStr("Targets"));
		public override string Format(bool isSource) {
			var str = current.value.ToString();
			return String.Join(", ", str
				.Split(new string[] { ", " }, 0)
				.Where(v => v != nameof(TileTargetType.Any))
				.Select(v => Lang.GetStr($"{identifier}_{v}"))
			);
		}
	}

	[Serializable]
	public class Radius : Attribute<int> {
		public override string identifier => "Attribute_TileTargetAbility_Radius";
		public override string TooltipText(IAttribute source) => current == 0 ? null : DefaultTooltip(source);
	}


	/// <summary>
	/// Returns a list of affected Tiles if the Ability is cast on tile.
	/// Used for highlighting and should be used when casting the ability (ensures equivalency).
	/// </summary>
	public virtual IEnumerable<Tile> GetAffectedArea(Tile tile) {
		return Game.grid.Radius(tile, radius.current);
	}

	/// <summary> Returns a list of valid target Tiles. </summary>
	public virtual IEnumerable<Tile> GetTargets() {
		var targets = GetDefaultRangeTiles();

		var value = targetType.current.value;
		if (!value.HasFlag(TileTargetType.Any)) {
			if (value == TileTargetType.None) return Enumerable.Empty<Tile>();
			var ally = value.HasFlag(TileTargetType.Ally);
			var enemy = value.HasFlag(TileTargetType.Enemy);
			var ground = value.HasFlag(TileTargetType.Ground);
			var wall = value.HasFlag(TileTargetType.Wall);
			return targets.Where(v
				=> (ally && v.units.Any(u => u.team == unit.team))
				|| (enemy && v.units.Any(u => u.team != unit.team))
				|| (ground && v.passable.current)
				|| (wall && !v.passable.current));
		}

		return targets;
	}

	public override string CombatLog(GameEvents.Ability msg) {
		return $"{Lang.GetStr($"{unit.identifier}_DisplayName")} casted {Lang.GetStr($"{identifier}_DisplayName")} on some tiles...";
	}
}