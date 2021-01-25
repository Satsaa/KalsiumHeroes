

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(UnitTargetAbilityData), menuName = "DataSources/" + nameof(UnitTargetAbilityData))]
public class UnitTargetAbilityData : TargetAbilityData {

	public override Type createTypeConstraint => typeof(UnitTargetAbility);

	[Tooltip("Types of valid targets.")]
	public UnitTargetType targetType;

}
