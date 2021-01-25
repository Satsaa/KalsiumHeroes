using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ShoveAbilityData), menuName = "DataSources/Units/Lone Ranger/" + nameof(ShoveAbilityData))]
public class ShoveAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(ShoveAbility);

	[Tooltip("This is the root modifier the ability gives the targeted unit")]
	public ModifierData rootModifier;

	[Tooltip("The maximum amount of tiles the target is shoved.")]
	public Attribute<int> shoveDist = new Attribute<int>(4);
}
