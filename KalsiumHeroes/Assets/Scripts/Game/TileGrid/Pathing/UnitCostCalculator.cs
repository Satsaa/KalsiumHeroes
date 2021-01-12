

public delegate float UnitCostCalculator(Unit unit, Tile from, Edge edge, Tile to);

public static class UnitCostCalculators {

	public static UnitCostCalculator For(RangeMode rangeMode) {
		return (rangeMode) switch {
			RangeMode.Distance => UnitCostCalculators.Distance,

			RangeMode.PathDistance => UnitCostCalculators.Distance,
			RangeMode.PathDistancePhased => UnitCostCalculators.Distance,
			RangeMode.PathDistanceFlying => UnitCostCalculators.Distance,
			RangeMode.PathDistancePhasedFlying => UnitCostCalculators.Distance,

			RangeMode.PathCost => UnitCostCalculators.MoveCost,
			RangeMode.PathCostPhased => UnitCostCalculators.MoveCost,
			RangeMode.PathCostFlying => UnitCostCalculators.MoveCost,
			RangeMode.PathCostPhasedFlying => UnitCostCalculators.MoveCost,

			_ => UnitCostCalculators.Distance,
		};
	}

	public static UnitCostCalculator Reverse(UnitCostCalculator pather) {
		return (Unit unit, Tile from, Edge edge, Tile to) => pather(unit, to, edge, from);
	}

	/// <summary> Returns the default cost to move over the edge to a tile.</summary>
	public static float MoveCost(Unit unit, Tile from, Edge edge, Tile to) {
		return edge.MoveCost(unit, from, to);
	}

	/// <summary> Always gives 1.</summary>
	public static float Distance(Unit unit, Tile from, Edge edge, Tile to) {
		return 1;
	}
}

