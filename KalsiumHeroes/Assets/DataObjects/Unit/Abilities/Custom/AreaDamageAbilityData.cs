using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AreaDamageAbilityData), menuName = "DataSources/Abilities/" + nameof(AreaDamageAbilityData))]
public class AreaDamageAbilityData : TileTargetAbilityData {

	public override Type createTypeConstraint => typeof(AreaDamageAbility);

	public Attribute<float> primaryDamage;

	public Attribute<float> secondaryDamage;

	public DamageType damageType;
}
