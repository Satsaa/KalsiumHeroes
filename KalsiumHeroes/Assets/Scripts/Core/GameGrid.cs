
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

public class GameGrid : MonoBehaviour, ISerializationCallbackReceiver {

  [field: SerializeField]
  public Vector2Int size { get; private set; }

  public Dictionary<Vector3Int, GameHex> hexes = new Dictionary<Vector3Int, GameHex>();

  /// <summary>
  /// Builds a new grid with default values.
  /// </summary>
  internal void ResetGrid() {
    hexes = new Dictionary<Vector3Int, GameHex>();
    for (int x = 0; x < size.y; x++) {
      int yOffset = Mathf.FloorToInt(x / 2);
      for (int y = -yOffset; y < size.x - yOffset; y++) {
        var hex = new Hex(y, x);
        hexes.Add(hex.pos, GameHex.Create(hex));
      }
    }

    foreach (var kv in hexes) {
      var gameHex = kv.Value;
      var hex = gameHex.hex;
      for (int i = 0; i < Hex.neighbors.Length; i++) {
        var offset = Hex.neighbors[i];
        var offsetHex = Hex.Add(hex, offset);
        if (hexes.TryGetValue(offsetHex.pos, out var neighbor))
          gameHex.SetNeighbor((GameHex.Dir)i, neighbor);
      }
    }
  }

  /// <summary>
  /// Iterates in a radius around a GameHex.
  /// </summary>
  public IEnumerable<GameHex> Radius(GameHex hex, int radius) {
    for (int x = -radius; x <= radius; x++) {
      for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(+radius, -x + radius); y++) {
        var pos = hex.hex.pos + new Vector3Int(x, y, -x - y);
        if (hexes.TryGetValue(pos, out var res)) yield return res;
      }
    }
  }

  /// <summary>
  /// Iterates in a Ring around a GameHex.
  /// </summary>
  public IEnumerable<GameHex> Ring(GameHex hex, int radius) {
    if (radius <= 0) {
      if (radius == 0) {
        yield return hex;
        yield break;
      }
      throw new ArgumentOutOfRangeException(nameof(radius));
    }

    var pos = hex.hex.pos + new Vector3Int(0, radius, -radius);
    for (int i = 0; i < 6; i++) {
      for (int j = 0; j < radius; j++) {
        if (hexes.TryGetValue(pos, out var res)) yield return res;
        pos = new Hex(pos).GetNeighbor(i).pos;
      }
    }
  }

  /// <summary>
  /// Return the hex distance of a and b.
  /// </summary>
  public int Distance(GameHex a, GameHex b) {
    return Hex.Distance(a.hex, b.hex);
  }

  /// <summary>
  /// Iterates in a direction until a null GameHex is reached
  /// </summary>
  public IEnumerable<GameHex> Direction(GameHex hex, GameHex.Dir direction) {
    var current = hex;
    while (current != null) {
      yield return current;
      current = current.GetNeighbor(direction);
    }
  }

  /// <summary>
  /// Raycasts along a plane (normal is up) at planeHeight and returns the hex at the raycast hit point.
  /// </summary>
  /// <param name="ray">The ray used for raycasting</param>
  /// <param name="planeHeight">The height of the plane that is being raycasted</param>
  /// <returns>The GameHex at the raycast hit point, if any.</returns>
  public GameHex RaycastHex(Ray ray, float planeHeight = 0) {
    var plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
    if (plane.Raycast(ray, out float enter)) {
      var point = (ray.origin + ray.direction * enter).xz();
      var hex = Layout.PixelToHex(point).Round();
      hexes.TryGetValue(hex.pos, out var res);
      return res;
    }
    return default;
  }

  /// <summary>
  /// Returns an IEnumerable that enumerates the hexes in a line from start to end.
  /// </summary>
  /// <remarks>
  /// Hexes outside the grid will be returned as null.
  /// </remarks>
  /// <param name="start">Start GameHex</param>
  /// <param name="end">End GameHex</param>
  /// <param name="altNudge">Use alternate opposite nudge direction? Edge cases will be nudged the other way.</param>
  /// <returns>An IEnumerable resulting in the hexes along the line.</returns>
  public IEnumerable<GameHex> Line(GameHex start, GameHex end, bool altNudge = false) {
    var _start = start.hex;
    var _end = end.hex;
    int dist = Hex.Distance(_start, _end);
    var xNudge = (altNudge ? -1e-06f : 1e-06f);
    var yNudge = (altNudge ? -1e-06f : 1e-06f);
    var zNudge = (altNudge ? +2e-06f : -2e-06f);
    FractHex startNudge = new FractHex(_start.x + xNudge, _start.y + yNudge, _start.z + zNudge);
    FractHex endNudge = new FractHex(_end.x + xNudge, _end.y + yNudge, _end.z + zNudge);
    for (int i = 0; i < dist; i++) {
      var hex = Hex.Lerp(startNudge, endNudge, 1f / dist * i);
      hexes.TryGetValue(hex.Round().pos, out var ghRes1);
      yield return ghRes1;
    }
    hexes.TryGetValue(_end.pos, out var ghRes2);
    yield return ghRes2;
  }

  /// <summary>
  /// Returns true if the hex has semi-direct vision to target.
  /// </summary>
  /// <note> The hex and target is sampled and <c>Distance(hex, target) - 1</c> amount of samples are taken between hex and target. </note>
  /// <param name="hex">The sighting GameHex</param>
  /// <param name="target">The sighted GameHex</param>
  /// <returns>Whether or not hex has sight of target</returns>
  public bool HasSight(GameHex hex, GameHex target) {
    int nudge = 0;
    var lineA = Line(hex, target, false).GetEnumerator();
    var lineB = Line(hex, target, true).GetEnumerator();

    while (lineA.MoveNext() && lineB.MoveNext()) {
      var a = lineA.Current;
      var b = lineB.Current;
      switch (nudge) {
        case 1: {
            if (Passable(a)) continue;
            else return false;
          }
        case 2: {
            if (Passable(b)) continue;
            else return false;
          }
        default: {
            var aPass = Passable(a);
            var bPass = Passable(b);
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

    bool Passable(GameHex a) {
      return a != null && !a.blocked;
    }
  }

  /// <summary>
  /// Iterates around the hex and yields visible hexes. 
  /// </summary>
  public IEnumerable<GameHex> Vision(GameHex hex, int range) {
    var radius = Radius(hex, range);
    var visible = new HashSet<GameHex>();
    foreach (var radiusHex in radius) {
      if (HasSight(hex, radiusHex) && visible.Add(radiusHex)) yield return radiusHex;
    }
  }

  /// <summary>
  /// Divides blocked of areas and returns the area id for all hexes.
  /// </summary>
  /// <returns>Dictionary of hex areas. Key: GameHex, Value: Area id</returns>
  public Dictionary<GameHex, int> GetAreas() {
    var results = new Dictionary<GameHex, int>(hexes.Count);
    var id = 0;

    foreach (var hex in hexes.Values) {
      if (hex.blocked || results.ContainsKey(hex)) continue;
      foreach (var flooded in Flood(hex)) {
        results.Add(flooded, id);
      }
      id++;
    }
    return results;
  }

  /// <summary>
  /// Iterates the passable hexes around source GameHex.
  /// </summary>
  public IEnumerable<GameHex> Flood(GameHex source) {
    var frontier = new Queue<GameHex>();
    yield return source;
    frontier.Enqueue(source);
    var reached = new HashSet<GameHex>() { source };

    while (frontier.Count > 0) {
      var hex = frontier.Dequeue();
      foreach (var neighbor in hex.Neighbors()) {
        if (reached.Add(neighbor)) {
          if (neighbor.blocked) continue;
          yield return neighbor;
          frontier.Enqueue(neighbor);
        }
      }
    }
  }

  #region Pathing


  private GameHex[] BuildPath(Dictionary<GameHex, GameHex> sources, GameHex closest) {
    var res = new List<GameHex>();
    var current = closest;
    while (current != null) {
      res.Add(current);
      sources.TryGetValue(current, out var source);
      current = source;
    }
    res.Reverse();
    return res.ToArray();
  }

  private float BitwiseAdd(float target, int bitIncrement) {
    byte[] bytes = BitConverter.GetBytes(target);
    int targetAsInt = BitConverter.ToInt32(bytes, 0);
    bytes = BitConverter.GetBytes(targetAsInt + bitIncrement);
    return BitConverter.ToSingle(bytes, 0);
  }


  private class SinglePriorityNode : FastPriorityQueueNode {
    public GameHex hex;
    public SinglePriorityNode(GameHex hex) {
      this.hex = hex;
    }
  }


  private class DoublePriorityNode : FastPriorityQueueNode {
    public GameHex hex;
    public float secondary;
    public DoublePriorityNode(GameHex hex, float secondary) {
      this.hex = hex;
      this.secondary = secondary;
    }
  }

  private class TriplePriorityNode : FastPriorityQueueNode {
    public GameHex hex;
    public float secondary;
    public float tertiary;
    public TriplePriorityNode(GameHex hex, float secondary, float tertiary) {
      this.hex = hex;
      this.secondary = secondary;
      this.tertiary = tertiary;
    }
  }

  private class DoubleComparer : IComparer<DoublePriorityNode> {
    public int Compare(DoublePriorityNode x, DoublePriorityNode y) {
      var a = x.Priority.CompareTo(y.Priority);
      if (a != 0) return a;
      var b = x.secondary.CompareTo(y.secondary);
      return b; ;
    }
  }

  private class TipleComparer : IComparer<TriplePriorityNode> {
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
  /// Finds the cheapest path leading from start to the closest reachable hex to target.
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <returns>True if target was reached</returns>
  public bool CheapestPath(GameHex start, GameHex target, out GameHex[] path) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new ComparisonPriorityQueue<TriplePriorityNode>(hexes.Count, new TipleComparer());
    frontier.Enqueue(new TriplePriorityNode(start, 0, 0), start.moveCost);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + neighbor.moveCost;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          float priority = cost;
          frontier.Enqueue(new TriplePriorityNode(neighbor, neighbor.positiviness, dist), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    path = BuildPath(sources, closest);
    return false;
  }

  public class PathField {
    public readonly GameHex closest;
    public readonly Dictionary<GameHex, float> scores;
    public readonly Dictionary<GameHex, GameHex> sources;
    public PathField(Dictionary<GameHex, float> costs, Dictionary<GameHex, GameHex> sources, GameHex closest) {
      this.scores = costs;
      this.sources = sources;
      this.closest = closest;
    }
  }

  /// <summary>
  /// Finds the cheapest path leading from start to the closest reachable hex to target.
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <param name="field">Field containing the costs and sources of traversed hexed.</param>
  /// <returns>True if target was reached</returns>
  public bool CheapestPath(GameHex start, GameHex target, out GameHex[] path, out PathField field) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new ComparisonPriorityQueue<TriplePriorityNode>(hexes.Count, new TipleComparer());
    frontier.Enqueue(new TriplePriorityNode(start, 0, 0), start.moveCost);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + neighbor.moveCost;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          float priority = cost;
          frontier.Enqueue(new TriplePriorityNode(neighbor, neighbor.positiviness, dist), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    field = new PathField(costs, sources, closest);
    path = BuildPath(sources, closest);
    return false;
  }


  /// <summary>
  /// Finds the shortest path leading from start to the closest reachable hex to target.
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <returns>True if target was reached</returns>
  public bool ShortestPath(GameHex start, GameHex target, out GameHex[] path) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(hexes.Count, new DoubleComparer());
    frontier.Enqueue(new DoublePriorityNode(start, start.positiviness), 1);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + 1;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          var priority = cost + dist;
          frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.positiviness), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    path = BuildPath(sources, closest);
    return false;
  }

  /// <summary>
  /// Finds the shortest path leading from start to the closest reachable hex to target.
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <param name="field">Field containing the distances and sources of traversed hexed.</param>
  /// <returns>True if target was reached</returns>
  public bool ShortestPath(GameHex start, GameHex target, out GameHex[] path, out PathField field) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new ComparisonPriorityQueue<DoublePriorityNode>(hexes.Count, new DoubleComparer());
    frontier.Enqueue(new DoublePriorityNode(start, start.positiviness), 1);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + 1;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          var priority = cost + dist;
          frontier.Enqueue(new DoublePriorityNode(neighbor, neighbor.positiviness), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    field = new PathField(costs, sources, closest);
    path = BuildPath(sources, closest);
    return false;
  }


  /// <summary>
  /// Finds the first path leading from start to the closest reachable hex to target.
  /// <para> The resulting path will be the first one found by the algorithm and is usually not the fastest or shortest path. </para>
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <returns>True if target was reached</returns>
  public bool FirstPath(GameHex start, GameHex target, out GameHex[] path) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new FastPriorityQueue<SinglePriorityNode>(hexes.Count);
    frontier.Enqueue(new SinglePriorityNode(start), 1);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + 1;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other))) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          var priority = dist;
          frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    path = BuildPath(sources, closest);
    return false;
  }

  /// <summary>
  /// Finds the first path leading from start to the closest reachable hex to target.
  /// <para> The resulting path will be the first one found by the algorithm and is usually not the fastest or shortest path. </para>
  /// </summary>
  /// <param name="start">Starting hex</param>
  /// <param name="target">Target hex</param>
  /// <param name="path">Array of hexes starting from start, representing the path.</param>
  /// <param name="field">Field containing the costs and sources of traversed hexed. For this algorithm the data is not reliable.</param>
  /// <returns>True if target was reached</returns>
  public bool FirstPath(GameHex start, GameHex target, out GameHex[] path, out PathField field) {
    var minDist = int.MaxValue;
    var closest = start;

    // Prevents needless expansion of search area when the target is unreachable
    var targetDistance = target.blocked ? 1 : 0;

    var costs = new Dictionary<GameHex, float>() { { start, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { start, null } };
    var frontier = new FastPriorityQueue<SinglePriorityNode>(hexes.Count);
    frontier.Enqueue(new SinglePriorityNode(start), 1);

    while (frontier.Count != 0) {
      var hex = frontier.Dequeue().hex;
      if (hex == target) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, hex);
        return true;
      }
      if (minDist <= targetDistance) {
        field = new PathField(costs, sources, hex);
        path = BuildPath(sources, closest);
        return false;
      }
      foreach (var neighbor in hex.Neighbors()) {
        var cost = costs[hex] + 1;
        if ((!neighbor.blocked) && (!costs.TryGetValue(neighbor, out var other))) {
          costs[neighbor] = cost;
          sources[neighbor] = hex;
          var dist = Distance(neighbor, target);
          var priority = dist;
          frontier.Enqueue(new SinglePriorityNode(neighbor), priority);
          if (dist < minDist) { minDist = dist; closest = neighbor; }
        }
      }
    }
    field = new PathField(costs, sources, closest);
    path = BuildPath(sources, closest);
    return false;
  }

  #endregion


  #region Fields

  public readonly struct CostField {
    public readonly Dictionary<GameHex, float> costs;
    public readonly Dictionary<GameHex, GameHex> sources;
    public CostField(Dictionary<GameHex, float> costs, Dictionary<GameHex, GameHex> sources) {
      this.costs = costs;
      this.sources = sources;
    }
  }

  /// <summary>
  /// Finds the cost to move to reachable hexes around source.
  /// </summary>
  /// <param name="source">Cost is calculated from this hex.</param>
  /// <param name="distance">Maximum distance of returned hexes</param>
  /// <param name="maxCost">Maximum cost of returned hexes</param>
  /// <returns>Field containing the costs and sources of traversed hexed</returns>
  public CostField GetCostField(GameHex source, int distance = int.MaxValue, float maxCost = float.PositiveInfinity) {
    var costs = new Dictionary<GameHex, float>() { { source, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { source, null } };
    var edges = new Dictionary<float, List<GameHex>>() { { 0, new List<GameHex>() { source } } };

    for (int i = 0; edges[i].Count > 0; i++) {
      if (i >= distance) break;
      edges[i + 1] = new List<GameHex>();
      foreach (var hex in edges[i]) {
        foreach (var neighbor in hex.Neighbors()) {
          var cost = costs[hex] + neighbor.moveCost;
          if (cost > maxCost) continue;
          if (!neighbor.blocked && (!costs.TryGetValue(neighbor, out var other) || other > cost)) {
            costs[neighbor] = cost;
            sources[neighbor] = hex;
            edges[i + 1].Add(neighbor);
          }
        }
      }
    }

    return new CostField(costs, sources);
  }


  public readonly struct DistanceField {
    public readonly Dictionary<GameHex, float> distances;
    public readonly Dictionary<GameHex, GameHex> sources;
    public DistanceField(Dictionary<GameHex, float> distances, Dictionary<GameHex, GameHex> sources) {
      this.distances = distances;
      this.sources = sources;
    }
  }

  /// <summary>
  /// Finds the cost to move to reachable hexes around source.
  /// </summary>
  /// <param name="source">Cost is calculated from this hex.</param>
  /// <param name="maxCost">Maximum cost of returned hexes</param>
  /// <returns>Field containing the distances and sources of traversed hexed</returns>
  public DistanceField GetDistanceField(GameHex source, int distance = int.MaxValue) {
    var dists = new Dictionary<GameHex, float>() { { source, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { source, null } };
    var edges = new Dictionary<float, List<GameHex>>() { { 0, new List<GameHex>() { source } } };

    for (int i = 0; edges[i].Count > 0; i++) {
      if (i >= distance) break;
      edges[i + 1] = new List<GameHex>();
      foreach (var hex in edges[i]) {
        foreach (var neighbor in hex.Neighbors()) {
          if (!neighbor.blocked && !dists.ContainsKey(neighbor)) {
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
  [SerializeField, HideInInspector] List<GameHex> valList = new List<GameHex>();

  void ISerializationCallbackReceiver.OnBeforeSerialize() {
    keyList = hexes.Select(v => v.Key).ToList();
    valList = hexes.Select(v => v.Value).ToList();
  }

  void ISerializationCallbackReceiver.OnAfterDeserialize() {
    hexes = new Dictionary<Vector3Int, GameHex>();
    for (int i = 0; i < keyList.Count; i++) {
      var key = keyList[i];
      var val = valList[i];
      hexes.Add(key, val);
    }
  }
}


#if UNITY_EDITOR

[CustomEditor(typeof(GameGrid))]
public class GameGridEditor : Editor {

  GameGrid t => (GameGrid)target;

  protected virtual void OnSceneGUI() {
    foreach (var kv in t.hexes) {
      var hex = kv.Value;
      var fillColor = hex.blocked ? Color.black : (
        Color.Lerp(
          Saturate(Whitener(Color.green, 0.75f), hex.positiviness * -0.08f),
          Saturate(Whitener(Color.red, 0.75f), hex.positiviness * -0.08f),
          hex.moveCost / 10f
        )
      );
      using (ColorScope(fillColor)) {
        Handles.DrawAAConvexPolygon(hex.corners);
      }
      using (ColorScope(Color.black)) {
        Handles.DrawAAPolyLine(hex.corners);
      }
    }
  }

  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button(nameof(t.ResetGrid))) t.ResetGrid();
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
    return new Deferred(() => {
      Handles.color = prevColor;
    });
  }

  private struct Deferred : IDisposable {
    private readonly Action onDispose;

    public Deferred(Action onDispose) {
      this.onDispose = onDispose;
    }

    public void Dispose() {
      if (onDispose != null)
        onDispose();
    }
  }
}
#endif