using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MovementChangeStatus : Status
{
    public MovementChangeStatusData statusData => (MovementChangeStatusData)base.data;

    public override Type dataType => typeof(MovementChangeStatusData);

    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.data.movement.ConfigureAlterer(add, v => v + statusData.movementChange.value);
    }
}
