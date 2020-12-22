
using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotTestStatusData), menuName = "DataSources/" + nameof(DotTestStatusData))]
public class DotTestStatusData : StatusData {

	public override Type componentTypeConstraint => typeof(DotTestStatus);

	[Header("Dot Test Data")]

	public Attribute<float> damage;

	public DamageType damageType;

}
