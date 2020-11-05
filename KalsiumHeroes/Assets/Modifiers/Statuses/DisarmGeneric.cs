using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisarmGeneric : StatusEffect
{
    public DisarmGenericData disarmGenericData => (DisarmGenericData)data;

    public override Type dataType => typeof(DisarmGenericData);

    public override void OnTurnStart() {
        unit.disarmed.value = true;
        base.OnTurnStart();
    }

    public override void OnTurnEnd() {
        unit.disarmed.value = false;
        base.OnTurnEnd();
    }
}
