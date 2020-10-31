using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEntityComponentAbility : Ability {

  public AddEntityComponentAbilityData createUnitAbilityData => (AddEntityComponentAbilityData)data;
  public override Type dataType => typeof(AddEntityComponentAbilityData);

  public override bool EventIsFinished() => true;
  public override bool SkipEvent() => true;
  public override void StartEvent(Events.Ability data) {
    var target = Game.grid.hexes[data.target];
    var aoe = GetAffectedArea(target);
    foreach (var hex in aoe) {
      if (hex.unit) {
        foreach (var component in createUnitAbilityData.components) {
          hex.unit.gameObject.AddEntityComponent(component);
        }
      }
    }
  }


}
