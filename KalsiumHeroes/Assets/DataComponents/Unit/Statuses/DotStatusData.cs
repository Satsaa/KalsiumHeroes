
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotStatusData), menuName = "DataSources/Statuses/" + nameof(DotStatusData))]
public class DotStatusData : StatusData {

	public override Type ownerConstraint => typeof(DotStatus);

	public Attribute<float> damage;

	public DamageType damageType;

}
