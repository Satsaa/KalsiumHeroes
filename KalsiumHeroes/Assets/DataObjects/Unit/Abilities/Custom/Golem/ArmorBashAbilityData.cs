using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ArmorBashAbilityData), menuName = "DataSources/Units/Golem/" + nameof(ArmorBashAbilityData))]
public class ArmorBashAbilityData : UnitTargetAbilityData {
	public override Type createTypeConstraint => typeof(ArmorBashAbility);

	public DamageType damageType;
}
