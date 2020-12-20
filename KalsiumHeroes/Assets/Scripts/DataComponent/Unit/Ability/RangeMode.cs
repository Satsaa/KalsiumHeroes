
using UnityEngine;

public enum RangeMode {
	[Tooltip("Range is determined raw distance from source to target.")]
	Distance,

	[Tooltip("Range is determined by shortest path distance from source to target. Walls and other Units are considered impassable.")]
	PathDistance,
	[Tooltip("Range is determined by shortest path distance from source to target. Only walls are considered impassable.")]
	PathDistancePhased,
	[Tooltip("Range is determined by shortest path distance from source to target. Only Units are considered impassable.")]
	PathDistanceFlying,
	[Tooltip("Range is determined by shortest path distance from source to target. Nothing is impassable.")]
	PathDistancePhasedFlying,

	[Tooltip("Range is determined by shortest path move cost from source to target. Walls and other Units are considered impassable.")]
	PathCost,
	[Tooltip("Range is determined by shortest path move cost from source to target. Only walls are considered impassable.")]
	PathCostPhased,
	[Tooltip("Range is determined by shortest path move cost from source to target. Only Units are considered impassable.")]
	PathCostFlying,
	[Tooltip("Range is determined by shortest path move cost from source to target. Nothing is impassable.")]
	PathCostPhasedFlying,
}
