using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockShieldAbilityData), menuName = "DataSources/Units/Golem/" + nameof(RockShieldAbilityData))]
public class RockShieldAbilityData : NoTargetAbilityData {

	public override Type createTypeConstraint => typeof(RockShieldAbility);

	public UnitModifierData rockShieldModifier;
}
