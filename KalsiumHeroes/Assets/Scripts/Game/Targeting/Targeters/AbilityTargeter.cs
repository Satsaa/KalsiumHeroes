
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
		return ability switch {
			TileTargetAbility ability => new HashSet<Tile>(ability.GetTargets()),
			UnitTargetAbility ability => new HashSet<Tile>(ability.GetTargets().Select(v => v.tile)),
			_ => throw new InvalidOperationException($"Unknown TargetAbility type: {ability.GetType().Name}"),
		};
	}

	public override HashSet<Tile> GetHover(Tile tile) {
		return ability switch {
			TileTargetAbility ability => new HashSet<Tile>(ability.GetAffectedArea(tile)),
			UnitTargetAbility ability => tile.hasUnits ? new HashSet<Tile>(ability.GetAffectedArea(tile.units.First())) : new HashSet<Tile>(),
			_ => throw new InvalidOperationException($"Unknown TargetAbility type: {ability.GetType().Name}"),
		};
	}
}
