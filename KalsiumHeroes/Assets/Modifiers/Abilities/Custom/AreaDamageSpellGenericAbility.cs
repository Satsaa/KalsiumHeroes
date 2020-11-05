using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamageSpellGenericAbility : Ability
{
    public AreaDamageSpellGenericData areaDamageSpellGenericData => (AreaDamageSpellGenericData)data;

    public override Type dataType => typeof(AreaDamageSpellGenericData);

    public override bool EventIsFinished() => true;

    public override bool SkipEvent() => true;

    public override void StartEvent(Events.Ability data) {
        var target = Game.grid.hexes[data.target];
        var aoe = GetAffectedArea(target);
        var primaryTarget = target.unit;
        if (target.unit != null) primaryTarget.Damage(areaDamageSpellGenericData.primaryDamage.value, areaDamageSpellGenericData.damageType);
        foreach (var hex in aoe) {
            if (hex.unit) hex.unit.Damage(areaDamageSpellGenericData.secondaryDamage.value, areaDamageSpellGenericData.damageType);
        }
    }
}
