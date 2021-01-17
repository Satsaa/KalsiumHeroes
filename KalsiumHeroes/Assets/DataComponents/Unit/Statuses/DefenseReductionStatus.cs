using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseReductionStatus : Status {
    public DefenseReductionStatusData defenseReductionStatusData => (DefenseReductionStatusData)data;

    public override Type dataType => typeof(DefenseReductionStatusData);

    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.unitData.defense.ConfigureAlterer(add, v => v - defenseReductionStatusData.defenseReduction.value);
    }
}
