using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ParryStanceStatusData), menuName = "DataSources/Units/Duelist/" + nameof(ParryStanceStatusData))]
public class ParryStanceStatusData : StatusData {
	public override Type ownerConstraint => typeof(ParryStanceStatus);

	[Header("Parry Stance Status Data")]
	public Attribute<int> defenseIncrease;
	public Attribute<float> damage;
	public DamageType damageType;
}
