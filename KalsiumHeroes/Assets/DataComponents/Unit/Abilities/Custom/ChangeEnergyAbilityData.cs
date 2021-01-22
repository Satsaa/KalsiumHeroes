using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ChangeEnergyAbilityData), menuName = "DataSources/Abilities/" + nameof(ChangeEnergyAbilityData))]
public class ChangeEnergyAbilityData : AbilityData {

	public override Type ownerConstraint => typeof(ChangeEnergyAbility);

	public Attribute<int> energyChange;

}
