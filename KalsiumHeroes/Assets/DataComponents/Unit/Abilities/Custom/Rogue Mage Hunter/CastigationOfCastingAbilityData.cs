using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(CastigationOfCastingAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(CastigationOfCastingAbilityData))]
public class CastigationOfCastingAbilityData : AbilityData
{
    public override Type componentConstraint => typeof(CastigationOfCastingAbility);

    [Header("Castigation of Casting Ability Attributes")]
    public DataComponentData markOfCastigationModifier;
}
