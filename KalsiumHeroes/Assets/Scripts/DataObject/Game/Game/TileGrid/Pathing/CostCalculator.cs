

public delegate float CostCalculator(Tile from, Edge edge, Tile to);

public static class CostCalculators {

	public static CostCalculator For(RangeMode rangeMode) {
		return (rangeMode) switch {
			RangeMode.Distance => CostCalculators.Distance,

			RangeMode.PathDistance => CostCalculators.Distance,
			RangeMode.PathDistancePhased => CostCalculators.Distance,
			RangeMode.PathDistanceFlying => CostCalculators.Distance,
			RangeMode.PathDistancePhasedFlying => CostCalculators.Distance,

			RangeMode.PathCost => CostCalculators.MoveCost,
			RangeMode.PathCostPhased => CostCalculators.MoveCost,
			RangeMode.PathCostFlying => CostCalculators.MoveCost,
			RangeMode.PathCostPhasedFlying => CostCalculators.MoveCost,

			_ => CostCalculators.Distance,
		};
	}

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

