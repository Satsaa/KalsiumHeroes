using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HuntTheMarkAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(HuntTheMarkAbilityData))]
public class HuntTheMarkAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(HuntTheMarkAbility);

	public Attribute<float> normalDamage;
	public Attribute<float> silenceDamage;
	public DamageType damageType;
}
