using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MarkOfPreyStatusData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(MarkOfPreyStatusData))]
public class MarkOfPreyStatusData : StatusData
{
    public override Type componentConstraint => typeof(MarkOfPreyStatus);
}
