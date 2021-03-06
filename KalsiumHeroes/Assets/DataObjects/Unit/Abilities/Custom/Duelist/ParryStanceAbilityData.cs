using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ParryStanceAbilityData), menuName = "DataSources/Units/Duelist/" + nameof(ParryStanceAbilityData))]
public class ParryStanceAbilityData : NoTargetAbilityData {

	public override Type createTypeConstraint => typeof(ParryStanceAbility);

	public ModifierData statusModifier;
}
