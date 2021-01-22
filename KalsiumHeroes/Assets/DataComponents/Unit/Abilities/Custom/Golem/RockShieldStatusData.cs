using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockShieldStatusData), menuName = "DataSources/Units/Golem/" + nameof(RockShieldStatusData))]
public class RockShieldStatusData : StatusData {

	public override Type ownerConstraint => typeof(RockShieldStatus);

	public Attribute<int> shieldHP;
	public UnitModifierData statusModifier;
	public Attribute<int> explosionRadius;
}
