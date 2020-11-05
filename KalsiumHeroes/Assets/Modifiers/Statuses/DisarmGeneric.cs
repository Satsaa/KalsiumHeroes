using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisarmGeneric : StatusEffect
{
    public DisarmGenericData disarmGenericData => (DisarmGenericData)data;

    public override Type dataType => typeof(DisarmGenericData);

    protected override void OnRegisterAlterers()
    {
        // Removed automatically when the component is destroyed
        unit.disarmed.RegisterAlterer(v => true);
    }

}
