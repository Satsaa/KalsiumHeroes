using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class StatusEffect : UnitModifier {

  public StatusEffectData statusEffectData => (StatusEffectData)data;
  public override Type dataType => typeof(StatusEffectData);

  public override void OnTurnEnd() {
    if (statusEffectData.turnDuration && --statusEffectData.turnDuration.value <= 0) {
      OnExpire();
    }
  }

  /// <summary> When the Unit got dispelled </summary>
  public virtual void OnDispell() {
    if (statusEffectData.dispellable) Destroy(this);
  }
  /// <summary> When this Modifier expires because of duration was reached </summary>
  public virtual void OnExpire() {
    Destroy(this);
  }
}