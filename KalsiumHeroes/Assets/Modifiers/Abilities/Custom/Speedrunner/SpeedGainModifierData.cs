using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainModifierData), menuName = "DataSources/Units/Speedrunner/" + nameof(SpeedGainModifierData))]
public class SpeedGainModifierData : StatusEffectData
{
    public Attribute<int> speedGain;
    public Attribute<int> movementGain;
}
