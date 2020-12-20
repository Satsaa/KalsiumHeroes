
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AbilityTargeter : Targeter {

	public Unit unit;
	public Ability ability;

	public AbilityTargeter(Unit unit, Ability ability, Action<Targeter> onComplete, Action<Targeter> onCancel = null) {
		this.unit = unit;
		this.ability = ability;
		this.onComplete = onComplete;
		this.onCancel = onCancel;
	}

	public override bool IsCompleted() {
		return selections.Count > 0;
	}

	public override HashSet<Tile> GetTargets() {
		return new HashSet<Tile>(ability.GetTargets());
	}

	public override HashSet<Tile> GetHover(Tile tile) {
		return new HashSet<Tile>(ability.GetAffectedArea(tile));
	}
}
