
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConfirmTileAbilityTargeter : AbilityTargeter {

	public bool confirmed;

	public ConfirmTileAbilityTargeter(Unit unit, TargetAbility ability, Action<Targeter> onComplete, Action<Targeter> onCancel)
		: base(unit, ability, onComplete, onCancel) { }

	public override bool IsCompleted() {
		return selections.Count > 0 && confirmed;
	}

	public override bool TrySelect(Tile tile) {
		var prevSelection = selections.LastOrDefault();
		if (prevSelection == tile) {
			confirmed = true;
			return true;
		} else {
			if (selections.Count > 0) selections.RemoveAt(selections.Count - 1);
			return base.TrySelect(tile);
		}
	}
}