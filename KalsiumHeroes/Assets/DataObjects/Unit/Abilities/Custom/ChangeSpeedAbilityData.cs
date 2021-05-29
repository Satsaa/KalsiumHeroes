using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ChangeSpeedAbilityData), menuName = "DataSources/Abilities/" + nameof(ChangeSpeedAbilityData))]
public class ChangeSpeedAbilityData : UnitTargetAbilityData {

	public override Type createTypeConstraint => typeof(ChangeSpeedAbility);

	public Attribute<int> speedChange;

}
