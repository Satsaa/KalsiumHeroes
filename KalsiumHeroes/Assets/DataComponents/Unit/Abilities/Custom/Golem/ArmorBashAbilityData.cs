using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ArmorBashAbilityData), menuName = "DataSources/Units/Golem/" + nameof(ArmorBashAbilityData))]
public class ArmorBashAbilityData : AbilityData {
	public override Type ownerConstraint => typeof(ArmorBashAbility);

	public DamageType damageType;
}
