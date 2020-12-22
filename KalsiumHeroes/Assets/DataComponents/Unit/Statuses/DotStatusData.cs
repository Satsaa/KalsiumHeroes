
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotStatusData), menuName = "DataSources/" + nameof(DotStatusData))]
public class DotStatusData : StatusData {

	public override Type componentConstraint => typeof(DotStatus);

	[Header("Dot Test Data")]

	public Attribute<float> damage;

	public DamageType damageType;

}
