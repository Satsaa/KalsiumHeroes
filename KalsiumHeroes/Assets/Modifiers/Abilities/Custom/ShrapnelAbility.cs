﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelAbility : Ability {

  public ShrapnelAbilityData shrapnelAbilityData => (ShrapnelAbilityData)data;
  public override Type dataType => typeof(ShrapnelAbilityData);

  public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
    return new InstantAbilityHandler(data, this, (ability) => {
      var target = Game.grid.hexes[data.target];
      var aoe = GetAffectedArea(target);

      var unit = ability.unit;

      var modifier = Modifier.AddEntityComponent(ability.unit.gameObject, shrapnelAbilityData.shrapnelModifierData) as ShrapnelAbilityModifier;
      modifier.casterData = ScriptableObject.Instantiate<ShrapnelAbilityData>(shrapnelAbilityData);
      modifier.target = target;
      modifier.aoe = aoe;
    });
  }

}