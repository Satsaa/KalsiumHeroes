
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InstantTargeter : Targeter {

  public InstantTargeter(Action<Targeter> onComplete, Action<Targeter> onCancel = null) {
    this.onComplete = onComplete;
    this.onCancel = onCancel;
  }

  public override bool IsCompleted() {
    return true;
  }

  public override void RefreshTargets() { }
}
