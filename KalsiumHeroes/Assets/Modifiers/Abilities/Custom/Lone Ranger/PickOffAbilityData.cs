using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PickOffAbilityData), menuName = "DataSources/Units/Lone Ranger/" + nameof(PickOffAbilityData))]
public class PickOffAbilityData : AbilityData
{
    [Header("Pick Off Ability Data")]
    public Attribute<float> damage;
    [Tooltip("Set the array length to the range of the bonus damage, and then check how the damage grows. 0 = first tile")]
    public float[] bonusDamageMultipliers;
    public DamageType damageType;
}
