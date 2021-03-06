using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(OpportunistPassiveData), menuName = "DataSources/Units/Duelist/" + nameof(OpportunistPassiveData))]
public class OpportunistPassiveData : PassiveData {
	public override Type createTypeConstraint => typeof(OpportunistPassive);

	public Attribute<float> damage;
	public DamageType damageType;
	public Attribute<int> range;

	public Attribute<int> energyGain;
}
