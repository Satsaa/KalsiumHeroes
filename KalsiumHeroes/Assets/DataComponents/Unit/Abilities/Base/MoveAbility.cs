using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability {

	[HideInInspector] public float usedMovement;


	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
		return new MoveAbilityHandler(data, this);
	}

	public override bool IsReady() {
		if (unit.rooted.value) return false;
		return usedMovement < unit.unitData.movement.value && base.IsReady();
	}

	protected override IEnumerable<Tile> GetTargets_GetRangeTargets(Tile tile) {
		var maxCost = unit.unitData.movement.value - usedMovement;
		var res = Pathing.GetCostField(tile, maxCost: maxCost, pather: Pathers.OneWayUnitBlocking).costs.Keys;
		return res;
	}

	public override void OnTurnStart() {
		usedMovement = 0;
		base.OnTurnStart();
	}

	public override Targeter GetTargeter() {
		return new PathConfirmAbilityTargeter(unit, this,
			onComplete: t => PostDefaultAbilityEvent(t.selection[0])
		) { pather = Pathers.OneWayUnitBlocking };
	}

}
