﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MoveAbility), menuName = "KalsiumHeroes/Ability/" + nameof(MoveAbility))]
public class MoveAbility : TileTargetAbility, IOnAbilityCastStart_Unit {

	[HideInInspector] public float usedMovement;
	[HideInInspector, SerializeField] bool blocked;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new MoveAbilityHandler(msg, this);
	}

	public override bool IsReady() {
		if (blocked) return false;
		if (unit.rooted.current) return false;
		var energyMovement = GetPaidMovement(unit.movement.current, unit.energy.current);
		return (usedMovement - energyMovement) < unit.movement.current && base.IsReady();
	}

	public override IEnumerable<Tile> GetTargets() {
		var movement = unit.movement.current;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.energy.current);
		var maxCost = freeMovement + energyMovement;
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
		if (!ability.allowMove.current) blocked = true;
	}

	public override void OnTurnStart() {
		blocked = false;
		usedMovement = 0;
		base.OnTurnStart();
	}

	public override Targeter GetTargeter() {
		var movement = unit.movement.current;
		var freeMovement = movement - usedMovement;
		var energyMovement = GetPaidMovement(movement, unit.energy.current);
		return new MoveAbilityTargeter(unit, this, freeMovement, energyMovement,
			onComplete: t => PostDefaultAbilityEvent(t.selections.ToArray())
		) { pather = Pathers.For(rangeMode), cc = CostCalculators.For(rangeMode) };
	}

	public override string CombatLog(GameEvents.Ability msg) {
		return $"{Lang.GetStr($"{unit.identifier}_DisplayName")} moved from {unit.tile.hex} to {new HexGrid.Hex(msg.targets.Last())}.";
	}
}
