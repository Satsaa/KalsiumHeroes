using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = nameof(RockShieldStatusData), menuName = "DataSources/Units/Golem/" + nameof(RockShieldStatusData))]
public class RockShieldStatusData : StatusData {

	public override Type createTypeConstraint => typeof(RockShieldStatus);

	public Attribute<int> shieldHP;

	public UnitModifierData modifier;

	public Attribute<int> explosionRadius;
}
