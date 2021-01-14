using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAbilityTargeter : PathConfirmAbilityTargeter {

	public float freeMovement;
	public float paidMovement;

	public MoveAbilityTargeter(Unit unit, Ability ability, float freeMovement, float paidMovement, Action<Targeter> onComplete, Action<Targeter> onCancel = null)
		: base(unit, ability, freeMovement + paidMovement, onComplete, onCancel) {
		this.freeMovement = freeMovement;
		this.paidMovement = paidMovement;
	}

	public override HashSet<Tile> GetTargets() {
		var res = base.GetTargets();
		RefreshCustom();
		return res;
	}

	public void RefreshCustom() {
		var paidTiles = field.tiles.Where(v => v.Value.cost > freeMovement).Select(v => v.Key);
		var customs = new Dictionary<Tile, (Color, int)>();
		foreach (var paidTile in paidTiles) {
			customs.Add(paidTile, (new Color(0.85f, 0.85f, 0f), Targeting.targetPriority + 1));
		}
		Game.targeting.RefreshCustoms(customs);
	}
}