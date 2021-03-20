using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(AreaDamageAbilityData), menuName = "DataSources/Abilities/" + nameof(AreaDamageAbilityData))]
public class AreaDamageAbilityData : TileTargetAbilityData {

	public override Type createTypeConstraint => typeof(AreaDamageAbility);

	public Attribute<float> centerDamage;

	public Attribute<float> outerDamage;

	public DamageType damageType;
}
