
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InstantTargetingSequence : TargetingSequence {

  public InstantTargetingSequence(Action<TargetingSequence> onComplete, Action<TargetingSequence> onCancel) {
    this.onComplete = onComplete;
    this.onCancel = onCancel;
  }

  public override bool IsCompleted() {
    return true;
  }

  public override void RefreshTargets() { }
}
