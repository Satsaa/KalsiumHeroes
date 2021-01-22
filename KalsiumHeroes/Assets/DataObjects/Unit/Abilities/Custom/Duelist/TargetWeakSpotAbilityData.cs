using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TargetWeakSpotAbilityData), menuName = "DataSources/Units/Duelist/" + nameof(TargetWeakSpotAbilityData))]
public class TargetWeakSpotAbilityData : AbilityData {
	public override Type createTypeConstraint => typeof(TargetWeakSpotAbility);

	public Attribute<float> damage;
	public Attribute<float> defenseReduction;

	public DamageType damageType;

	public ModifierData defenseReductionModifier;
}
