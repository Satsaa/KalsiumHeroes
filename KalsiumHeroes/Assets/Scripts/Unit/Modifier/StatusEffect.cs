using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Systems.Values;

public class StatusEffect : UnitModifier {

  public DebuffType debuffType = DebuffType.None;
  public bool positive = true;
  public bool dispellable = true;
  public ToggleAttribute<int> turnDuration = new ToggleAttribute<int>(false);

  public override void OnTurnEnd() {
    if (turnDuration.enabled && --turnDuration.value <= 0) {
      OnExpire();
    }
  }

  /// <summary> When the Unit got dispelled </summary>
  public virtual void OnDispell() {
    if (dispellable) Destroy(this);
  }
  /// <summary> When this Modifier expires because of duration was reached </summary>
  public virtual void OnExpire() {
    Destroy(this);
  }
}