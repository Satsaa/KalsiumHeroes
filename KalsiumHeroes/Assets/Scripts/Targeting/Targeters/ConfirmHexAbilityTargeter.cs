
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class ConfirmHexAbilityTargeter : AbilityTargeter {

  public bool confirmed;

  public ConfirmHexAbilityTargeter(Unit unit, Ability ability, Action<Targeter> onComplete, Action<Targeter> onCancel)
      : base(unit, ability, onComplete, onCancel) { }

  public override bool IsCompleted() {
    return selection.Count > 0 && confirmed;
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