
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

public static class Pathing {

	#region Pathing

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


	public class PathResult {

		public readonly Tile source;
		public readonly Tile closest;
		public readonly Dictionary<Tile, FieldItem> tiles;
		public Tile[] path { get; }

		public PathResult(Tile source, Dictionary<Tile, FieldItem> tiles, Tile closest) {
			this.source = source;
			this.tiles = tiles;
			this.closest = closest;
			this.path = BuildPath(closest);
		}

		public Tile[] BuildPath(Tile to) {
			if (!tiles.TryGetValue(to, out var _)) throw new KeyNotFoundException("Tile was not reached during the search.");
			var capacity = 0;
			var current = to;
			while (current != null) {
				if (tiles.TryGetValue(current, out var item)) {
					current = item.source;
					capacity++;
				} else {
					break;
				}
			}
			var res = new Tile[capacity];
			current = to;
			for (int i = 0; i < capacity; i++) {
				res[capacity - i - 1] = current;
				current = tiles[current].source;
			}
			return res;
		}
	}

	public class FieldItem {
		public readonly float cost;
		public readonly float appeal;
		public readonly Tile source;
		public FieldItem(float cost, float appeal, Tile source) {
			this.cost = cost;
			this.appeal = appeal;
			this.source = source;
		}
	}


	/// <summary>
	/// Finds the cheapest path leading from start to the closest reachable Tile to target using Dijkstra's.
	/// </summary>
	/// <remarks>
	/// The cheapest path is selected. The most appealing path is selected.
	/// </remarks>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="result">Object containing the results.</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <param name="costCalculator">Calculates the cost to move over an Edge.</param>
	/// <returns>True if target was reached.</returns>
	public static bool CheapestPath(Tile start, Tile target, out PathResult result, Pather pather = null, CostCalculator costCalculator = null) {
		pather ??= Pathers.Phased;
		costCalculator ??= CostCalculators.MoveCost;

		var minDist = Game.grid.Distance(start, target);
		var minCost = float.PositiveInfinity;
		var closest = start;

		var tiles = new Dictionary<Tile, FieldItem>() { { start, new FieldItem(0, 0, null) } };
		var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(Game.grid.tiles.Count, new DoubleComparer());
		frontier.Enqueue(new DoublePriorityNode(start, 0), start.moveCost.current);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var (neighbor, edge) in tile.NeighborsWithEdges()) {
					var cost = tiles[tile].cost + costCalculator(tile, edge, neighbor);
					var appeal = tiles[tile].appeal + tile.appeal.current;
					if (pather(tile, edge, neighbor) && (!tiles.TryGetValue(neighbor, out var other) || other.cost > cost || (other.cost == cost && other.appeal < appeal))) {
						tiles[neighbor] = new FieldItem(cost, appeal, tile);
						if (cost < minCost || closest != target) {
							float priority = cost;
							frontier.Enqueue(new DoublePriorityNode(neighbor, -neighbor.appeal.current), priority);
							var dist = Game.grid.Distance(neighbor, target);
							if (dist < minDist) {
								minDist = dist;
								closest = neighbor;
								if (closest == target) minCost = cost;
							}
						}
					}
				}
			}
		}
		result = new PathResult(start, tiles, closest);
		return closest == target;
	}


	/// <summary>
	/// Finds the shortest path leading from start to the closest reachable Tile to target using A*.
	/// </summary>
	/// <remarks>
	/// The shortest path is selected. Appealing tile are favored.
	/// </remarks>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="result">Object containing the results.</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>True if target was reached</returns>
	public static bool ShortestPath(Tile start, Tile target, out PathResult result, Pather pather = null) {
		pather ??= Pathers.Phased;

		var minDist = Game.grid.Distance(start, target);
		var maxAppeal = float.NegativeInfinity;
		var closest = start;

		var tiles = new Dictionary<Tile, FieldItem>() { { start, new FieldItem(0, 0, null) } };
		var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(Game.grid.tiles.Count, new DoubleComparer());
		frontier.Enqueue(new DoublePriorityNode(start, 0), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var (neighbor, edge) in tile.NeighborsWithEdges()) {
					var cost = tiles[tile].cost + 1;
					var appeal = tiles[tile].appeal + tile.appeal.current;
					if (pather(tile, edge, neighbor) && (!tiles.TryGetValue(neighbor, out var other) || other.cost > cost || (other.cost == cost && other.appeal < appeal))) {
						tiles[neighbor] = new FieldItem(cost, appeal, tile);
						if (closest != target) {
							var dist = Game.grid.Distance(neighbor, target);
							var priority = dist + cost;
							frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.appeal.current), priority);
							if (dist < minDist) {
								minDist = dist;
								closest = neighbor;
								if (closest == target) maxAppeal = appeal;
							}
						}
					}
				}
			}
		}
		result = new PathResult(start, tiles, closest);
		return closest == target;
	}


	/// <summary>
	/// Finds the first path leading from start to the closest reachable Tile to target using optimism. Computationally less intensive.
	/// </summary>
	/// <remarks>
	/// Tiles closer to the target are favored.
	/// </remarks>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="result">Object containing the results.</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>True if target was reached</returns>
	public static bool FirstPath(Tile start, Tile target, out PathResult result, Pather pather = null) {
		pather ??= Pathers.Phased;

		var minDist = Game.grid.Distance(start, target);
		var closest = start;

		var tiles = new Dictionary<Tile, FieldItem>() { { start, new FieldItem(0, 0, null) } };
		var frontier = new FastPriorityQueue<SinglePriorityNode>(Game.grid.tiles.Count);
		frontier.Enqueue(new SinglePriorityNode(start), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				foreach (var (neighbor, edge) in tile.NeighborsWithEdges()) {
					var cost = tiles[tile].cost + 1;
					if (pather(tile, edge, neighbor) && (!tiles.TryGetValue(neighbor, out var other))) {
						tiles[neighbor] = new FieldItem(cost, 0, tile);
						var dist = Game.grid.Distance(neighbor, target);
						var priority = dist;
						frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (closest == target) {
								result = new PathResult(start, tiles, closest);
								return true;
							}
						}
					}
				}
			}
		}
		result = new PathResult(start, tiles, closest);
		return closest == target;
	}

	#endregion


	#region Fields

	public class FieldResult {

		public readonly Tile source;
		public readonly Dictionary<Tile, FieldItem> tiles;

		public FieldResult(Tile source, Dictionary<Tile, FieldItem> tiles) {
			this.source = source;
			this.tiles = tiles;
		}

		public Tile[] BuildPath(Tile to) {
			if (!tiles.TryGetValue(to, out var _)) throw new KeyNotFoundException("Tile was not reached during the search.");
			var capacity = 0;
			var current = to;
			while (current != null) {
				if (tiles.TryGetValue(current, out var item)) {
					current = item.source;
					capacity++;
				} else {
					break;
				}
			}
			var res = new Tile[capacity];
			current = to;
			for (int i = 0; i < capacity; i++) {
				res[capacity - i - 1] = current;
				current = tiles[current].source;
			}
			return res;
		}
	}

	/// <summary>
	/// Finds the cost to move to reachable Tiles around source using Dijkstra's.
	/// </summary>
	/// <param name="source">Field starts from this Tile.</param>
	/// <param name="distance">Maximum distance of returned Tiles.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <param name="costCalculator">Calculates the cost to move over an Edge.</param>
	/// <returns>Field containing the costs and sources of reached Tiles.</returns>
	public static FieldResult GetCostField(Tile source, int distance = int.MaxValue, float maxCost = float.PositiveInfinity, Pather pather = null, CostCalculator costCalculator = null) {
		pather ??= Pathers.Phased;
		costCalculator ??= CostCalculators.MoveCost;

		var tiles = new Dictionary<Tile, FieldItem>() { { source, new FieldItem(0, 0, null) } };
		var frontier = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { source } } };

		for (int i = 0; frontier[i].Count > 0; i++) {
			if (i >= distance) break;
			frontier[i + 1] = new List<Tile>();
			foreach (var tile in frontier[i]) {
				foreach (var (neighbor, edge) in tile.NeighborsWithEdges()) {
					var cost = tiles[tile].cost + costCalculator(tile, edge, neighbor);
					if (cost > maxCost) continue;
					var appeal = tiles[tile].appeal + tile.appeal.current;
					if (pather(tile, edge, neighbor) && (!tiles.TryGetValue(neighbor, out var other) || other.cost > cost || (other.cost == cost && other.appeal < appeal))) {
						tiles[neighbor] = new FieldItem(cost, appeal, tile);
						frontier[i + 1].Add(neighbor);
					}
				}
			}
		}

		return new FieldResult(source, tiles);
	}


	public readonly struct DistanceField {
		public readonly Dictionary<Tile, int> distances;
		public readonly Dictionary<Tile, Tile> sources;
		public DistanceField(Dictionary<Tile, int> distances, Dictionary<Tile, Tile> sources) {
			this.distances = distances;
			this.sources = sources;
		}
	}

	/// <summary>
	/// Finds the path distance of reachable Tiles around source using Dijkstra's.
	/// </summary>
	/// <param name="source">Field starts from this Tile.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles.</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>Field containing the distances and sources of reached Tiles.</returns>
	public static FieldResult GetDistanceField(Tile source, int distance = int.MaxValue, Pather pather = null) {
		pather ??= Pathers.Phased;

		var tiles = new Dictionary<Tile, FieldItem>() { { source, new FieldItem(0, 0, null) } };
		var frontier = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { source } } };

		for (int i = 0; frontier[i].Count > 0; i++) {
			if (i >= distance) break;
			frontier[i + 1] = new List<Tile>();
			foreach (var tile in frontier[i]) {
				foreach (var (neighbor, edge) in tile.NeighborsWithEdges()) {
					var cost = i + 1;
					var appeal = tiles[tile].appeal + tile.appeal.current;
					if (pather(tile, edge, neighbor) && (!tiles.TryGetValue(neighbor, out var other) || other.cost > cost || (other.cost == cost && other.appeal < appeal))) {
						tiles[neighbor] = new FieldItem(cost, appeal, tile);
						frontier[cost].Add(neighbor);
					}
				}
			}
		}

		return new FieldResult(source, tiles);
	}

	#endregion

}

