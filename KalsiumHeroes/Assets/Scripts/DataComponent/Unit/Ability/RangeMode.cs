﻿
using UnityEngine;

public enum RangeMode {
	[Tooltip("Range is determined raw distance from source to target.")]
	Distance,
	[Tooltip("Range is determined by shortest path distance from source to target. Walls and other units are considered inpassable.")]
	PathDistance,
	[Tooltip("Range is determined by shortest path move cost from source to target. Only walls are considered inpassable.")]
	PathDistancePassThrough,
	[Tooltip("Range is determined by shortest path move cost from source to target. Walls and other units are considered inpassable.")]
	PathCost,
	[Tooltip("Range is determined by shortest path move cost from source to target. Only walls are considered inpassable.")]
	PathCostPassThrough,
}