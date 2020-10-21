
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Targeter {

  public IEnumerable<GameHex> targets = new List<GameHex>(0);
  public List<GameHex> selection = new List<GameHex>(0);
  public Dictionary<GameHex, Color> highlights = new Dictionary<GameHex, Color>();

  public Action<Targeter> onComplete;
  public Action<Targeter> onCancel;

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
    if (IsCompleted()) throw new InvalidOperationException($"Attempted to select after the {nameof(Targeter)} was completed.");
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
