
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

public class TileGrid : MonoBehaviour, ISerializationCallbackReceiver {

	[field: SerializeField]
	public Vector2Int size { get; private set; }

	[FormerlySerializedAs("tile")]
	public TileData defaultTile;
	public EdgeData defaultEdge;

	public Dictionary<Vector3Int, Tile> tiles = new Dictionary<Vector3Int, Tile>();

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
				var go = MasterComponent.Instantiate(defaultTile, Layout.HexToPoint(hex).xxy().SetY(defaultTile.instantiatee.transform.position.y));
				go.transform.parent = transform;
				go.name = $"Tile ({x}, {y})";
				var tile = go.GetComponent<Tile>();
				tiles.Add(hex.pos, tile);
			}
		}

		foreach (var kv in tiles) {
			var tile = kv.Value;
			var hex = tile.hex;
			for (int i = 0; i < Hex.neighborOffsets.Length; i++) {
				var offset = Hex.neighborOffsets[i];
				var offsetHex = Hex.Add(hex, offset);
				if (tiles.TryGetValue(offsetHex.pos, out var neighbor))
					tile.SetNeighbor((TileDir)i, neighbor);
			}
			for (int i = 0; i < tile.edges.Length; i++) {
				var edge = tile.edges[i];
				if (edge == null) {
					var nbr = tile.GetNeighbor(i);
					var pos = (tile.corners[i] + tile.corners[new CircularInt(i + 1, 6)]) / 2;
					var ego = MasterComponent.Instantiate<Edge>(defaultEdge, pos);
					ego.transform.parent = transform;
					ego.name = $"Edge ({tile.hex.x}, {tile.hex.y})" + (nbr == null ? $" {((TileDir)i).ToString("g")}" : $" - ({nbr.hex.x}, {nbr.hex.y})");
					edge = ego.GetComponent<Edge>();
					tile.SetEdge(i, edge);
				}
				foreach (var edgeSource in tile.tileData.edgeModifiers) {
					edge.AddDataComponent<EdgeModifier>(edgeSource, v => v.Init(tile));
				}
			}
		}
#if UNITY_EDITOR
		foreach (Transform child in transform) {
			SceneVisibilityManager.instance.DisablePicking(child.gameObject, false);
		}
#endif
	}

	public Tile ReplaceTile(Hex hex, TileData dataSource) {
		DestroyTile(hex);
		return CreateTile(hex, dataSource);
	}

	public bool DestroyTile(Hex hex) {
		if (tiles.TryGetValue(hex.pos, out var tile)) {
			foreach (var edge in tile.edges) edge.RemoveModifiersByContext(tile);
			ObjectUtil.Destroy(tile.gameObject);
			tiles.Remove(hex.pos);
			return true;
		}
		return false;
	}

	public Tile CreateTile(Hex hex, TileData dataSource) {
		if (tiles.ContainsKey(hex.pos)) throw new ArgumentException("Hex already contains a Tile.");
		var go = MasterComponent.Instantiate(dataSource, Layout.HexToPoint(hex).xxy().SetY(dataSource.instantiatee.transform.position.y),
			v => {
				Debug.Assert(!(v as Tile).awoken);
			}
		);
		go.transform.parent = transform;
		go.name = $"Tile ({hex.x}, {hex.y})";
		var tile = go.GetComponent<Tile>();
		tiles.Add(hex.pos, tile);

		for (int i = 0; i < Hex.neighborOffsets.Length; i++) {
			var offset = Hex.neighborOffsets[i];
			var offsetHex = Hex.Add(hex, offset);
			if (tiles.TryGetValue(offsetHex.pos, out var neighbor))
				tile.SetNeighbor((TileDir)i, neighbor);
		}
		for (int i = 0; i < tile.edges.Length; i++) {
			var edge = tile.edges[i];
			if (edge == null) {
				var nbr = tile.GetNeighbor(i);
				if (nbr && (edge = nbr.GetEdge(new CircularInt(i + 3, 6))) != null) {
					tile.SetEdge(i, edge);
				} else {
					var pos = (tile.corners[i] + tile.corners[new CircularInt(i + 1, 6)]) / 2;
					var ego = MasterComponent.Instantiate<Edge>(defaultEdge, pos);
					ego.transform.parent = transform;
					ego.name = $"Edge ({tile.hex.x}, {tile.hex.y})" + (nbr == null ? $" {((TileDir)i).ToString("g")}" : $" - ({nbr.hex.x}, {nbr.hex.y})");
					edge = ego.GetComponent<Edge>();
					tile.SetEdge(i, edge);
				}
			}
			foreach (var edgeSource in tile.tileData.edgeModifiers) {
				edge.AddDataComponent<EdgeModifier>(edgeSource, v => v.Init(tile));
			}
		}
#if UNITY_EDITOR
		SceneVisibilityManager.instance.DisablePicking(tile.gameObject, false);
		foreach (var edge in tile.edges) SceneVisibilityManager.instance.DisablePicking(edge.gameObject, false);
#endif
		return tile;
	}

	public void DestroyGrid() {
		foreach (Transform child in transform.Cast<Transform>().ToList()) {
			if (child) { // Can be already destroyed by Tile OnDestroy
				ObjectUtil.Destroy(child.gameObject);
			}
		}
		tiles.Clear();
	}

	/// <summary>
	/// Iterates in a radius around hex.
	/// </summary>
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
	/// Returns an IEnumerable that enumerates Tiles in a line from start to end.
	/// </summary>
	/// <remarks>
	/// Tiles outside the grid will be returned as null.
	/// </remarks>
	/// <param name="start">Start Hex</param>
	/// <param name="end">End Hex</param>
	/// <param name="altNudge">Use alternate opposite nudge direction? Edge cases will be nudged the other way.</param>
	public IEnumerable<Tile> Line(Hex start, Hex end, bool altNudge = false) {
		int dist = Hex.Distance(start, end);
		var xNudge = altNudge ? -1e-06f : 1e-06f;
		var yNudge = altNudge ? -1e-06f : 1e-06f;
		var zNudge = altNudge ? +2e-06f : -2e-06f;
		FractHex startNudge = new FractHex(start.x + xNudge, start.y + yNudge, start.z + zNudge);
		FractHex endNudge = new FractHex(end.x + xNudge, end.y + yNudge, end.z + zNudge);
		for (int i = 0; i < dist; i++) {
			var hex = Hex.Lerp(startNudge, endNudge, 1f / dist * i);
			tiles.TryGetValue(hex.Round().pos, out var ghRes1);
			yield return ghRes1;
		}
		tiles.TryGetValue((Vector3Int)end.pos, out var ghRes2);
		yield return ghRes2;
	}

	/// <summary>
	/// Returns true if tile has pseudo-direct vision of target.
	/// </summary>
	/// <remarks> The tile and target is sampled and <c>Distance(tile, target) - 1</c> amount of samples are taken between tile and target. </remarks>
	/// <param name="tile">The sighting Tile</param>
	/// <param name="target">The sighted Tile</param>
	/// <param name="predicate">Predicate which determines if a Tile is see-through.</param>
	public bool HasSight(Hex hex, Hex target, Predicate<Tile> predicate = null) {
		predicate = predicate ?? (h => h.tileData.transparent.value);
		int nudge = 0;
		var lineA = Line(hex, target, false).GetEnumerator();
		var lineB = Line(hex, target, true).GetEnumerator();

		while (lineA.MoveNext() && lineB.MoveNext()) {
			var a = lineA.Current;
			var b = lineB.Current;
			switch (nudge) {
				case 1: {
						if (Transparent(a)) continue;
						else return false;
					}
				case 2: {
						if (Transparent(b)) continue;
						else return false;
					}
				default: {
						var aPass = Transparent(a);
						var bPass = Transparent(b);
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

		bool Transparent(Tile a) {
			return a == null || predicate(a);
		}
	}

	/// <summary>
	/// Iterates in a radius around hex and yields visible Tiles.
	/// </summary>
	/// <param name="seeThrough">Predicate which determines if a Tile is see-through.</param>
	public IEnumerable<Tile> Vision(Hex hex, int range, Predicate<Tile> seeThrough = null) {
		var radius = Radius(hex, range);
		var visible = new HashSet<Tile>();
		foreach (var radiusTile in radius) {
			if (HasSight(hex, radiusTile, seeThrough) && visible.Add(radiusTile)) yield return radiusTile;
		}
	}

	/// <summary>
	/// Finds blocked of areas and returns the area id for all Tiles.
	/// </summary>
	/// <param name="passable">Predicate which determines if a Tile is passable and thus included in the search.</param>
	/// <returns>Dictionary of Tile areas. Key: Tile, Value: Area id</returns>
	public Dictionary<Tile, int> GetAreas(Predicate<Tile> passable = null) {
		passable = passable ?? (h => h.tileData.passable.value);
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
		passable = passable ?? (h => h.tileData.passable.value);
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

[CustomEditor(typeof(TileGrid))]
public class GameGridEditor : Editor {

	TileGrid t => (TileGrid)target;

	protected virtual void OnSceneGUI() {
		var a = typeof(EditorGUI).GetMethod("HasVisibleChildFields", BindingFlags.NonPublic | BindingFlags.Static);
		var b = a.GetParameters();
		foreach (var kv in t.tiles) {
			var tile = kv.Value;
			var fillColor = (tile == null || tile.tileData == null) ? Color.magenta : (tile.tileData.passable.value ? (
				Color.Lerp(
					Saturate(Whitener(Color.green, 0.75f), tile.tileData.appeal.value * -0.08f),
					Saturate(Whitener(Color.red, 0.75f), tile.tileData.appeal.value * -0.08f),
					tile.tileData.moveCost.value / 10f
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