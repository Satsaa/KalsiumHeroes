using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ShoveAbilityData), menuName = "DataSources/Units/Lone Ranger/" + nameof(ShoveAbilityData))]
public class ShoveAbilityData : AbilityData {

	public override Type componentConstraint => typeof(ShoveAbility);

	[Header("Shove Ability Data")]
	[Tooltip("This is the root modifier the ability gives the targeted unit")]
	public DataComponentData rootModifier;
}
