
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using HexGrid;
using System;
using Priority_Queue;
using UnityEngine.Serialization;

public delegate float CostCalculator(Tile from, Edge edge, Tile to);

public static class CostCalculators {

	public static CostCalculator Reverse(CostCalculator pather) {
		return (Tile from, Edge edge, Tile to) => pather(to, edge, from);
	}

	/// <summary> Returns the default cost to move over the edge to a tile.</summary>
	public static float MoveCost(Tile from, Edge edge, Tile to) {
		return edge.MoveCost(from, to);
	}

	/// <summary> Always gives 1.</summary>
	public static float Distance(Tile from, Edge edge, Tile to) {
		return 1;
	}
}

