
public delegate bool UnitPather(Unit unit, Tile from, Edge edge, Tile to);

public static class UnitPathers {

	public static UnitPather For(RangeMode rangeMode) {
		return (rangeMode) switch {
			RangeMode.Distance => UnitPathers.FlyingPhased,

			RangeMode.PathDistance => UnitPathers.Unphased,
			RangeMode.PathDistancePhased => UnitPathers.Phased,
			RangeMode.PathDistanceFlying => UnitPathers.Flying,
			RangeMode.PathDistancePhasedFlying => UnitPathers.FlyingPhased,

			RangeMode.PathCost => UnitPathers.Unphased,
			RangeMode.PathCostPhased => UnitPathers.Phased,
			RangeMode.PathCostFlying => UnitPathers.Flying,
			RangeMode.PathCostPhasedFlying => UnitPathers.FlyingPhased,

			_ => UnitPathers.FlyingPhased
		};
	}

	public static UnitPather Reverse(UnitPather pather) {
		return (Unit unit, Tile from, Edge edge, Tile to) => pather(unit, to, edge, from);
	}


	/// <summary> Returns true if you can advance forward over the Edge.</summary>
	public static bool Phased(Unit unit, Tile from, Edge edge, Tile to) {
		return edge.CanPass(unit, from, to);
	}

	/// <summary> Returns true if you can advance forward over the Edge and disallows advancing to Tiles with a Unit.</summary>
	public static bool Unphased(Unit unit, Tile from, Edge edge, Tile to) {
		return !to.hasUnits && Phased(unit, from, edge, to);
	}

	/// <summary> Returns true if you can advance forward AND backwards over the Edge.</summary>
	public static bool BothWays(Unit unit, Tile from, Edge edge, Tile to) {
		return Phased(unit, from, edge, to) && Phased(unit, to, edge, from);
	}

	/// <summary> Returns true if you can advance forward OR backwards over the Edge.</summary>
	public static bool EitherWay(Unit unit, Tile from, Edge edge, Tile to) {
		return Phased(unit, from, edge, to) || Phased(unit, to, edge, from);
	}

	/// <summary> Returns true when there is no unit at Tile "to".</summary>
	public static bool Flying(Unit unit, Tile from, Edge edge, Tile to) {
		return !to.hasUnits;
	}

	/// <summary> Returns true always (TM).</summary>
	public static bool FlyingPhased(Unit unit, Tile from, Edge edge, Tile to) {
		return true;
	}

}
