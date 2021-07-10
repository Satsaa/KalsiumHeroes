using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainAbilityData), menuName = "DataSources/Units/Speedrunner/" + nameof(SpeedGainAbilityData))]
public class SpeedGainAbilityData : NoTargetAbilityData {

	public override Type createTypeConstraint => typeof(SpeedGainAbility);

	[Tooltip("Radius of checked for other units")]
	public Attribute<int> radius = new(2);

	[Tooltip("This is the actual speed gain modifier gained from using the spell")]
	public ModifierData speedGainModifier;

	[Tooltip("Gain speed from units matching any of these unit target types.")]
	public UnitTargetType teamType;
}
