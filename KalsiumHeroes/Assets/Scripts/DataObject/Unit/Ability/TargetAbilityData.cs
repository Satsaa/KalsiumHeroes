

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TargetAbilityData), menuName = "DataSources/" + nameof(TargetAbilityData))]
public abstract class TargetAbilityData : AbilityData {

	public override Type createTypeConstraint => typeof(TargetAbility);

	[Tooltip("Cast range of the ability.")]
	public Range range;

	[Tooltip("How the range is determined.")]
	public RangeMode rangeMode;

	[Tooltip("Only directly visible Tiles are valid in range?")]
	public RequiresVision requiresVision;

	[Serializable]
	public class Range : ToggleAttribute<int> {
		Range() : base(1) { }
		public override string identifier => "Attribute_TargetAbility_Range";
		public override string Format(bool isSource) => enabled ? Lang.GetStr("Infinite") : base.Format(isSource);
	}

	[Serializable]
	public class RequiresVision : Attribute<bool> {
		RequiresVision() : base(false) { }
		public override string identifier => "Attribute_TargetAbility_RequiresVision";
		public override string TooltipText(IAttribute source) => current ? null : DefaultTooltip(source);
	}

}
