using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateUnitAbility : Ability {

  public CreateUnitAbilityData createUnitAbilityData => (CreateUnitAbilityData)data;
  public override Type dataType => typeof(CreateUnitAbilityData);

  public override bool EventIsFinished() => true;
  public override bool SkipEvent() => true;
  public override void StartEvent(Events.Ability data) {
    var target = Game.grid.hexes[data.target];
    var aoe = GetAffectedArea(target);
    foreach (var hex in aoe) {
      if (!hex.unit) {
        var go = Instantiate(createUnitAbilityData.unitPrefab);
        go.GetComponent<Unit>().MovePosition(hex, true);
      }
    }
  }


}
