using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisgracefulBehaviourStatus : Status
{
    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.silenced.ConfigureAlterer(add, v => true);
        unit.disarmed.ConfigureAlterer(add, v => true);
    }
}
