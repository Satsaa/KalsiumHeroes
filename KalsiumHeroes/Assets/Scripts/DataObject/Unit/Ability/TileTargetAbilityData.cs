

using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(TileTargetAbilityData), menuName = "DataSources/" + nameof(TileTargetAbilityData))]
public class TileTargetAbilityData : TargetAbilityData {

	public override Type createTypeConstraint => typeof(TileTargetAbility);

	[Tooltip("Radius of the affected tiles around the target.")]
	public Attribute<int> radius = new Attribute<int>(0);

	[Tooltip("Types of valid targets.")]
	public TileTargetType targetType;

}
