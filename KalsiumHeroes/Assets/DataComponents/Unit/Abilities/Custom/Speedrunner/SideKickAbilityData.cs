using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SideKickAbilityData), menuName = "DataSources/Units/Speedrunner/" + nameof(SideKickAbilityData))]
public class SideKickAbilityData : AbilityData {

	public override Type ownerConstraint => typeof(SideKickAbility);

	public Attribute<float> movementDamageMultiplier;

	public Attribute<float> speedDamageMultiplier;

	public DamageType damageType;
}
