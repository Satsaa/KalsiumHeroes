
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

public class GameGrid : MonoBehaviour, ISerializationCallbackReceiver {

	[field: SerializeField]
	public Vector2Int size { get; private set; }

	[FormerlySerializedAs("tile")]
	public GameObject tilePrefab;

	public Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

	/// <summary>
	/// Builds a new grid with default values.
	/// </summary>
	public void ResetGrid() {

		DestroyTiles();

		for (int y = 0; y < size.y; y++) {
			int xOffset = Mathf.FloorToInt(y / 2);
			for (int x = -xOffset; x < size.x - xOffset; x++) {
				var hex = new Hex(x, y);

				// Cut right edge for symmetry
				var yOddRow = y / 2 + 1;
				if (y % 2 == 1 && x + yOddRow == size.x) continue;
#if UNITY_EDITOR
				var go = Application.isPlaying ? Instantiate(tilePrefab, transform) : (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, transform);
#else
       			var go = Instantiate(tilePrefab, transform);
#endif
				var comp = go.GetComponent<Tile>();
				if (comp == null) comp = go.AddComponent<Tile>();
				comp.Init(hex);
				tiles.Add(hex.pos, comp);
			}
		}

		foreach (var kv in tiles) {
			var tile = kv.Value;
			var hex = tile.hex;
			for (int i = 0; i < Hex.neighbors.Length; i++) {
				var offset = Hex.neighbors[i];
				var offsetHex = Hex.Add(hex, offset);
				if (tiles.TryGetValue(offsetHex.pos, out var neighbor))
					tile.SetNeighbor((Tile.Dir)i, neighbor);
			}
		}
	}

	public void DestroyTiles() {
		foreach (var tile in tiles.Values) {
			if (!tile) continue;
			if (!Application.isPlaying) DestroyImmediate(tile.gameObject);
			else Destroy(tile.gameObject);
		}
		tiles.Clear();
	}

	/// <summary>
	/// Iterates in a radius around a Tile.
	/// </summary>
	public IEnumerable<Tile> Radius(Tile tile, int radius) {
		for (int x = -radius; x <= radius; x++) {
			for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(+radius, -x + radius); y++) {
				var pos = tile.hex.pos + new Vector3Int(x, y, -x - y);
				if (tiles.TryGetValue(pos, out var res)) yield return res;
			}
		}
	}

	/// <summary>
	/// Iterates in a Ring around a Tile.
	/// </summary>
	public IEnumerable<Tile> Ring(Tile tile, int radius) {
		if (radius <= 0) {
			if (radius == 0) {
				yield return tile;
				yield break;
			}
			throw new ArgumentOutOfRangeException(nameof(radius));
		}

		var pos = tile.hex.pos + new Vector3Int(0, radius, -radius);
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < radius; j++) {
				if (tiles.TryGetValue(pos, out var res)) yield return res;
				pos = new Hex(pos).GetNeighbor(i).pos;
			}
		}
	}

	/// <summary>
	/// Return the hex distance between a and b.
	/// </summary>
	public int Distance(Tile a, Tile b) {
		return Hex.Distance(a.hex, b.hex);
	}

	/// <summary>
	/// Iterates in a direction until a null Tile is reached.
	/// </summary>
	public IEnumerable<Tile> Direction(Tile tile, Tile.Dir direction) {
		var current = tile;
		while (current != null) {
			yield return current;
			current = current.GetNeighbor(direction);
		}
	}

	/// <summary>
	/// Raycasts along a plane (normal is up) at planeHeight and returns the Tile at the raycast hit point.
	/// </summary>
	/// <param name="ray">The ray used for raycasting.</param>
	/// <param name="planeHeight">The height of the plane that is being raycasted.</param>
	/// <returns>The Tile at the raycast hit point, if any.</returns>
	public Tile RaycastTile(Ray ray, float planeHeight = 0) {
		var plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
		if (plane.Raycast(ray, out float enter)) {
			var point = (ray.origin + ray.direction * enter).xz();
			var hex = Layout.PixelToHex(point).Round();
			tiles.TryGetValue(hex.pos, out var res);
			return res;
		}
		return default;
	}

	/// <summary>
	/// Returns an IEnumerable that enumerates the Tiles in a line from start to end.
	/// </summary>
	/// <remarks>
	/// Tiles outside the grid will be returned as null.
	/// </remarks>
	/// <param name="start">Start Tile</param>
	/// <param name="end">End Tile</param>
	/// <param name="altNudge">Use alternate opposite nudge direction? Edge cases will be nudged the other way.</param>
	/// <returns>IEnumerable yielding the Tiles along the line.</returns>
	public IEnumerable<Tile> Line(Tile start, Tile end, bool altNudge = false) {
		var _start = start.hex;
		var _end = end.hex;
		int dist = Hex.Distance(_start, _end);
		var xNudge = altNudge ? -1e-06f : 1e-06f;
		var yNudge = altNudge ? -1e-06f : 1e-06f;
		var zNudge = altNudge ? +2e-06f : -2e-06f;
		FractHex startNudge = new FractHex(_start.x + xNudge, _start.y + yNudge, _start.z + zNudge);
		FractHex endNudge = new FractHex(_end.x + xNudge, _end.y + yNudge, _end.z + zNudge);
		for (int i = 0; i < dist; i++) {
			var hex = Hex.Lerp(startNudge, endNudge, 1f / dist * i);
			tiles.TryGetValue(hex.Round().pos, out var ghRes1);
			yield return ghRes1;
		}
		tiles.TryGetValue(_end.pos, out var ghRes2);
		yield return ghRes2;
	}

	/// <summary>
	/// Returns true if tile has semi-direct vision to target.
	/// </summary>
	/// <note> The tile and target is sampled and <c>Distance(tile, target) - 1</c> amount of samples are taken between tile and target. </note>
	/// <param name="tile">The sighting Tile</param>
	/// <param name="target">The sighted Tile</param>
	/// <param name="target">The sighted Tile</param>
	/// <param name="seeThrough">Predicate which determines if a Tile can be seen through.</param>
	/// <returns>Whether or not tile has sight of target</returns>
	public bool HasSight(Tile tile, Tile target, Predicate<Tile> seeThrough = null) {
		seeThrough = seeThrough ?? (h => !h.blocked);
		int nudge = 0;
		var lineA = Line(tile, target, false).GetEnumerator();
		var lineB = Line(tile, target, true).GetEnumerator();

		while (lineA.MoveNext() && lineB.MoveNext()) {
			var a = lineA.Current;
			var b = lineB.Current;
			switch (nudge) {
				case 1: {
						if (SeeThrough(a)) continue;
						else return false;
					}
				case 2: {
						if (SeeThrough(b)) continue;
						else return false;
					}
				default: {
						var aPass = SeeThrough(a);
						var bPass = SeeThrough(b);
						if (aPass) {
							if (!bPass) nudge = 1;
							continue;
						} else {
							nudge = 2;
							if (bPass) continue;
							else return false;
						}
					}
			}
		}
		return true;

		bool SeeThrough(Tile a) {
			return a != null && seeThrough(a);
		}
	}

	/// <summary>
	/// Iterates around tile and yields visible tiles.
	/// </summary>
	/// <param name="seeThrough">Predicate which determines if a Tile can be seen through.</param>
	public IEnumerable<Tile> Vision(Tile tile, int range, Predicate<Tile> seeThrough = null) {
		var radius = Radius(tile, range);
		var visible = new HashSet<Tile>();
		foreach (var radiusTile in radius) {
			if (HasSight(tile, radiusTile, seeThrough) && visible.Add(radiusTile)) yield return radiusTile;
		}
	}

	/// <summary>
	/// Finds blocked of areas and returns the area id for all Tiles.
	/// </summary>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>Dictionary of Tile areas. Key: Tile, Value: Area id</returns>
	public Dictionary<Tile, int> GetAreas(Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var results = new Dictionary<Tile, int>(tiles.Count);
		var id = 0;

		foreach (var tile in tiles.Values) {
			if (!passable(tile) || results.ContainsKey(tile)) continue;
			foreach (var flooded in Flood(tile)) {
				results.Add(flooded, id);
			}
			id++;
		}
		return results;
	}

	/// <summary>
	/// Iterates the passable Tiles around source.
	/// </summary>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	public IEnumerable<Tile> Flood(Tile source, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var frontier = new Queue<Tile>();
		yield return source;
		frontier.Enqueue(source);
		var reached = new HashSet<Tile>() { source };

		while (frontier.Count > 0) {
			var hex = frontier.Dequeue();
			foreach (var neighbor in hex.Neighbors()) {
				if (reached.Add(neighbor)) {
					if (!passable(neighbor)) continue;
					yield return neighbor;
					frontier.Enqueue(neighbor);
				}
			}
		}
	}

	#region Pathing


	private Tile[] BuildPath(Dictionary<Tile, Tile> sources, Tile closest) {
		var res = new List<Tile>();
		var current = closest;
		var i = 0;
		while (current != null && i++ < 1000) {
			res.Add(current);
			sources.TryGetValue(current, out var source);
			current = source;
		}
		res.Reverse();
		return res.ToArray();
	}


	private class SinglePriorityNode : FastPriorityQueueNode {
		public Tile tile;
		public SinglePriorityNode(Tile tile) {
			this.tile = tile;
		}
	}


	private class DoublePriorityNode : FastPriorityQueueNode {
		public Tile tile;
		public float secondary;
		public DoublePriorityNode(Tile tile, float secondary) {
			this.tile = tile;
			this.secondary = secondary;
		}
	}

	private class TriplePriorityNode : FastPriorityQueueNode {
		public Tile tile;
		public float secondary;
		public float tertiary;
		public TriplePriorityNode(Tile tile, float secondary, float tertiary) {
			this.tile = tile;
			this.secondary = secondary;
			this.tertiary = tertiary;
		}
	}

	private class DoubleComparer : IComparer<DoublePriorityNode> {
		public int Compare(DoublePriorityNode x, DoublePriorityNode y) {
			var a = x.Priority.CompareTo(y.Priority);
			if (a != 0) return a;
			var b = x.secondary.CompareTo(y.secondary);
			return b;
		}
	}

	private class TripleComparer : IComparer<TriplePriorityNode> {
		public int Compare(TriplePriorityNode x, TriplePriorityNode y) {
			var a = x.Priority.CompareTo(y.Priority);
			if (a != 0) return a;
			var b = x.secondary.CompareTo(y.secondary);
			if (b != 0) return b;
			var c = x.tertiary.CompareTo(y.tertiary);
			return c;
		}
	}

	/// <summary>
	/// Finds the cheapest path leading from start to the closest reachable Tile to target.
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool CheapestPath(Tile start, Tile target, out Tile[] path, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, float>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<TriplePriorityNode>(tiles.Count, new TripleComparer());
		frontier.Enqueue(new TriplePriorityNode(start, 0, 0), start.moveCost);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + neighbor.moveCost;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						float priority = cost;
						frontier.Enqueue(new TriplePriorityNode(neighbor, neighbor.positiviness, dist), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		path = BuildPath(sources, closest);
		return false;
	}

	public class PathField<T> {
		public readonly Tile closest;
		public readonly Dictionary<Tile, T> scores;
		public readonly Dictionary<Tile, Tile> sources;
		public PathField(Dictionary<Tile, T> costs, Dictionary<Tile, Tile> sources, Tile closest) {
			this.scores = costs;
			this.sources = sources;
			this.closest = closest;
		}
	}

	/// <summary>
	/// Finds the cheapest path leading from start to the closest reachable Tile to target.
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of hTiles starting from start, representing the path.</param>
	/// <param name="field">Field containing the costs and sources of traversed Tiles.</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool CheapestPath(Tile start, Tile target, out Tile[] path, out PathField<float> field, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, float>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<TriplePriorityNode>(tiles.Count, new TripleComparer());
		frontier.Enqueue(new TriplePriorityNode(start, 0, 0), start.moveCost);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + neighbor.moveCost;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<float>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						float priority = cost;
						frontier.Enqueue(new TriplePriorityNode(neighbor, neighbor.positiviness, dist), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								field = new PathField<float>(costs, sources, tile);
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		field = new PathField<float>(costs, sources, closest);
		path = BuildPath(sources, closest);
		return false;
	}


	/// <summary>
	/// Finds the shortest path leading from start to the closest reachable Tile to target.
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool ShortestPath(Tile start, Tile target, out Tile[] path, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(tiles.Count, new DoubleComparer());
		frontier.Enqueue(new DoublePriorityNode(start, start.positiviness), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						var priority = dist + cost;
						frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.positiviness), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		path = BuildPath(sources, closest);
		return false;
	}

	/// <summary>
	/// Finds the shortest path leading from start to the closest reachable Tile to target.
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="field">Field containing the distances and sources of traversed Tiles. (contains inaccuracies).</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool ShortestPath(Tile start, Tile target, out Tile[] path, out PathField<int> field, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(tiles.Count, new DoubleComparer());
		frontier.Enqueue(new DoublePriorityNode(start, start.positiviness), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<int>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						var priority = dist + cost;
						frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.positiviness), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								field = new PathField<int>(costs, sources, tile);
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		field = new PathField<int>(costs, sources, closest);
		path = BuildPath(sources, closest);
		return false;
	}


	/// <summary>
	/// Finds the first path leading from start to the closest reachable Tile to target.
	/// <para> The resulting path will be the first one found by the algorithm and is usually not the fastest or shortest path. </para>
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool FirstPath(Tile start, Tile target, out Tile[] path, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, float>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new FastPriorityQueue<SinglePriorityNode>(tiles.Count);
		frontier.Enqueue(new SinglePriorityNode(start), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other))) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						var priority = dist;
						frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		path = BuildPath(sources, closest);
		return false;
	}

	/// <summary>
	/// Finds the first path leading from start to the closest reachable Tile to target.
	/// <para> The resulting path will be the first one found by the algorithm and is usually not the fastest or shortest path. </para>
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="field">Field containing the costs and sources of traversed Tiles. (Contains inaccuracies).</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>True if target was reached</returns>
	public bool FirstPath(Tile start, Tile target, out Tile[] path, out PathField<int> field, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var minDist = Distance(start, target);
		var closest = start;

		// Prevents needless expansion of search area when the target is unreachable
		var targetDistance = passable(target) ? 0 : 1;

		var costs = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new FastPriorityQueue<SinglePriorityNode>(tiles.Count);
		frontier.Enqueue(new SinglePriorityNode(start), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<int>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other))) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Distance(neighbor, target);
						var priority = dist;
						frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= targetDistance) {
								field = new PathField<int>(costs, sources, tile);
								path = BuildPath(sources, closest);
								return false;
							}
						}
					}
				}
			}
		}
		field = new PathField<int>(costs, sources, closest);
		path = BuildPath(sources, closest);
		return false;
	}

	#endregion


	#region Fields

	public readonly struct CostField {
		public readonly Dictionary<Tile, float> costs;
		public readonly Dictionary<Tile, Tile> sources;
		public CostField(Dictionary<Tile, float> costs, Dictionary<Tile, Tile> sources) {
			this.costs = costs;
			this.sources = sources;
		}
	}

	/// <summary>
	/// Finds the cost to move to reachable Tiles around source.
	/// </summary>
	/// <param name="source">Cost is calculated from this Tile.</param>
	/// <param name="distance">Maximum distance of returned Tiles.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>Field containing the costs and sources of traversed Tiles.</returns>
	public CostField GetCostField(Tile source, int distance = int.MaxValue, float maxCost = float.PositiveInfinity, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var costs = new Dictionary<Tile, float>() { { source, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { source, null } };
		var edges = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { source } } };

		for (int i = 0; edges[i].Count > 0; i++) {
			if (i >= distance) break;
			edges[i + 1] = new List<Tile>();
			foreach (var tile in edges[i]) {
				foreach (var neighbor in tile.Neighbors()) {
					var cost = costs[tile] + neighbor.moveCost;
					if (cost > maxCost) continue;
					if (passable(neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						edges[i + 1].Add(neighbor);
					}
				}
			}
		}

		return new CostField(costs, sources);
	}


	public readonly struct DistanceField {
		public readonly Dictionary<Tile, float> distances;
		public readonly Dictionary<Tile, Tile> sources;
		public DistanceField(Dictionary<Tile, float> distances, Dictionary<Tile, Tile> sources) {
			this.distances = distances;
			this.sources = sources;
		}
	}

	/// <summary>
	/// Finds the cost to move to reachable Tiles around source.
	/// </summary>
	/// <param name="source">Cost is calculated from this Tile.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles.</param>
	/// <param name="passable">Predicate which determines if a Tile is passable.</param>
	/// <returns>Field containing the distances and sources of traversed Tiles.</returns>
	public DistanceField GetDistanceField(Tile source, int distance = int.MaxValue, Predicate<Tile> passable = null) {
		passable = passable ?? (h => !h.blocked);
		var dists = new Dictionary<Tile, float>() { { source, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { source, null } };
		var edges = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { source } } };

		for (int i = 0; edges[i].Count > 0; i++) {
			if (i >= distance) break;
			edges[i + 1] = new List<Tile>();
			foreach (var hex in edges[i]) {
				foreach (var neighbor in hex.Neighbors()) {
					if (passable(neighbor) && !dists.ContainsKey(neighbor)) {
						dists[neighbor] = i + 1;
						sources[neighbor] = hex;
						edges[i + 1].Add(neighbor);
					}
				}
			}
		}

		return new DistanceField(dists, sources);
	}

	#endregion

	[SerializeField, HideInInspector] List<Vector3Int> keyList = new List<Vector3Int>();
	[SerializeField, HideInInspector] List<Tile> valList = new List<Tile>();

	void ISerializationCallbackReceiver.OnBeforeSerialize() {
		keyList = tiles.Select(v => v.Key).ToList();
		valList = tiles.Select(v => v.Value).ToList();
	}

	void ISerializationCallbackReceiver.OnAfterDeserialize() {
		tiles = new Dictionary<Vector3Int, Tile>();
		for (int i = 0; i < keyList.Count; i++) {
			var key = keyList[i];
			var val = valList[i];
			tiles.Add(key, val);
		}
	}
}


#if UNITY_EDITOR

[CustomEditor(typeof(GameGrid))]
public class GameGridEditor : Editor {

	GameGrid t => (GameGrid)target;

	protected virtual void OnSceneGUI() {
		foreach (var kv in t.tiles) {
			var tile = kv.Value;
			var fillColor = tile.blocked ? Color.black : (
					Color.Lerp(
							Saturate(Whitener(Color.green, 0.75f), tile.positiviness * -0.08f),
							Saturate(Whitener(Color.red, 0.75f), tile.positiviness * -0.08f),
							tile.moveCost / 10f
					)
			);
			using (ColorScope(fillColor)) {
				Handles.DrawAAConvexPolygon(tile.corners);
			}
			using (ColorScope(Color.black)) {
				Handles.DrawAAPolyLine(tile.corners);
			}
		}
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		if (GUILayout.Button(nameof(t.ResetGrid))) t.ResetGrid();
		// if (GUILayout.Button(nameof(t.RefreshModels))) t.RefreshModels();
	}

	private Color Whitener(Color color, float minWhite) {
		color.r = Mathf.Max(color.r, minWhite);
		color.g = Mathf.Max(color.g, minWhite);
		color.b = Mathf.Max(color.b, minWhite);
		return color;
	}

	private Color Saturate(Color color, float saturationChange) {
		Color.RGBToHSV(color, out var h, out var s, out var v);
		s += saturationChange;
		return Color.HSVToRGB(h, s, v);
	}


	private Deferred ColorScope(Color color) {
		var prevColor = Handles.color;
		Handles.color = color;
		return new Deferred(() => Handles.color = prevColor);
	}

	private struct Deferred : IDisposable {
		private readonly Action onDispose;

		public Deferred(Action onDispose) {
			this.onDispose = onDispose;
		}

		public void Dispose() {
			onDispose?.Invoke();
		}
	}
}
#endif