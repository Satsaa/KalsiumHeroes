using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseChangeStatus : Status {
    public DefenseChangeStatusData defenseReductionStatusData => (DefenseChangeStatusData)data;

    public override Type dataType => typeof(DefenseChangeStatusData);

    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.data.defense.ConfigureAlterer(add, v => v - defenseReductionStatusData.defenseReduction.value);
    }
}
