using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ParryStanceAbilityData), menuName = "DataSources/Units/Duelist/" + nameof(ParryStanceAbilityData))]
public class ParryStanceAbilityData : AbilityData {

	public override Type ownerConstraint => typeof(ParryStanceAbility);

	[Header("Parry Stance Data")]

	public ModifierData statusModifier;
}
