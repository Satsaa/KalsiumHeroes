

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileTargetAbilityData), menuName = "DataSources/" + nameof(TileTargetAbilityData))]
public class TileTargetAbilityData : TargetAbilityData {

	public override Type createTypeConstraint => typeof(TileTargetAbility);

	[Tooltip("Types of valid targets.")]
	public TileTargetType targetType;

	[Tooltip("Radius of the affected tiles around the target.")]
	public Radius radius;

	public class Radius : Attribute<int> {
		public override string identifier => "Attribute_TileTargetAbility_Radius";
		public override string TooltipText(IAttribute source) => value == 0 ? null : DefaultTooltip(source);
	}
}
