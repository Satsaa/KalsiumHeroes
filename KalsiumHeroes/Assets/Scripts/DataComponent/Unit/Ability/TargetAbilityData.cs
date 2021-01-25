

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TargetAbilityData), menuName = "DataSources/" + nameof(TargetAbilityData))]
public abstract class TargetAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(TargetAbility);

	[Tooltip("Cast range of the ability.")]
	public ToggleAttribute<int> range = new ToggleAttribute<int>(1, true);

	[Tooltip("How the range is determined.")]
	public RangeMode rangeMode;

	[Tooltip("Only directly visible Tiles are valid in range?")]
	public Attribute<bool> requiresVision = new Attribute<bool>(false);

}
