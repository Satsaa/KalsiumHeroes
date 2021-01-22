using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SpeedGainAbilityData), menuName = "DataSources/Units/Speedrunner/" + nameof(SpeedGainAbilityData))]
public class SpeedGainAbilityData : AbilityData {

	public override Type ownerConstraint => typeof(SpeedGainAbility);

	[Header("Speed Gain Ability Data")]
	[Tooltip("This is the actual speed gain modifier gained from using the spell")]
	public ModifierData speedGainModifier;
}
