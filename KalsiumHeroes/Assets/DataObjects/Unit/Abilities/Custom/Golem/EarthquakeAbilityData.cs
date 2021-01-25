using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EarthquakeAbilityData), menuName = "DataSources/Units/Golem/" + nameof(EarthquakeAbilityData))]
public class EarthquakeAbilityData : NoTargetAbilityData {

	public override Type createTypeConstraint => typeof(EarthquakeAbility);

	[Header("Global Attributes")]
	public DamageType damageType;

	[Header("Range 1 Attributes")]
	public Attribute<float> damage1;
	public ModifierData rootModifier;

	[Header("Range 2 Attributes")]
	public Attribute<float> damage2;
	public ModifierData slowModifier2;

	[Header("Range 3 Attributes")]
	public Attribute<float> damage3;
	public ModifierData slowModifier3;
}
