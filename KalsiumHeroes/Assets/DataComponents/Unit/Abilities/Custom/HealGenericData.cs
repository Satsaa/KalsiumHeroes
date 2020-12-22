using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HealGenericData), menuName = "DataSources/" + nameof(HealGenericData))]
public class HealGenericData : AbilityData {

	public override Type componentTypeConstraint => typeof(HealGenericAbility);

	[Header("Generic Heal Ability Data")]

	public Attribute<float> heal;

}
