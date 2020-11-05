using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealGenericAbility : Ability
{
    public HealGenericData healGenericData => (HealGenericData)data;

    public override Type dataType => typeof(HealGenericData);

    public override bool EventIsFinished() => true;

    public override bool SkipEvent() => true;

    public override void StartEvent(Events.Ability data) {
        var target = Game.grid.hexes[data.target];
        var aoe = GetAffectedArea(target);
        foreach (var hex in aoe) {
            if (hex.unit) hex.unit.Heal(healGenericData.heal.value);
        }
    }

}
