using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(SwiftStrikeAbilityData), menuName = "DataSources/Units/Rogue Mage Hunter/" + nameof(SwiftStrikeAbilityData))]
public class SwiftStrikeAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(SwiftStrikeAbility);

	public Attribute<float> damage;
	public DamageType damageType;
}
