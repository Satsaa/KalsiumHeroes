
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

	/// <summary> Returns the default cost to move over the edge.</summary>
	public static float Default(Tile from, Edge edge, Tile to) {
		return edge.MoveCost(from, to);
	}

}

