using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathConfirmAbilityTargeter : AbilityTargeter {

	public Pather pather;
	public CostCalculator cc;
	protected Pathing.FieldResult field;

	public PathConfirmAbilityTargeter(Unit unit, TargetAbility ability, float maxCost, Action<Targeter> onComplete, Action<Targeter> onCancel = null) : base(unit, ability, onComplete, onCancel) {
		pather ??= Pathers.For(ability.rangeMode);
		cc ??= CostCalculators.For(ability.rangeMode);
		field = Pathing.GetCostField(unit.tile, maxCost: maxCost, pather: pather, costCalculator: cc);
	}


	public override HashSet<Tile> GetHover(Tile tile) {
		Tile[] path;
		Pathing.CheapestPath(unit.tile, tile, out var result, pather, cc);
		path = result.path;
		return new HashSet<Tile>(path);
	}
}