
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AbilityTargeter : Targeter {

	public Unit unit;
	public TargetAbility ability;

	public AbilityTargeter(Unit unit, TargetAbility ability, Action<Targeter> onComplete, Action<Targeter> onCancel = null) {
		this.unit = unit;
		this.ability = ability;
		this.onComplete = onComplete;
		this.onCancel = onCancel;
	}

	public override bool IsCompleted() {
		return selections.Count > 0;
	}

	public override HashSet<Tile> GetTargets() {
		switch (ability) {
			case TileTargetAbility ability:
				return new HashSet<Tile>(ability.GetTargets());
			case UnitTargetAbility ability:
				return new HashSet<Tile>(ability.GetTargets().Select(v => v.tile));
			default:
				throw new InvalidOperationException($"Unknown TargetAbility type: {ability.GetType().Name}");
		}
	}

	public override HashSet<Tile> GetHover(Tile tile) {
		switch (ability) {
			case TileTargetAbility ability:
				return new HashSet<Tile>(ability.GetAffectedArea(tile));
			case UnitTargetAbility ability:
				if (tile.hasUnits) return new HashSet<Tile>(ability.GetAffectedArea(tile.units.First()));
				else return new HashSet<Tile>();
			default:
				throw new InvalidOperationException($"Unknown TargetAbility type: {ability.GetType().Name}");
		}
	}
}
