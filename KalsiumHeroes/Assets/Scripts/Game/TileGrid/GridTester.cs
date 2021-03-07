

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

	public Tile main;
	public Tile hover;
	[HideInInspector] public Hex mainHex;
	[HideInInspector] public Hex hoverHex;

	public Draw draw;
	public enum Draw {
		None,
		Line, SmartLine,
		Radius, Ring, Spiral,
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
	Color lightBlue => ChangeAlpha(Color.blue, 0.25f);

	protected virtual void OnSceneGUI() {

		if (!grid) return;

		var ray = HandleUtility.GUIPointToWorldRay(UnityEngine.Event.current.mousePosition);
		t.hoverHex = grid.RaycastHex(ray);

		var mousePos = Vector2.zero;
		if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out float enter))
			mousePos = (ray.origin + ray.direction * enter).xz();

		t.hover = grid.GetTile(t.hoverHex);
		t.main = grid.GetTile(t.mainHex);

		UnityEngine.Event e = UnityEngine.Event.current;
		if (e.type == EventType.MouseDown && e.button == 0) {
			t.mainHex = t.hoverHex;
			e.Use();
		}

		var distance = Hex.Distance(t.hoverHex, t.mainHex);
		DrawHex(t.mainHex, ChangeAlpha(Color.red, 0.25f));
		DrawHex(t.hoverHex, ChangeAlpha(Color.yellow, 0.25f));


		if ((t.draw == GridTester.Draw.Line || t.draw == GridTester.Draw.SmartLine)) {
			foreach (var tile in t.draw == GridTester.Draw.Line ? grid.Line(t.mainHex, t.hoverHex) : grid.SmartLine(t.mainHex, t.hoverHex)) {
				DrawTile(tile, lightBlue);
			}
			foreach (var pair in Hex.Line(t.mainHex, t.hoverHex)) {
				var fractHex = pair.Item2;
				var ws = Layout.HexToPoint(fractHex);
				var spherePos = new Vector3(ws.x, 0, ws.y);
				Handles.SphereHandleCap(0, spherePos, Quaternion.identity, 0.25f, EventType.Repaint);
			}
		}
		if (t.draw == GridTester.Draw.Radius) {
			foreach (var tile in grid.Radius(t.mainHex, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.draw == GridTester.Draw.Ring) {
			foreach (var tile in grid.Ring(t.mainHex, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.draw == GridTester.Draw.Spiral) {
			var i = 0;
			var total = 3 * Mathf.Pow((distance + 1), 2) - 3 * (distance + 1) + 1;
			foreach (var tile in grid.Spiral(t.mainHex, distance)) {
				var color = Color.Lerp(Color.green, Color.red, i++ / total);
				DrawTile(tile, color);
			}
		}

		if (t.draw == GridTester.Draw.Nearest) {
			var nearest = grid.NearestTile(mousePos);
			if (nearest) DrawTile(nearest, lightBlue);
		}
		if (t.draw == GridTester.Draw.NearestSpiralSearch) {
			var nearest = grid.NearestTileSpiralSearch(mousePos, 10);
			if (nearest) DrawTile(nearest, lightBlue);
		}

		if (t.draw == GridTester.Draw.CostField && t.main != null) {
			var result = Pathing.GetCostField(t.main);
			foreach (var kv in result.tiles) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value.cost / 15f));
		}
		if (t.draw == GridTester.Draw.DistanceField && t.main != null) {
			var result = Pathing.GetDistanceField(t.main);
			foreach (var kv in result.tiles) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value.cost / 15f));
		}

		if (t.draw == GridTester.Draw.Flood && t.main != null) {
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
			foreach (var tile in grid.Vision(t.mainHex, distance)) {
				DrawTile(tile, lightBlue);
			}
		}

		if (t.draw == GridTester.Draw.CheapestPath && t.main != null && t.hover != null) {
			Pathing.CheapestPath(t.main, t.hover, out var result);
			foreach (var kv in result.tiles) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value.cost / 15f));
			foreach (var segment in result.path) DrawTile(segment, Color.blue);
		}
		if (t.draw == GridTester.Draw.ShortestPath && t.main != null && t.hover != null) {
			Pathing.ShortestPath(t.main, t.hover, out var result);
			foreach (var kv in result.tiles) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value.cost / 15f));
			foreach (var segment in result.path) DrawTile(segment, Color.blue);
		}
		if (t.draw == GridTester.Draw.FirstPath && t.main != null && t.hover != null) {
			Pathing.FirstPath(t.main, t.hover, out var result);
			foreach (var kv in result.tiles) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value.cost / 15f));
			foreach (var segment in result.path) DrawTile(segment, Color.blue);
		}


		if (t.paint) {
			if (t.paintTile == null) {
				Game.grid.DestroyTile(t.mainHex);

#if UNITY_EDITOR
				foreach (Transform child in Game.game.transform) SceneVisibilityManager.instance.DisablePicking(child.gameObject, false);
#endif
			} else if (!t.main || t.main.source != t.paintTile) {
				t.main = Game.grid.ReplaceTile(t.mainHex, t.paintTile);

#if UNITY_EDITOR
				foreach (Transform child in Game.game.transform) SceneVisibilityManager.instance.DisablePicking(child.gameObject, false);
#endif
			}
		}

	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		Tile btnTile = null;
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.UpLeft)))) if (GUILayout.Button(nameof(TileDir.UpLeft))) t.mainHex = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.UpRight)))) if (GUILayout.Button(nameof(TileDir.UpRight))) t.mainHex = btnTile;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.Left)))) if (GUILayout.Button(nameof(TileDir.Left))) t.mainHex = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.Right)))) if (GUILayout.Button(nameof(TileDir.Right))) t.mainHex = btnTile;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.DownLeft)))) if (GUILayout.Button(nameof(TileDir.DownLeft))) t.mainHex = btnTile;
			using (new EditorGUI.DisabledGroupScope(!t.main || !(btnTile = t.main.GetNeighbor(TileDir.DownRight)))) if (GUILayout.Button(nameof(TileDir.DownRight))) t.mainHex = btnTile;
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledGroupScope(true)) {
			var dist = Hex.Distance(t.hoverHex, t.mainHex);
			EditorGUILayout.IntField("Distance", dist);
		}
	}

	private void DrawHex(Hex hex, Color color) {
		var wsCorners = Layout.Corners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToList();
		wsCorners.Add(wsCorners.First());
		var array = wsCorners.ToArray();
		using (ColorScope(color)) {
			Handles.DrawAAConvexPolygon(array);
		}
		using (ColorScope(Color.black)) {
			Handles.DrawAAPolyLine(array);
		}
	}

	private Vector3[] noAllocArray = new Vector3[7];
	private void DrawTile(Tile tile, Color color) {
		tile.corners.CopyTo(noAllocArray, 0);
		noAllocArray[6] = noAllocArray[0];
		using (ColorScope(color)) {
			Handles.DrawAAConvexPolygon(noAllocArray);
		}
		using (ColorScope(Color.black)) {
			Handles.DrawAAPolyLine(noAllocArray);
		}
	}

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