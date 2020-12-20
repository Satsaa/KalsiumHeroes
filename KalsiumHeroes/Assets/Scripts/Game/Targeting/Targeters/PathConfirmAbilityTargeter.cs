using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathConfirmAbilityTargeter : AbilityTargeter {

	public Pather pather;
	public CostCalculator cc;
	Pathing.FieldResult field;

	public PathConfirmAbilityTargeter(Unit unit, Ability ability, float maxCost, Action<Targeter> onComplete, Action<Targeter> onCancel = null) : base(unit, ability, onComplete, onCancel) {
		pather ??= (ability.abilityData.rangeMode) switch {
			RangeMode.Distance => Pathers.FlyingPhased,

			RangeMode.PathDistance => Pathers.Unphased,
			RangeMode.PathDistancePhased => Pathers.Phased,
			RangeMode.PathDistanceFlying => Pathers.Flying,
			RangeMode.PathDistancePhasedFlying => Pathers.FlyingPhased,

			RangeMode.PathCost => Pathers.Unphased,
			RangeMode.PathCostPhased => Pathers.Phased,
			RangeMode.PathCostFlying => Pathers.Flying,
			RangeMode.PathCostPhasedFlying => Pathers.FlyingPhased,

			_ => Pathers.FlyingPhased
		};
		cc ??= (ability.abilityData.rangeMode) switch {
			RangeMode.Distance => CostCalculators.Distance,

			RangeMode.PathDistance => CostCalculators.Distance,
			RangeMode.PathDistancePhased => CostCalculators.Distance,
			RangeMode.PathDistanceFlying => CostCalculators.Distance,
			RangeMode.PathDistancePhasedFlying => CostCalculators.Distance,

			RangeMode.PathCost => CostCalculators.MoveCost,
			RangeMode.PathCostPhased => CostCalculators.MoveCost,
			RangeMode.PathCostFlying => CostCalculators.MoveCost,
			RangeMode.PathCostPhasedFlying => CostCalculators.MoveCost,

			_ => CostCalculators.Distance,
		};
		field = Pathing.GetCostField(unit.tile, maxCost: maxCost, pather: pather, costCalculator: cc);
	}


	public override HashSet<Tile> GetHover(Tile tile) {
		Tile[] path;
		// Differences with fields and paths atm so false is added
		if (false && field.tiles.ContainsKey(tile)) {
			path = field.BuildPath(tile);
		} else {
			Pathing.CheapestPath(unit.tile, tile, out var result, pather, cc);
			path = result.path;
		}
		return new HashSet<Tile>(path);
	}
}