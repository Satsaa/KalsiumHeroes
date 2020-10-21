
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class AbilityTargetingSequence : TargetingSequence {

  public Unit unit;
  public Ability ability;

  public AbilityTargetingSequence(Unit unit, Ability ability, Action<TargetingSequence> onComplete, Action<TargetingSequence> onCancel) {
    this.unit = unit;
    this.ability = ability;
    this.onComplete = onComplete;
    this.onCancel = onCancel;
  }

  public override bool IsCompleted() {
    return (selection.Count > 0);
  }

  public override void RefreshTargets() {
    targets = ability.GetTargets();
  }
}