using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkOfCastigationStatus : Status, IOnAbilityCastEnd_Unit
{
    public new MarkOfCastigationStatusData data => (MarkOfCastigationStatusData)base.data;
    public override Type dataType => typeof(MarkOfCastigationStatusData);

    public void OnAbilityCastEnd(Ability ability) {
        if (ability.data.abilityType == AbilityType.Spell) {
            ability.unit.gameObject.AddDataComponent(data.silenceModifier);
            ability.unit.gameObject.AddDataComponent(data.markOfPreyModifier);
            ability.unit.DealCalculatedDamage(this, data.damage.value, data.damageType);
            this.Destroy();
        }
    }
}
