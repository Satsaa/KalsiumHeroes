using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class MoveCostEdgeModifier : EdgeModifier {

	public MoveCostEdgeModifierData moveCostEdgeModifierData => (MoveCostEdgeModifierData)data;
	public override Type dataType => typeof(MoveCostEdgeModifierData);

	public override float MoveCost(Tile from, Tile to, float current) {
		return current + moveCostEdgeModifierData.additionalMoveCost.value;
	}

}
