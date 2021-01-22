using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HuntTheMarkAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(HuntTheMarkAbilityData))]
public class HuntTheMarkAbilityData : AbilityData
{
    public override Type componentConstraint => typeof(HuntTheMarkAbility);

    [Header("Hunt The Mark Ability Attributes")]
    public Attribute<float> normalDamage;
    public Attribute<float> silenceDamage;
    public DamageType damageType;
}
