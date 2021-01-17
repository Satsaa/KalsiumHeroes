using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : Ability {

	[HideInInspector] public float usedMovement;


	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new MoveAbilityHandler(msg, this);
	}

	public override bool IsReady() {
		if (unit.rooted.value) return false;
		var energyMovement = GetPaidMovement(unit.unitData.movement.value, unit.unitData.energy.value);
		return (usedMovement - energyMovement) < unit.unitData.movement.value && base.IsReady();
	}

	protected override IEnumerable<Tile> GetTargets_GetRangeTargets(Tile tile) {
		var movement = unit.unitData.movement.value;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.unitData.energy.value);
		var maxCost = freeMovement + energyMovement;
		var rangeMode = abilityData.rangeMode;
		var res = Pathing.GetCostField(tile, maxCost: maxCost, pather: Pathers.For(rangeMode), costCalculator: CostCalculators.For(rangeMode)).tiles.Keys;
		return res.Where(v => !v.unit); // Ignore tiles with units
	}

	public float GetPaidMovement(int movement, int energy) {
		if (movement < 3) return energy / 3;
		if (movement > 5) return energy / 1;
		return energy / 2;
	}

	public float GetPaidMovementCost(float cost, float movement) {
		cost -= movement - usedMovement;
		if (cost <= 0) return 0;
		if (movement < 3) return cost * 3;
		if (movement > 5) return cost * 1;
		return cost * 2;
	}

	public override void OnTurnStart() {
		usedMovement = 0;
		base.OnTurnStart();
	}

	public override Targeter GetTargeter() {
		var movement = unit.unitData.movement.value;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.unitData.energy.value);
		return new MoveAbilityTargeter(unit, this, freeMovement, energyMovement,
			onComplete: t => PostDefaultAbilityEvent(t.selections[0])
		) { pather = Pathers.For(abilityData.rangeMode), cc = CostCalculators.For(abilityData.rangeMode) };
	}

}
