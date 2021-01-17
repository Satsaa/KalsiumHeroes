using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class MoveCostEdgeModifier : EdgeModifier, IOnGetMoveCost_Edge {

	public new MoveCostEdgeModifierData data => (MoveCostEdgeModifierData)base.data;
	public override Type dataType => typeof(MoveCostEdgeModifierData);

	public float OnGetMoveCost(Tile from, Tile to, float current) {
		return current + data.additionalMoveCost.value;
	}

	public float OnGetMoveCost(Unit unit, Tile from, Tile to, float current) {
		return current + data.additionalMoveCost.value;
	}
}
