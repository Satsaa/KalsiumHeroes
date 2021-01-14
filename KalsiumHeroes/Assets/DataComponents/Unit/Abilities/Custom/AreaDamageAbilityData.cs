using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AreaDamageAbilityData), menuName = "DataSources/Abilities/" + nameof(AreaDamageAbilityData))]
public class AreaDamageAbilityData : AbilityData {

	public override Type componentConstraint => typeof(AreaDamageAbility);

	[Header("Area Damage Ability Data")]

	public Attribute<float> primaryDamage;

	public Attribute<float> secondaryDamage;

	public DamageType damageType;
}
