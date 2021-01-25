using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HealAbilityData), menuName = "DataSources/Abilities/" + nameof(HealAbilityData))]
public class HealAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(HealAbility);

	public Attribute<float> heal;

}
