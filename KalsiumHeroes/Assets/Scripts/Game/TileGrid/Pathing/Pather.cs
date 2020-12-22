
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

	public static Pather For(RangeMode rangeMode) {
		return (rangeMode) switch {
			RangeMode.Distance => Pathers.FlyingPhased,

			RangeMode.PathDistance => Pathers.Unphased,
			RangeMode.PathDistancePhased => Pathers.Phased,
			RangeMode.PathDistanceFlying => Pathers.Flying,
			RangeMode.PathDistancePhasedFlying => Pathers.FlyingPhased,

			RangeMode.PathCost => Pathers.Unphased,
			RangeMode.PathCostPhased => Pathers.Phased,
			RangeMode.PathCostFlying => Pathers.Flying,
			RangeMode.PathCostPhasedFlying => Pathers.FlyingPhased,

			_ => Pathers.FlyingPhased
		};
	}

	public static Pather Reverse(Pather pather) {
		return (Tile from, Edge edge, Tile to) => pather(to, edge, from);
	}


	/// <summary> Returns true if you can advance forward over the Edge.</summary>
	public static bool Phased(Tile from, Edge edge, Tile to) {
		return edge.IsPassable(from, to);
	}

	/// <summary> Returns true if you can advance forward over the Edge and disallows advancing to Tiles with an Unit.</summary>
	public static bool Unphased(Tile from, Edge edge, Tile to) {
		return to.unit == null && Phased(from, edge, to);
	}

	/// <summary> Returns true if you can advance forward AND backwards over the Edge.</summary>
	public static bool BothWays(Tile from, Edge edge, Tile to) {
		return Phased(from, edge, to) && Phased(to, edge, from);
	}

	/// <summary> Returns true if you can advance forward OR backwards over the Edge.</summary>
	public static bool EitherWay(Tile from, Edge edge, Tile to) {
		return Phased(from, edge, to) || Phased(to, edge, from);
	}

	/// <summary> Returns true when there is no unit at Tile "to".</summary>
	public static bool Flying(Tile from, Edge edge, Tile to) {
		return to.unit == null;
	}

	/// <summary> Returns true always (TM).</summary>
	public static bool FlyingPhased(Tile from, Edge edge, Tile to) {
		return true;
	}


}

