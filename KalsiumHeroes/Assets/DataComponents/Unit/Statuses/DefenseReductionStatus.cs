using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseReductionStatus : Status {

    public new DefenseReductionStatusData data => (DefenseReductionStatusData)base.data;
    public override Type dataType => typeof(DefenseReductionStatusData);

    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.data.defense.ConfigureAlterer(add, v => v - data.defenseReduction.value);
    }
}
