
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

public delegate bool Pather(Tile from, Edge edge, Tile to);

public static class Pathers {

	/// <summary> Returns true if you can advance forward over the Edge.</summary>
	public static bool OneWay(Tile from, Edge edge, Tile to) {
		if (to == null) return false;
		return edge.IsPassable(from, to);
	}

	/// <summary> Returns true if you can advance forward over the Edge and disallows advancing to Tiles with an Unit.</summary>
	public static bool OneWayUnitBlocking(Tile from, Edge edge, Tile to) {
		return to.unit == null && OneWay(from, edge, to);
	}

	/// <summary> Returns true if you can advance forward AND backwards over the Edge.</summary>
	public static bool BothWays(Tile from, Edge edge, Tile to) {
		return OneWay(from, edge, to) && OneWay(to, edge, from);
	}

	/// <summary> Returns true if you can advance forward OR backwards over the Edge.</summary>
	public static bool EitherWay(Tile from, Edge edge, Tile to) {
		return OneWay(from, edge, to) || OneWay(to, edge, from);
	}

}

