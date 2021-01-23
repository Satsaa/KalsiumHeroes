using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(OverHeadUIModifierData), menuName = "DataSources/UnitModifiers/" + nameof(OverHeadUIModifierData))]
public class OverHeadUIModifierData : UnitModifierData {

	public override Type createTypeConstraint => typeof(OverHeadUIModifier);

	[Tooltip("World space offset or the UI")]
	public Vector3 wsOffset;
	[Tooltip("Screen space offset or the UI")]
	public Vector2 ssOffset;

}
