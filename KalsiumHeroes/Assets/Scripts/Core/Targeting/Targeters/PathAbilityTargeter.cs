using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathConfirmAbilityTargeter : AbilityTargeter {

	public Pather pather;

	public PathConfirmAbilityTargeter(Unit unit, Ability ability, Action<Targeter> onComplete, Action<Targeter> onCancel = null) : base(unit, ability, onComplete, onCancel) {
		if (pather == null) {
			switch (ability.abilityData.rangeMode) {
				case RangeMode.PathCost:
				case RangeMode.PathDistance:
					pather = Pathers.OneWayUnitBlocking;
					break;
				case RangeMode.PathCostPassThrough:
				case RangeMode.PathDistancePassThrough:
					pather = Pathers.OneWay;
					break;
				case RangeMode.Distance:
				default:
					pather = delegate { return true; };
					break;
			}
		}
	}


	public override bool Hover(Tile tile) {
		var valid = base.Hover(tile);
		if (valid) {
			Pathing.CheapestPath(unit.tile, tile, out var path, out var _, pather);
			foreach (var segment in path) {
				hovers.Add(segment);
			}
		}
		return valid;
	}
}