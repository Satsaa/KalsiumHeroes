using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(MoveCostEdgeModifier), menuName = "KalsiumHeroes/EdgeModifier/" + nameof(MoveCostEdgeModifier))]
public class MoveCostEdgeModifier : EdgeModifier, IOnGetMoveCost_Edge {

	[Tooltip("Aditional move cost.")]
	public Attribute<float> moveCostChange = new(1);


	public void OnGetMoveCost(Tile from, Tile to, ref float cost) {
		cost += moveCostChange.current;
	}

	public void OnGetMoveCost(Unit unit, Tile from, Tile to, ref float cost) {
		cost += moveCostChange.current;
	}
}
