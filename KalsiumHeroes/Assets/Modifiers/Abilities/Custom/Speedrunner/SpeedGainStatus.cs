using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedGainStatus : StatusEffect
{
    public SpeedGainModifierData speedGainModifierData => (SpeedGainModifierData)data;

    public override Type dataType => typeof(SpeedGainModifierData);

    protected override void OnRegisterAlterers() {
        var unitsFound = unit.GetComponent<SpeedGainAbility>().unitsFound.value;
        var oldMt = unit.unitData.movement.value;
        unit.unitData.movement.RegisterAlterer(v => v + speedGainModifierData.movementGain.value * unitsFound);
        print("Old Movement: " + oldMt + " New Movement: " + unit.unitData.movement.value);
        var oldSpd = unit.unitData.speed.value;
        unit.unitData.speed.RegisterAlterer(v => v + speedGainModifierData.speedGain.value * unitsFound);
        print("Old Speed: " + oldSpd + " New Speed: " + unit.unitData.speed.value);
    }
}
