
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConfirmTileAbilityTargeter : AbilityTargeter {

	public bool confirmed;

	public ConfirmTileAbilityTargeter(Unit unit, Ability ability, Action<Targeter> onComplete, Action<Targeter> onCancel)
		: base(unit, ability, onComplete, onCancel) { }

	public override bool IsCompleted() {
		return selection.Count > 0 && confirmed;
	}

	public override bool Select(Tile tile) {
		var prevSelection = selection.FirstOrDefault();
		if (prevSelection == tile) {
			confirmed = true;
			return true;
		} else {
			selection.Clear();
			return base.Select(tile);
		}
	}
}