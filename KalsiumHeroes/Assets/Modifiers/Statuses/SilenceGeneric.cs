using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceGeneric : StatusEffect
{
    public SilenceGenericData silenceGenericData => (SilenceGenericData)data;

    public override Type dataType => typeof(SilenceGenericData);

    protected override void OnRegisterAlterers()
    {
        // Removed automatically when the component is destroyed
        unit.silenced.RegisterAlterer(v => true);
    }

}