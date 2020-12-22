using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HealAbilityData), menuName = "DataSources/" + nameof(HealAbilityData))]
public class HealAbilityData : AbilityData {

	public override Type componentConstraint => typeof(HealAbility);

	[Header("Heal Ability Data")]

	public Attribute<float> heal;

}
