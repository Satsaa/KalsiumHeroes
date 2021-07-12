using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class MoveCostEdgeModifier : EdgeModifier, IOnGetMoveCost_Edge {

	public new MoveCostEdgeModifierData data => (MoveCostEdgeModifierData)_data;
	public override Type dataType => typeof(MoveCostEdgeModifierData);

	public void OnGetMoveCost(Tile from, Tile to, ref float cost) {
		cost += data.additionalMoveCost.current;
	}

	public void OnGetMoveCost(Unit unit, Tile from, Tile to, ref float cost) {
		cost += data.additionalMoveCost.current;
	}
}
