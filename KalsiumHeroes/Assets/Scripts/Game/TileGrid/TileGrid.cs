
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using Muc.Numerics;
using HexGrid;
using System;
using Priority_Queue;
using UnityEngine.Serialization;
using System.Reflection;
using Serialization;
using Muc.Data;

public class TileGrid : ScriptableObject, IGameSerializable {

	[field: SerializeField]
	public Vector2Int size { get; private set; }

	public bool drawHexes;

	public TileData defaultTile;
	public EdgeData defaultEdge;

	public SerializedDictionary<Vector3Int, Tile> tiles = new();

	public Tile GetTile(Hex hex) => GetTile(hex.pos);
	public Tile GetTile(Vector3Int pos) {
		tiles.TryGetValue(pos, out var res);
		return res;
	}

	/// <summary>
	/// Builds a new grid with default values.
	/// </summary>
	public void ResetGrid() {

		DestroyGrid();

		for (int y = 0; y < size.y; y++) {
			int xOffset = Mathf.FloorToInt(y / 2);
			for (int x = -xOffset; x < size.x - xOffset; x++) {
				var hex = new Hex(x, y);

				// Cut right edge for symmetry
				var yOddRow = y / 2 + 1;
				if (y % 2 == 1 && x + yOddRow == size.x) continue;
				var tile = Tile.Create(defaultTile, hex);
			}
		}

	}

	protected void DestroyGrid() {
		foreach (var tile in Game.grid.tiles.Values.ToList()) {
			if (tile != null) tile.Remove();
		}
		foreach (var tile in FindObjectsOfType<Tile>()) {
			ObjectUtil.Destroy(tile);
		}

		tiles.Clear();
		Resources.UnloadUnusedAssets();
	}

	public Tile CreateTile(Hex hex, TileData source) {
		return Tile.Create(source, hex);
	}

	public Tile ReplaceTile(Hex hex, TileData source) {
		Muc.Collections.SafeList<Unit> units = default;
		if (tiles.TryGetValue(hex.pos, out var tile)) {
			units = tile.units;
		}
		DestroyTile(hex);
		var res = CreateTile(hex, source);
		if (units != null) {
			foreach (var unit in units) {
				unit.SetTile(res, false);
			}
			res.units = units;
		}
		return res;
	}

	public bool DestroyTile(Hex hex) {
		if (tiles.TryGetValue(hex.pos, out var tile)) {
			tile.Remove();
			return true;
		}
		return false;
	}


	/// <summary> Iterates in a radius around hex. </summary>
	public IEnumerable<Tile> Radius(Hex hex, int radius) {
		for (int x = -radius; x <= radius; x++) {
			for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(+radius, -x + radius); y++) {
				var pos = hex.pos + new Vector3Int(x, y, -x - y);
				if (tiles.TryGetValue(pos, out var res)) yield return res;
			}
		}
	}

	/// <summary>
	/// Iterates in a ring around hex.
	/// </summary>
	public IEnumerable<Tile> Ring(Hex hex, int distance) {
		if (distance <= 0) {
			if (distance == 0) {
				if (tiles.TryGetValue(hex.pos, out var res))
					yield return res;
				yield break;
			}
			throw new ArgumentOutOfRangeException(nameof(distance));
		}
		var pos = hex.pos + new Vector3Int(-distance, distance, 0);
		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < distance; j++) {
				if (tiles.TryGetValue(pos, out var res)) yield return res;
				pos = new Hex(pos).GetNeighbor(i).pos;
			}
		}
	}

	/// <summary>
	/// Iterates in a spiral around hex.
	/// </summary>
	/// <param name="max">Max distance</param>
	public IEnumerable<Tile> Spiral(Hex hex, int distance = Int32.MaxValue) => Spiral(hex, 0, distance);

	/// <summary>
	/// Iterates in a spiral around hex that starts at distance min.
	/// </summary>
	/// <param name="min">Start distance</param>
	/// <param name="max">Max distance</param>
	public IEnumerable<Tile> Spiral(Hex hex, int min, int max) {
		for (int i = min; i <= max; i++) {
			// Yields the first Tile in the ring last.
			var enumer = Ring(hex, i).GetEnumerator();
			if (enumer.MoveNext()) {
				var first = enumer.Current;
				while (enumer.MoveNext()) {
					yield return enumer.Current;
				}
				yield return first;
			}
		}
	}

	/// <summary> Returns the distance between a and b in tiles. </summary>
	public int Distance(Hex a, Hex b) {
		return Hex.Distance(a, b);
	}

	/// <summary>
	/// Iterates in a direction until a null Tile is reached.
	/// </summary>
	/// <remarks>
	/// The initial Tile is included in the iteration.
	/// </remarks>
	/// <param name="hex">The starting Hex.</param>
	/// <param name="direction">The direction to iterate in.</param>
	public IEnumerable<Tile> Direction(Hex hex, TileDir direction) {
		tiles.TryGetValue(hex.pos, out var current);
		while (current != null) {
			yield return current;
			current = current.GetNeighbor(direction);
		}
	}

	/// <summary>
	/// Raycasts along a plane at height and returns the Tile at the raycast hit point.
	/// </summary>
	/// <param name="ray">The ray used for raycasting.</param>
	/// <param name="height">The height of the plane that is being raycasted.</param>
	/// <returns>The Tile at the raycast hit point, if any.</returns>
	public Tile RaycastTile(Ray ray, float height = 0) {
		var plane = new Plane(Vector3.up, new Vector3(0, height, 0));
		if (plane.Raycast(ray, out float enter)) {
			var point = (ray.origin + ray.direction * enter).xz();
			return TileAtPoint(point);
		}
		return default;
	}

	/// <summary>
	/// Raycasts along a plane at height and returns the Hex of the raycast hit point.
	/// </summary>
	/// <param name="ray">The ray used for raycasting.</param>
	/// <param name="height">The height of the plane that is being raycasted.</param>
	/// <returns>The Hex of the raycast hit point.</returns>
	public Hex RaycastHex(Ray ray, float height = 0) {
		var plane = new Plane(Vector3.up, new Vector3(0, height, 0));
		if (plane.Raycast(ray, out float enter)) {
			var point = (ray.origin + ray.direction * enter).xz();
			return Layout.PointToHex(point).Round();
		}
		return default;
	}

	/// <summary> Returns the Tile under mouse. </summary>
	public Tile GetHoveredTile(int raycastRadius = 1) => GetHoveredTile(Camera.main, raycastRadius);
	/// <summary> Returns the Tile under mouse. </summary>
	public Tile GetHoveredTile(Camera camera, int raycastRadius = 1) {
		var ray = camera.ScreenPointToRay(App.input.pointer);
		var hex = Game.grid.RaycastHex(ray);
		var main = Game.grid.GetTile(hex);
		var radius = Game.grid.Radius(hex, raycastRadius);
		List<(Tile tile, Collider col)> candidates = radius.Select(v => (v, v.gameObject ? v.gameObject.GetComponent<Collider>() : null)).ToList();

		var tile = main;
		var minDist = float.PositiveInfinity;
		foreach (var candidate in candidates) {
			if (candidate.col && candidate.col.Raycast(ray, out var hit, minDist) && hit.point.y >= (main == null || !main.gameObject ? -0.25f : main.gameObject.transform.position.y - 0.25f)) {
				minDist = hit.distance;
				tile = candidate.tile;
			}
		}
		return tile;
	}

	/// <summary> Returns the Tile at point. </summary>
	public Tile TileAtPoint(Vector2 point) {
		var hex = Layout.PointToHex(point).Round();
		tiles.TryGetValue(hex.pos, out var res);
		return res;
	}

	/// <summary> Returns the nearest Tile to point. </summary>
	/// <remarks> Not optimized when there is no Tile at point. Distances to all other Tiles are calculated. </remarks> <summary>
	/// <param name="point">Reference point</param>
	/// <param name="predicate">Optional predicate for Tiles.</param>
	public Tile NearestTile(Vector2 point, Predicate<Tile> predicate = null) {
		var hex = Layout.PointToHex(point).Round();
		if (!tiles.TryGetValue(hex.pos, out var res) || (predicate != null && !predicate(res))) {
			var dist = float.PositiveInfinity;
			foreach (var tile in tiles.Values) {
				if (predicate == null || predicate(tile)) {
					var thisDist = Vector2.Distance(point, Layout.HexToPoint(tile.hex));
					if (thisDist < dist) {
						dist = thisDist;
						res = tile;
					}
				}
			}
		}
		return res;
	}

	/// <summary> Returns the nearest Tile to point. Searchs in a spiral around point and picks the nearest Tile in the first ring with a Tile. </summary>
	/// <remarks> Not optimized at large distances, use maxDistance with null handling. </remarks>
	/// <param name="point">Reference point</param>
	/// <param name="predicate">Optional predicate for Tiles.</param>
	/// <param name="maxDistance">Maximum search distance.</param>
	public Tile NearestTileSpiralSearch(Vector2 point, Predicate<Tile> predicate = null, int maxDistance = int.MaxValue) =>
		NearestTileSpiralSearch(point, maxDistance, predicate);

	/// <summary> Returns the nearest Tile to point. Searchs in a spiral around point and picks the nearest Tile in the first ring with a Tile. </summary>
	/// <remarks> Not optimized at large distances, use maxDistance with null handling. </remarks>
	/// <param name="point">Reference point</param>
	/// <param name="maxDistance">Maximum search distance.</param>
	/// <param name="predicate">Optional predicate for Tiles.</param>
	public Tile NearestTileSpiralSearch(Vector2 point, int maxDistance = int.MaxValue, Predicate<Tile> predicate = null) {
		var hex = Layout.PointToHex(point).Round();
		if (!tiles.TryGetValue(hex.pos, out var res) || (predicate != null && !predicate(res))) {
			foreach (var spiralTile in Spiral(hex, maxDistance)) {
				var ringDist = Distance(hex, spiralTile);
				var dist = float.PositiveInfinity;
				foreach (var tile in Ring(hex, ringDist)) {
					if (predicate == null || predicate(tile)) {
						var thisDist = Vector2.Distance(point, Layout.HexToPoint(tile.hex));
						if (thisDist < dist) {
							dist = thisDist;
							res = tile;
						}
					}
				}
				return res;
			}
		}
		return res;
	}

	/// <summary>
	/// Returns an IEnumerable that enumerates existing Tiles in a line from start to end.
	/// </summary>
	/// <param name="start">Start Hex</param>
	/// <param name="end">End Hex</param>
	/// <param name="altNudge">Use alternate opposite nudge direction? Edge cases will be nudged the other way.</param>
	public IEnumerable<Tile> Line(Hex start, Hex end, bool altNudge = false) {
		int dist = Hex.Distance(start, end);

		var nudge = new Vector3(
			altNudge ? -1e-06f : +1e-06f,
			altNudge ? -1e-06f : +1e-06f,
			altNudge ? +2e-06f : -2e-06f
		);

		var startNudge = new FractHex(start.pos + nudge);
		var endNudge = new FractHex(end.pos + nudge);

		for (int i = 0; i < dist; i++) {
			var hex = Hex.Lerp(startNudge, endNudge, 1f / dist * i);
			if (tiles.TryGetValue(hex.Round().pos, out var ghRes1))
				yield return ghRes1;
		}
		if (tiles.TryGetValue(end.pos, out var ghRes2))
			yield return ghRes2;
	}

	/// <summary>
	/// Returns an IEnumerable that enumerates existing Tiles in a line from start to end. When no Tile is found or the predicate return false, tries again with the other nudge direction.
	/// </summary>
	/// <param name="start">Start Hex</param>
	/// <param name="end">End Hex</param>
	/// <param name="predicate">Determines whether a Tile is not avoided.</param>
	public IEnumerable<Tile> SmartLine(Hex start, Hex end, Predicate<Tile> predicate = null) {
		predicate = predicate ?? (h => true);
		int dist = Hex.Distance(start, end);
		var nudge = new Vector3(+1e-06f, +1e-06f, -2e-06f);
		var altNudge = new Vector3(-1e-06f, -1e-06f, +2e-06f);

		var startNudge = new FractHex(start.pos + nudge);
		var endNudge = new FractHex(end.pos + nudge);
		var altStartNudge = new FractHex(start.pos + altNudge);
		var altEndNudge = new FractHex(end.pos + altNudge);

		for (int i = 0; i < dist; i++) {
			var hex = Hex.Lerp(startNudge, endNudge, 1f / dist * i);
			if (tiles.TryGetValue(hex.Round().pos, out var res1) && predicate(res1)) {
				yield return res1;
			} else {
				var altHex = Hex.Lerp(altStartNudge, altEndNudge, 1f / dist * i);
				if (tiles.TryGetValue(altHex.Round().pos, out var res2))
					yield return res2;
			}
		}
		if (tiles.TryGetValue(end.pos, out var res3))
			yield return res3;
	}

	/// <summary>
	/// Returns true if tile has pseudo-direct vision of target.
	/// </summary>
	/// <remarks> The tile and target is sampled additionally <c>Distance(tile, target) - 1</c> amount of samples are taken between tile and target. </remarks>
	/// <param name="tile">The sighting Tile</param>
	/// <param name="target">The sighted Tile</param>
	/// <param name="predicate">Predicate which determines if a Tile is see-through.</param>
	public bool HasSight(Hex hex, Hex target, Predicate<Tile> predicate = null) {
		predicate = predicate ?? (h => h == null || h.data.transparent.current);
		return SmartLine(hex, target, v => predicate(v)).All(predicate.Invoke);
	}

	/// <summary>
	/// Iterates in a radius around hex and yields visible Tiles.
	/// </summary>
	/// <param name="seeThrough">Predicate which determines if a Tile is see-through.</param>
	public IEnumerable<Tile> Vision(Hex hex, int range, Predicate<Tile> seeThrough = null) {
		var radius = Radius(hex, range);
		foreach (var radiusTile in radius) {
			if (HasSight(hex, radiusTile, seeThrough)) yield return radiusTile;
		}
	}

	/// <summary>
	/// Finds blocked of areas and returns the area id for all Tiles.
	/// </summary>
	/// <param name="passable">Predicate which determines if a Tile is passable and thus included in the search.</param>
	/// <returns>Dictionary of Tile areas. Key: Tile, Value: Area id</returns>
	public Dictionary<Tile, int> GetAreas(Predicate<Tile> passable = null) {
		passable = passable ?? (h => h.data.passable.current);
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
	/// <param name="passable">Predicate which determines if a Tile is passable and thus included in the search.</param>
	public IEnumerable<Tile> Flood(Tile source, Predicate<Tile> passable = null) {
		passable = passable ?? (h => h.data.passable.current);
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

}


#if UNITY_EDITOR

[CustomEditor(typeof(TileGrid))]
public class TileGridEditor : Editor {

	TileGrid t => (TileGrid)target;

	protected virtual void OnSceneGUI() {
		if (!t.drawHexes) return;
		foreach (var kv in t.tiles) {
			var tile = kv.Value;
			var fillColor = (tile == null || tile.data == null) ? Color.magenta : (tile.data.passable.current ? (
				Color.Lerp(
					Saturate(Whitener(Color.green, 0.75f), tile.data.appeal.current * -0.08f),
					Saturate(Whitener(Color.red, 0.75f), tile.data.appeal.current * -0.08f),
					tile.data.moveCost.current / 10f
				)
			) : Color.black);
			if (tile) {
				using (ColorScope(fillColor)) {
					Handles.DrawAAConvexPolygon(tile.corners);
				}
				using (ColorScope(Color.black)) {
					Handles.DrawAAPolyLine(tile.corners);
				}
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