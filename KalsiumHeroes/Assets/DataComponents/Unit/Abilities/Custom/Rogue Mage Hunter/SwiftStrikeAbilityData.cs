using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SwiftStrikeAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(SwiftStrikeAbilityData))]
public class SwiftStrikeAbilityData : AbilityData
{
    public override Type componentConstraint => typeof(SwiftStrikeAbility);

    [Header("Swift Strike Ability Attributes")]
    public Attribute<float> damage;
    public DamageType damageType;
}
