

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using System;
using System.Linq;
using Debug = UnityEngine.Debug;
using Muc.Extensions;

[RequireComponent(typeof(TileGrid))]
public class GridTester : MonoBehaviour {

	[field: SerializeField]
	public TileGrid grid { get; private set; }

	// Editor
	public Tile main;
	public Tile hover;

	public Draw draw;
	public enum Draw {
		None,
		Line, Radius, Ring, Spiral,
		Nearest, NearestSpiralSearch,
		CostField, DistanceField,
		Areas, Flood, Vision,
		CheapestPath, ShortestPath, FirstPath,
	}

	public bool paint;
	public TileData paintTile;

	void OnValidate() {
		grid = GetComponent<TileGrid>();
	}
}



#if UNITY_EDITOR
[CustomEditor(typeof(GridTester))]
public class GridTesterEditor : Editor {

	GridTester t => (GridTester)target;
	TileGrid grid => t.grid;


	protected virtual void OnSceneGUI() {

		if (t.main == null || !grid.tiles.ContainsValue(t.main)) t.main = grid.tiles.First().Value;
		if (t.hover == null || !grid.tiles.ContainsValue(t.hover)) t.hover = grid.tiles.First().Value;
		if (!t.main) return;

		var distance = Hex.Distance(t.hover.hex, t.main.hex);
		var lightBlue = ChangeAlpha(Color.blue, 0.25f);

		// Draw selection
		if (grid != null) {
			DrawTile(t.main, ChangeAlpha(Color.red, 0.25f));
		}

		// Draw hover
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hex = grid.RaycastHex(ray);
		var hovered = grid.RaycastTile(ray);

		var mousePos = Vector2.zero;
		if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
			mousePos = (ray.origin + ray.direction * enter).xz();

		if (hovered) {
			t.hover = hovered;
			DrawTile(t.hover, ChangeAlpha(Color.yellow, 0.25f));
			Event e = Event.current;
			if (e.type == EventType.MouseDown && e.button == 0) { t.main = t.hover; e.Use(); }
		}

		if (t.draw == GridTester.Draw.Line) {
			foreach (var tile in grid.Line(t.main, t.hover)) {
				DrawTile(tile, lightBlue);
			}
			foreach (var pair in Hex.Line(t.main.hex, t.hover.hex)) {
				var fractHex = pair.Item2;
				var ws = Layout.HexToPoint(fractHex);
				var spherePos = new Vector3(ws.x, 0, ws.y);
				Handles.SphereHandleCap(0, spherePos, Quaternion.identity, 0.25f, EventType.Repaint);
			}
		}
		if (t.draw == GridTester.Draw.Radius) {
			foreach (var tile in grid.Radius(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.draw == GridTester.Draw.Ring) {
			foreach (var tile in grid.Ring(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.draw == GridTester.Draw.Spiral) {
			var i = 0;
			var total = 3 * Mathf.Pow((distance + 1), 2) - 3 * (distance + 1) + 1;
			foreach (var tile in grid.Spiral(t.main, distance)) {
				var color = Color.Lerp(Color.green, Color.red, i++ / total);
				DrawTile(tile, color);
			}
		}

		if (t.draw == GridTester.Draw.Nearest) {
			var nearest = grid.NearestTile(mousePos);
			DrawTile(nearest, lightBlue);
		}
		if (t.draw == GridTester.Draw.NearestSpiralSearch) {
			var nearest = grid.NearestTileSpiralSearch(mousePos, 10);
			DrawTile(nearest, lightBlue);
		}

		if (t.draw == GridTester.Draw.CostField) {
			var field = Pathing.GetCostField(t.main);
			foreach (var kv in field.costs) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
		}
		if (t.draw == GridTester.Draw.DistanceField) {
			var field = Pathing.GetDistanceField(t.main);
			foreach (var kv in field.distances) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
		}

		if (t.draw == GridTester.Draw.Flood) {
			foreach (var tile in grid.Flood(t.main)) DrawTile(tile, lightBlue);
		}
		if (t.draw == GridTester.Draw.Areas) {
			var areas = grid.GetAreas();
			foreach (var area in areas) {
				var r = new System.Random(area.Value);
				var color = new Color(r.Next(64, 256) / 256f, r.Next(64, 256) / 256f, r.Next(64, 256) / 256f);
				DrawTile(area.Key, color);
			}
		}
		if (t.draw == GridTester.Draw.Vision) {
			foreach (var tile in grid.Vision(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}

		if (t.draw == GridTester.Draw.CheapestPath) {
			Pathing.CheapestPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}
		if (t.draw == GridTester.Draw.ShortestPath) {
			Pathing.ShortestPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}
		if (t.draw == GridTester.Draw.FirstPath) {
			Pathing.FirstPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}


		if (t.paint && t.paintTile != null && t.main.source != t.paintTile) {
			t.main = Game.grid.ReplaceTile(t.main.hex, t.paintTile);
		}

	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		Tile btnTile = null;
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.UpLeft)))) if (GUILayout.Button(nameof(TileDir.UpLeft))) t.main = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.UpRight)))) if (GUILayout.Button(nameof(TileDir.UpRight))) t.main = btnTile;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.Left)))) if (GUILayout.Button(nameof(TileDir.Left))) t.main = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.Right)))) if (GUILayout.Button(nameof(TileDir.Right))) t.main = btnTile;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.DownLeft)))) if (GUILayout.Button(nameof(TileDir.DownLeft))) t.main = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.DownRight)))) if (GUILayout.Button(nameof(TileDir.DownRight))) t.main = btnTile;
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledGroupScope(true)) {
			var dist = t.hover && t.main ? Hex.Distance(t.hover.hex, t.main.hex) : 0;
			EditorGUILayout.IntField("Distance", dist);
		}
	}

	private void DrawTile(Tile tile, Color color) {
		if (!tile || tile.corners == null) return;
		var wsCorners = tile.corners.Select(v => (Vector3)v).ToList();
		wsCorners.Add(wsCorners[0]);
		using (ColorScope(color)) {
			Handles.DrawAAConvexPolygon(wsCorners.ToArray());
		}
		using (ColorScope(Color.black)) {
			Handles.DrawAAPolyLine(wsCorners.ToArray());
		}
	}

	public override bool RequiresConstantRepaint() => true;

	private Deferred ColorScope(Color color) {
		var prevColor = Handles.color;
		Handles.color = color;
		return new Deferred(() => Handles.color = prevColor);
	}

	private Color ChangeAlpha(Color color, float alpha) {
		color.a = alpha;
		return color;
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