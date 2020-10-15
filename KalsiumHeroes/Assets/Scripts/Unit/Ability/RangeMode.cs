
using UnityEngine;

public enum RangeMode {
  [Tooltip("Range is determined raw distance from source to target.")]
  Distance,
  [Tooltip("Range is determined by shortest path distance from source to target. Blocked areas are not included in range.")]
  PathDistance,
  [Tooltip("Range is determined by shortest path move cost from source to target. Blocked areas are not included in range.")]
  PathCost,
}
