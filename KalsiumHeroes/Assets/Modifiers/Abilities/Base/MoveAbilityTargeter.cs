using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MoveAbilityTargetingSequence : ConfirmHexAbilityTargetingSequence {

  public MoveAbilityTargetingSequence(Unit unit, Ability ability, Action<TargetingSequence> onComplete, Action<TargetingSequence> onCancel)
    : base(unit, ability, onComplete, onCancel) { }

  public override void RefreshHighlights() {
    base.RefreshHighlights(); // Default range colors

    var target = selection.FirstOrDefault();
    if (target) {
      Game.grid.CheapestPath(unit.hex, target, out var path);
      foreach (var segment in path) {
        highlights[segment] = selectionColor / 2f;
      }
      highlights[target] = selectionColor;
    }
  }

}