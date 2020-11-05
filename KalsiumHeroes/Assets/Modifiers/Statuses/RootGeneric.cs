using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootGeneric : StatusEffect
{
    Attribute<int> normalMovement;
    public RootGenericData rootGenericData => (RootGenericData)data;

    public override Type dataType => typeof(RootGenericData);

    public override void OnAdd() {
        normalMovement = unit.unitData.movement;
        base.OnAdd();
    }
    public override void OnTurnStart() {
        unit.unitData.movement.value = 0;
        base.OnTurnStart();
    }

    public override void OnTurnEnd() {
        unit.unitData.movement = normalMovement;
        base.OnTurnEnd();
    }
}
