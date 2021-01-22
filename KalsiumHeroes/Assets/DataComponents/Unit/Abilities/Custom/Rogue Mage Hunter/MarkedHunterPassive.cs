using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MarkedHunterPassive : Passive, IOnDealDamage_Unit
{
    public new MarkedHunterPassiveData data => (MarkedHunterPassiveData)base.data;
    public override Type dataType => typeof(MarkedHunterPassiveData);

    public void OnDealDamage(UnitModifier source, Unit targetUnit, float damage, DamageType damageType) {
        print("at least this works?");
        if (targetUnit.data.health.value > 0 && !targetUnit.modifiers.Get<MarkOfPreyStatus>().Any()) targetUnit.gameObject.AddDataComponent(data.markOfPreyModifier);
    }
}
