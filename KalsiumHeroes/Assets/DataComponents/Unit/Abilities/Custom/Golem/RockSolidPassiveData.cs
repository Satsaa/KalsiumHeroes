using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockSolidPassiveData), menuName = "DataSources/Units/Golem/" + nameof(RockSolidPassiveData))]
public class RockSolidPassiveData : PassiveData {
	public override Type ownerConstraint => typeof(RockSolidPassive);

	[Header("Rock Solid Passive Data")]
	[Tooltip("Increase Defense and Resistance from units in range of 1")]
	public Attribute<int> increase1;
	[Tooltip("Increase Defense and Resistance from units in range of 2")]
	public Attribute<int> increase2;
	[Tooltip("Increase Defense and Resistance from units in range of 3")]
	public Attribute<int> increase3;
}
