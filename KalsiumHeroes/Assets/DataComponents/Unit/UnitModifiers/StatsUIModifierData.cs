using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(StatsUIModifierData), menuName = "DataSources/UnitModifiers/" + nameof(StatsUIModifierData))]
public class StatsUIModifierData : UnitModifierData {

	public override Type ownerConstraint => typeof(StatsUIModifier);

	[Header("Stats UI Modifier Data. Include " + nameof(StatsUI) + " in the container!")]

	[Tooltip("World space offset or the UI")]
	public Vector3 wsOffset;
	[Tooltip("Screen space offset or the UI")]
	public Vector2 ssOffset;

}
