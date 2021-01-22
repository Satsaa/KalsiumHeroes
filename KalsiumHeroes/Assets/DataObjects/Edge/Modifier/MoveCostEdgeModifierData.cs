using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MoveCostEdgeModifierData), menuName = "DataSources/EdgeModifiers/" + nameof(MoveCostEdgeModifierData))]
public class MoveCostEdgeModifierData : EdgeModifierData {

	public override Type createTypeConstraint => typeof(MoveCostEdgeModifier);

	[Tooltip("Aditional move cost.")]
	public Attribute<float> additionalMoveCost = new Attribute<float>(1);

}
