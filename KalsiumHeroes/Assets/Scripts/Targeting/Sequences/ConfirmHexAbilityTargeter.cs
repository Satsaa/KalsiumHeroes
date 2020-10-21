
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConfirmHexAbilityTargetingSequence : AbilityTargetingSequence {

  public bool confirmed;

  public ConfirmHexAbilityTargetingSequence(Unit unit, Ability ability, Action<TargetingSequence> onComplete, Action<TargetingSequence> onCancel)
    : base(unit, ability, onComplete, onCancel) { }

  public override bool IsCompleted() {
    return (selection.Count > 0 && confirmed);
  }

  public override bool Select(GameHex hex) {
    var prevSelection = selection.FirstOrDefault();
    if (prevSelection == hex) {
      confirmed = true;
      return true;
    } else {
      selection.Clear();
      return base.Select(hex);
    }
  }
}