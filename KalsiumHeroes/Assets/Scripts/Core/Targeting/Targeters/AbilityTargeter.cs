
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
		return selection.Count > 0;
	}

	public override void RefreshTargets() {
		targets = ability.GetTargets();
	}

	public override bool Hover(Tile tile) {
		var valid = base.Hover(tile);
		if (valid) {
			hovers.Clear();
			hovers.UnionWith(ability.GetAffectedArea(tile));
		}
		return valid;
	}
}
