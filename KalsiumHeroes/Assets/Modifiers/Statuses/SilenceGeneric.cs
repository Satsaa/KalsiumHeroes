using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilenceGeneric : StatusEffect
{
    public SilenceGenericData silenceGenericData => (SilenceGenericData)data;

    public override Type dataType => typeof(SilenceGenericData);

    public override void OnTurnStart() {
        unit.silenced.value = true;
        base.OnTurnStart();
    }

    public override void OnTurnEnd() {
        unit.silenced.value = false;
        base.OnTurnEnd();
    }
}
