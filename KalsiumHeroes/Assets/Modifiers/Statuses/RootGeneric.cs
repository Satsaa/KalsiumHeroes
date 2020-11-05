using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootGeneric : StatusEffect
{
    Attribute<int> normalMovement;
    public RootGenericData rootGenericData => (RootGenericData)data;

    public override Type dataType => typeof(RootGenericData);

    protected override void OnRegisterAlterers()
    {
        // Removed automatically when the component is destroyed
        unit.unitData.movement.RegisterAlterer(v => 0);
    }

}
