﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MoveCostEdgeModifierData), menuName = "DataSources/EdgeModifiers/" + nameof(MoveCostEdgeModifierData))]
public class MoveCostEdgeModifierData : EdgeModifierData {

	public override Type componentConstraint => typeof(MoveCostEdgeModifier);

	[Header("MoveCost Edge Modifier Data")]
	[Tooltip("Aditional move cost.")]
	public Attribute<float> additionalMoveCost = new Attribute<float>(1);

}
