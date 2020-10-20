
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class TargetingSequence {

  public IEnumerable<GameHex> targets = new List<GameHex>(0);
  public List<GameHex> selection = new List<GameHex>(0);
  public Dictionary<GameHex, Color> highlights = new Dictionary<GameHex, Color>();

  public Action<TargetingSequence> onComplete;
  public Action<TargetingSequence> onCancel;

  protected Color targetColor => new Color(0.25f, 0.75f, 0.25f);
  protected Color selectionColor => new Color(0.25f, 0.25f, 1f);

  public abstract bool IsCompleted();
  public abstract void RefreshTargets();

  public virtual void RefreshHighlights() {
    highlights.Clear();
    foreach (var target in targets) {
      highlights[target] = targetColor;
    }
  }

  /// <summary> Attempt to select a hex. Return true if the selection is accepted. </summary>
  public virtual bool Select(GameHex hex) {
    if (IsCompleted()) throw new InvalidOperationException($"Attempted to select after the {nameof(TargetingSequence)} was completed.");
    if (targets.Contains(hex)) {
      selection.Add(hex);
      return true;
    }
    return false;
  }

  /// <summary> Attempt canceling targeting sequence. Usually when clicking an invalid target. Return true to accept cancellation. </summary>
  public virtual bool Cancel() {
    return true;
  }
}


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