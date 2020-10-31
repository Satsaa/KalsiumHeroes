using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRangeDamageAbility : Ability {

  public MeleeRangeDamageData meleeRangeDamageData => (MeleeRangeDamageData)data;
  public override Type dataType => typeof(MeleeRangeDamageData);

  public override bool EventIsFinished() => true;
  public override bool SkipEvent() => true;
  public override void StartEvent(Events.Ability data) {
    var target = Game.grid.hexes[data.target];
    var aoe = GetAffectedArea(target);
    foreach (var hex in aoe) {
      if (hex.unit) hex.unit.Damage(meleeRangeDamageData.damage.value, meleeRangeDamageData.damageType);
    }
  }


}
