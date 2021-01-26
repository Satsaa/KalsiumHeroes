using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(EarthquakeAbilityData), menuName = "DataSources/Units/Golem/" + nameof(EarthquakeAbilityData))]
public class EarthquakeAbilityData : NoTargetAbilityData {

	public override Type createTypeConstraint => typeof(EarthquakeAbility);

	public DamageType damageType;

	[Tooltip("Matching units get affected by this ability.")]
	public UnitTargetType affected;

	[Tooltip("Values per ring around caster.")]
	public RingValues[] rings;

	[Serializable]
	public class RingValues {
		public Attribute<float> damage;
		public ModifierData modifier;
	}
}
