using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathConfirmAbilityTargeter : AbilityTargeter {

  public Predicate<GameHex> passable;

  public PathConfirmAbilityTargeter(Unit unit, Ability ability, Action<Targeter> onComplete, Action<Targeter> onCancel = null)
    : base(unit, ability, onComplete, onCancel
  ) {
    if (passable == null) {
      switch (ability.abilityData.rangeMode) {
        case RangeMode.PathCost:
        case RangeMode.PathDistance:
          passable = h => !h.blocked && !h.unit;
          break;
        case RangeMode.PathCostPassThrough:
        case RangeMode.PathDistancePassThrough:
          passable = h => !h.blocked;
          break;
        case RangeMode.Distance:
        default:
          passable = h => true;
          break;
      }
    }
  }


  public override bool Hover(GameHex hex) {
    var inRange = base.Hover(hex);
    if (inRange) {
      Game.grid.CheapestPath(unit.hex, hex, out var path, passable);
      foreach (var segment in path) {
        hovers.Add(segment);
      }
    }
    return inRange;
  }
}