﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ChangeEnergyAbilityData), menuName = "DataSources/Abilities/" + nameof(ChangeEnergyAbilityData))]
public class ChangeEnergyAbilityData : AbilityData {

	public override Type componentConstraint => typeof(ChangeEnergyAbility);

	[Header("Change Energy Ability Data")]

	public Attribute<int> energyChange;

}
