
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotStatusData), menuName = "DataSources/Statuses/" + nameof(DotStatusData))]
public class DotStatusData : StatusData {

	public override Type ownerConstraint => typeof(DotStatus);

	[Header("Dot Test Data")]

	public Attribute<float> damage;

	public DamageType damageType;

}
