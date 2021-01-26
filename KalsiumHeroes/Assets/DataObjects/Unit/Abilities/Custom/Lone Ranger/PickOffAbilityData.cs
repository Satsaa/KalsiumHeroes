using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(PickOffAbilityData), menuName = "DataSources/Units/Lone Ranger/" + nameof(PickOffAbilityData))]
public class PickOffAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(PickOffAbility);

	public Attribute<float> damage;

	public DamageType damageType;

	[Tooltip("Only units matching any of these unit target types affect the multiplier.")]
	public UnitTargetType multiplyingTypes;

	[FormerlySerializedAs("bonusDamageMultipliers")]
	[Tooltip("Damage multipliers based on the distance of the nearest unit of the target.")]
	public float[] multipliers;

}
