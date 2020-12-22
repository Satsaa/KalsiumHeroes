

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AbilityData), menuName = "DataSources/" + nameof(AbilityData))]
public class AbilityData : UnitModifierData {

	public override Type componentConstraint => typeof(Ability);

	[Header("Ability Data")]

	[Tooltip("Type of the ability.")]
	public AbilityType abilityType;

	[Tooltip("Types of valid targets.")]
	public TargetType targetType;

	[Tooltip("Cast range of the ability.")]
	public ToggleAttribute<int> range = new ToggleAttribute<int>(1, true);

	[Tooltip("How the range is determined.")]
	public RangeMode rangeMode;

	[Tooltip("Radius of the affected tiles around the target.")]
	public Attribute<int> radius = new Attribute<int>(0);

	[Tooltip("Only directly visible Tiles are valid in range?")]
	public Attribute<bool> requiresVision = new Attribute<bool>(false);

	[AttributeLabels("Current", "Max")]
	[Tooltip("How many turns it takes for this ability to gain a charge.")]
	public DualAttribute<int> cooldown = new DualAttribute<int>(0, 0);

	[AttributeLabels("Current", "Max")]
	[Tooltip("How many charges does the ability have.")]
	public DualAttribute<int> charges = new DualAttribute<int>(1, 1);

	[Tooltip("How many times can the ability be cast in total.")]
	public ToggleAttribute<int> uses = new ToggleAttribute<int>(false);

	[Tooltip("Can the ability be cast after other abilities?")]
	public Attribute<bool> alwaysCastable = new Attribute<bool>(false);

}
