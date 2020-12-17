
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

	private static Tile[] BuildPath(Dictionary<Tile, Tile> sources, Tile closest) {
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
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <param name="costCalculator">Calculates the cost to move over an Edge.</param>
	/// <returns>True if target was reached</returns>
	public static bool CheapestPath(Tile start, Tile target, out Tile[] path, out PathField<float> field, Pather pather = null, CostCalculator costCalculator = null) {
		pather ??= Pathers.OneWay;
		costCalculator ??= CostCalculators.Default;

		var minDist = Game.grid.Distance(start, target);
		var closest = start;

		var costs = new Dictionary<Tile, float>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<TriplePriorityNode>(Game.grid.tiles.Count, new TripleComparer());
		frontier.Enqueue(new TriplePriorityNode(start, 0, 0), start.tileData.moveCost.value);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				for (int i = 0; i < tile.neighbors.Length; i++) {
					var neighbor = tile.neighbors[i];
					if (neighbor == null) continue;
					var edge = tile.edges[i];
					var cost = costs[tile] + costCalculator(tile, edge, neighbor);
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<float>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (pather(tile, edge, neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Game.grid.Distance(neighbor, target);
						float priority = cost;
						frontier.Enqueue(new TriplePriorityNode(neighbor, neighbor.tileData.appeal.value, dist), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= 0) {
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
	/// <param name="field">Field containing the distances and sources of traversed Tiles. (contains inaccuracies).</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>True if target was reached</returns>
	public static bool ShortestPath(Tile start, Tile target, out Tile[] path, out PathField<int> field, Pather pather = null) {
		pather ??= Pathers.OneWay;

		var minDist = Game.grid.Distance(start, target);
		var closest = start;

		var costs = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(Game.grid.tiles.Count, new DoubleComparer());
		frontier.Enqueue(new DoublePriorityNode(start, start.tileData.appeal.value), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				for (int i = 0; i < tile.neighbors.Length; i++) {
					var neighbor = tile.neighbors[i];
					if (neighbor == null) continue;
					var edge = tile.edges[i];
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<int>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (pather(tile, edge, neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Game.grid.Distance(neighbor, target);
						var priority = dist + cost;
						frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.tileData.appeal.value), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= 0) {
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
	/// <para> The resulting path will be the first one found by the algorithm and is often not the fastest or the shortest path. </para>
	/// </summary>
	/// <param name="start">Starting Tile</param>
	/// <param name="target">Target Tile</param>
	/// <param name="path">Array of Tiles starting from start, representing the path.</param>
	/// <param name="field">Field containing the costs and sources of traversed Tiles. (Contains inaccuracies).</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>True if target was reached</returns>
	public static bool FirstPath(Tile start, Tile target, out Tile[] path, out PathField<int> field, Pather pather = null) {
		pather ??= Pathers.OneWay;

		var minDist = Game.grid.Distance(start, target);
		var closest = start;

		var costs = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new FastPriorityQueue<SinglePriorityNode>(Game.grid.tiles.Count);
		frontier.Enqueue(new SinglePriorityNode(start), 1);

		if (start != target) {
			while (frontier.Count != 0) {
				var tile = frontier.Dequeue().tile;
				for (int i = 0; i < tile.neighbors.Length; i++) {
					var neighbor = tile.neighbors[i];
					if (neighbor == null) continue;
					var edge = tile.edges[i];
					var cost = costs[tile] + 1;
					if (neighbor == target) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						field = new PathField<int>(costs, sources, neighbor);
						path = BuildPath(sources, neighbor);
						return true;
					}
					if (pather(tile, edge, neighbor) && (!costs.TryGetValue(neighbor, out var other))) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						var dist = Game.grid.Distance(neighbor, target);
						var priority = dist;
						frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
						if (dist < minDist) {
							minDist = dist;
							closest = neighbor;
							if (minDist <= 0) {
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
	/// <param name="start">Field starts from this Tile.</param>
	/// <param name="distance">Maximum distance of returned Tiles.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <param name="costCalculator">Calculates the cost to move over an Edge.</param>
	/// <returns>Field containing the costs and sources of traversed Tiles.</returns>
	public static CostField GetCostField(Tile start, int distance = int.MaxValue, float maxCost = float.PositiveInfinity, Pather pather = null, CostCalculator costCalculator = null) {
		pather ??= Pathers.OneWay;
		costCalculator ??= CostCalculators.Default;

		var costs = new Dictionary<Tile, float>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { start } } };

		for (int i = 0; frontier[i].Count > 0; i++) {
			if (i >= distance) break;
			frontier[i + 1] = new List<Tile>();
			foreach (var tile in frontier[i]) {
				for (int j = 0; j < tile.neighbors.Length; j++) {
					var neighbor = tile.neighbors[j];
					if (neighbor == null) continue;
					var edge = tile.edges[j];
					var cost = costs[tile] + costCalculator(tile, edge, neighbor);
					if (cost > maxCost) continue;
					if (pather(tile, edge, neighbor) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
						costs[neighbor] = cost;
						sources[neighbor] = tile;
						frontier[i + 1].Add(neighbor);
					}
				}
			}
		}

		return new CostField(costs, sources);
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
	/// Finds the path distance of reachable Tiles around source.
	/// </summary>
	/// <param name="start">Field starts from this Tile.</param>
	/// <param name="maxCost">Maximum cost of returned Tiles.</param>
	/// <param name="pather">Determines if you can move over an Edge.</param>
	/// <returns>Field containing the distances and sources of traversed Tiles.</returns>
	public static DistanceField GetDistanceField(Tile start, int distance = int.MaxValue, Pather pather = null) {
		pather ??= Pathers.OneWay;

		var dists = new Dictionary<Tile, int>() { { start, 0 } };
		var sources = new Dictionary<Tile, Tile>() { { start, null } };
		var frontier = new Dictionary<float, List<Tile>>() { { 0, new List<Tile>() { start } } };

		for (int i = 0; frontier[i].Count > 0; i++) {
			if (i >= distance) break;
			frontier[i + 1] = new List<Tile>();
			foreach (var tile in frontier[i]) {
				for (int j = 0; j < tile.neighbors.Length; j++) {
					var neighbor = tile.neighbors[j];
					if (neighbor == null) continue;
					var edge = tile.edges[j];
					if (pather(tile, edge, neighbor) && !dists.ContainsKey(neighbor)) {
						dists[neighbor] = i + 1;
						sources[neighbor] = tile;
						frontier[i + 1].Add(neighbor);
					}
				}
			}
		}

		return new DistanceField(dists, sources);
	}

	#endregion

}

