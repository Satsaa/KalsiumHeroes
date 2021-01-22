using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HealAbilityData), menuName = "DataSources/Abilities/" + nameof(HealAbilityData))]
public class HealAbilityData : AbilityData {

	public override Type ownerConstraint => typeof(HealAbility);

	[Header("Heal Ability Data")]

	public Attribute<float> heal;

}
