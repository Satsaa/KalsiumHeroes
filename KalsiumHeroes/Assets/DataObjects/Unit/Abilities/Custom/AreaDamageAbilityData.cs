using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(AreaDamageAbilityData), menuName = "DataSources/Abilities/" + nameof(AreaDamageAbilityData))]
public class AreaDamageAbilityData : TileTargetAbilityData {

	public override Type createTypeConstraint => typeof(AreaDamageAbility);

	[FormerlySerializedAs("primaryDamage")]
	public Attribute<float> centerDamage;

	[FormerlySerializedAs("secondaryDamage")]
	public Attribute<float> outerDamage;

	public DamageType damageType;
}
