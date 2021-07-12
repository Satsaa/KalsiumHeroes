using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbility : TileTargetAbility, IOnAbilityCastStart_Unit {

	[HideInInspector] public float usedMovement;
	[HideInInspector, SerializeField] bool blocked;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new MoveAbilityHandler(msg, this);
	}

	public override bool IsReady() {
		if (blocked) return false;
		if (unit.data.rooted.current) return false;
		var energyMovement = GetPaidMovement(unit.data.movement.current, unit.data.energy.current);
		return (usedMovement - energyMovement) < unit.data.movement.current && base.IsReady();
	}

	public override IEnumerable<Tile> GetTargets() {
		var movement = unit.data.movement.current;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.data.energy.current);
		var maxCost = freeMovement + energyMovement;
		var rangeMode = data.rangeMode;
		var res = Pathing.GetCostField(unit.tile, maxCost: maxCost, pather: Pathers.For(rangeMode), costCalculator: CostCalculators.For(rangeMode)).tiles.Keys;
		return res;
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

	public void OnAbilityCastStart(Ability ability) {
		if (!ability.data.allowMove.current) blocked = true;
	}

	public override void OnTurnStart() {
		blocked = false;
		usedMovement = 0;
		base.OnTurnStart();
	}

	public override Targeter GetTargeter() {
		var movement = unit.data.movement.current;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.data.energy.current);
		return new MoveAbilityTargeter(unit, this, freeMovement, energyMovement,
			onComplete: t => PostDefaultAbilityEvent(t.selections.ToArray())
		) { pather = Pathers.For(data.rangeMode), cc = CostCalculators.For(data.rangeMode) };
	}

}
