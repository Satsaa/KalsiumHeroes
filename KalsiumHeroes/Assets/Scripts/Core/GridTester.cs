

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

[RequireComponent(typeof(GameGrid))]
public class GridTester : MonoBehaviour {

	[field: SerializeField]
	public GameGrid grid { get; private set; }

	// Editor
	public Tile main;
	public Tile hover;

	[Space]

	public bool drawLine;
	public bool drawRadius;
	public bool drawRing;
	public bool drawSpiral;

	[Space]

	public bool drawNearest;
	public bool drawNearestSpiralSearch;

	[Space]

	public bool drawCostField;
	public bool drawDistanceField;

	[Space]

	public bool drawArea;
	public bool drawFlood;
	public bool drawVision;

	[Space]

	public bool drawCheapestPath;
	public bool drawShortestPath;
	public bool drawFirstPath;

	[Space]

	public PaintType paint;

	public enum PaintType {
		None,
		Passable,
		Impassable,
		AppealN2,
		AppealN1,
		Appeal0,
		Appeal1,
		Appeal2,
		MoveCost0,
		MoveCost1,
		MoveCost2,
		MoveCost3,
		MoveCost4,
		MoveCost5,
		MoveCost6,
		MoveCost7,
		MoveCost8,
		MoveCost9,
		MoveCost10,
	}

	void OnValidate() {
		grid = GetComponent<GameGrid>();
		if (main == null || !grid.tiles.ContainsValue(main)) main = grid.tiles.First().Value;
		if (hover == null || !grid.tiles.ContainsValue(hover)) hover = grid.tiles.First().Value;
	}
}



#if UNITY_EDITOR
[CustomEditor(typeof(GridTester))]
public class GridTesterEditor : Editor {

	GridTester t => (GridTester)target;
	GameGrid grid => t.grid;


	protected virtual void OnSceneGUI() {

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

		if (t.drawLine) {
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
		if (t.drawRadius) {
			foreach (var tile in grid.Radius(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.drawRing) {
			foreach (var tile in grid.Ring(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}
		if (t.drawSpiral) {
			var i = 0;
			var total = 3 * Mathf.Pow((distance + 1), 2) - 3 * (distance + 1) + 1;
			foreach (var tile in grid.Spiral(t.main, distance)) {
				var color = Color.Lerp(Color.green, Color.red, i++ / total);
				DrawTile(tile, color);
			}
		}

		if (t.drawNearest) {
			var nearest = grid.NearestTile(mousePos);
			DrawTile(nearest, lightBlue);
		}
		if (t.drawNearestSpiralSearch) {
			var nearest = grid.NearestTileSpiralSearch(mousePos, 10);
			DrawTile(nearest, lightBlue);
		}

		if (t.drawCostField) {
			var field = grid.GetCostField(t.main);
			foreach (var kv in field.costs) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
		}
		if (t.drawDistanceField) {
			var field = grid.GetDistanceField(t.main);
			foreach (var kv in field.distances) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
		}

		if (t.drawFlood) {
			foreach (var tile in grid.Flood(t.main)) DrawTile(tile, lightBlue);
		}
		if (t.drawArea) {
			var areas = grid.GetAreas();
			foreach (var area in areas) {
				var r = new System.Random(area.Value);
				var color = new Color(r.Next(64, 256) / 256f, r.Next(64, 256) / 256f, r.Next(64, 256) / 256f);
				DrawTile(area.Key, color);
			}
		}
		if (t.drawVision) {
			foreach (var tile in grid.Vision(t.main, distance)) {
				DrawTile(tile, lightBlue);
			}
		}

		if (t.drawCheapestPath) {
			grid.CheapestPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}
		if (t.drawShortestPath) {
			grid.ShortestPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}
		if (t.drawFirstPath) {
			grid.FirstPath(t.main, t.hover, out var path, out var field);
			foreach (var kv in field.scores) DrawTile(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
			foreach (var segment in path) DrawTile(segment, Color.blue);
		}


		switch (t.paint) {
			case GridTester.PaintType.Passable: t.main.tileData.passable.value = true; break;
			case GridTester.PaintType.Impassable: t.main.tileData.passable.value = false; break;
			case GridTester.PaintType.AppealN2: t.main.tileData.appeal.value = -2; break;
			case GridTester.PaintType.AppealN1: t.main.tileData.appeal.value = -1; break;
			case GridTester.PaintType.Appeal0: t.main.tileData.appeal.value = 0; break;
			case GridTester.PaintType.Appeal1: t.main.tileData.appeal.value = 1; break;
			case GridTester.PaintType.Appeal2: t.main.tileData.appeal.value = 2; break;
			case GridTester.PaintType.MoveCost0: t.main.tileData.moveCost.value = 0; break;
			case GridTester.PaintType.MoveCost1: t.main.tileData.moveCost.value = 1; break;
			case GridTester.PaintType.MoveCost2: t.main.tileData.moveCost.value = 2; break;
			case GridTester.PaintType.MoveCost3: t.main.tileData.moveCost.value = 3; break;
			case GridTester.PaintType.MoveCost4: t.main.tileData.moveCost.value = 4; break;
			case GridTester.PaintType.MoveCost5: t.main.tileData.moveCost.value = 5; break;
			case GridTester.PaintType.MoveCost6: t.main.tileData.moveCost.value = 6; break;
			case GridTester.PaintType.MoveCost7: t.main.tileData.moveCost.value = 7; break;
			case GridTester.PaintType.MoveCost8: t.main.tileData.moveCost.value = 8; break;
			case GridTester.PaintType.MoveCost9: t.main.tileData.moveCost.value = 9; break;
			case GridTester.PaintType.MoveCost10: t.main.tileData.moveCost.value = 10; break;
		}

	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector();

		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.upLeft == null)) if (GUILayout.Button(nameof(t.main.upLeft))) t.main = t.main.upLeft ? t.main.upLeft : t.main;
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.upRight == null)) if (GUILayout.Button(nameof(t.main.upRight))) t.main = t.main.upRight ? t.main.upRight : t.main;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.left == null)) if (GUILayout.Button(nameof(t.main.left))) t.main = t.main.left ? t.main.left : t.main;
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.right == null)) if (GUILayout.Button(nameof(t.main.right))) t.main = t.main.right ? t.main.right : t.main;
		}
		using (new EditorGUILayout.HorizontalScope()) {
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.downLeft == null)) if (GUILayout.Button(nameof(t.main.downLeft))) t.main = t.main.downLeft ? t.main.downLeft : t.main;
			using (new EditorGUI.DisabledGroupScope(!t.main || t.main.downRight == null)) if (GUILayout.Button(nameof(t.main.downRight))) t.main = t.main.downRight ? t.main.downRight : t.main;
		}

		EditorGUILayout.Space();
		using (new EditorGUI.DisabledGroupScope(true)) {
			var dist = Hex.Distance(t.hover.hex, t.main.hex);
			EditorGUILayout.IntField("Distance", dist);
		}
	}

	private void DrawTile(Tile tile, Color color) {
		if (!tile) return;
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