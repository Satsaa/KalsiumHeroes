

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

[RequireComponent(typeof(GameGrid))]
public class GridTester : MonoBehaviour {

  [field: SerializeField]
  public GameGrid grid { get; private set; }

  // Editor
  public GameHex main;
  public GameHex hover;

  [Space]

  public bool drawLine;
  public bool drawRadius;
  public bool drawRing;
  public bool drawVision;

  [Space]

  public bool drawCostField;
  public bool drawDistanceField;

  [Space]

  public bool drawArea;
  public bool drawFlood;

  [Space]

  public bool drawCheapestPath;
  public bool drawShortestPath;
  public bool drawFirstPath;

  [Space]

  public PaintType paint;

  public enum PaintType {
    None,
    NoWall,
    Wall,
    PositivinessN2,
    PositivinessN1,
    Positiviness0,
    Positiviness1,
    Positiviness2,
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
    if (main == null || !grid.hexes.ContainsValue(main)) main = grid.hexes.First().Value;
    if (hover == null || !grid.hexes.ContainsValue(hover)) hover = grid.hexes.First().Value;
  }


  void Start() {

  }

  void Update() {

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
      DrawHex(t.main, ChangeAlpha(Color.red, 0.25f));
    }

    // Draw hover
    var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    var hovered = grid.RaycastHex(ray);
    if (hovered) {
      t.hover = hovered;
      DrawHex(t.hover, ChangeAlpha(Color.yellow, 0.25f));
      Event e = Event.current;
      if (e.type == EventType.MouseDown && e.button == 0) { t.main = t.hover; e.Use(); }
    }

    if (t.drawLine) {
      foreach (var hex in grid.Line(t.main, t.hover)) {
        DrawHex(hex, lightBlue);
      }
      foreach (var pair in Hex.GetLine(t.main.hex, t.hover.hex)) {
        var fractHex = pair.Item2;
        var ws = Layout.HexToPixel(fractHex);
        var spherePos = new Vector3(ws.x, 0, ws.y);
        Handles.SphereHandleCap(0, spherePos, Quaternion.identity, 0.25f, EventType.Repaint);
      }
    }

    if (t.drawRadius) {
      foreach (var hex in grid.Radius(t.main, distance)) {
        DrawHex(hex, lightBlue);
      }
    }

    if (t.drawRing) {
      foreach (var hex in grid.Ring(t.main, distance)) {
        DrawHex(hex, lightBlue);
      }
    }

    if (t.drawVision) {
      foreach (var hex in grid.Vision(t.main, distance)) {
        DrawHex(hex, lightBlue);
      }
    }

    switch (t.paint) {
      case GridTester.PaintType.NoWall: t.main.blocked = false; break;
      case GridTester.PaintType.Wall: t.main.blocked = true; break;
      case GridTester.PaintType.PositivinessN2: t.main.positiviness = -2; break;
      case GridTester.PaintType.PositivinessN1: t.main.positiviness = -1; break;
      case GridTester.PaintType.Positiviness0: t.main.positiviness = 0; break;
      case GridTester.PaintType.Positiviness1: t.main.positiviness = 1; break;
      case GridTester.PaintType.Positiviness2: t.main.positiviness = 2; break;
      case GridTester.PaintType.MoveCost0: t.main.moveCost = 0; break;
      case GridTester.PaintType.MoveCost1: t.main.moveCost = 1; break;
      case GridTester.PaintType.MoveCost2: t.main.moveCost = 2; break;
      case GridTester.PaintType.MoveCost3: t.main.moveCost = 3; break;
      case GridTester.PaintType.MoveCost4: t.main.moveCost = 4; break;
      case GridTester.PaintType.MoveCost5: t.main.moveCost = 5; break;
      case GridTester.PaintType.MoveCost6: t.main.moveCost = 6; break;
      case GridTester.PaintType.MoveCost7: t.main.moveCost = 7; break;
      case GridTester.PaintType.MoveCost8: t.main.moveCost = 8; break;
      case GridTester.PaintType.MoveCost9: t.main.moveCost = 9; break;
      case GridTester.PaintType.MoveCost10: t.main.moveCost = 10; break;
    }

    if (t.drawCostField) {
      var field = grid.GetCostField(t.main);
      foreach (var kv in field.costs) DrawHex(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
    }
    if (t.drawDistanceField) {
      var field = grid.GetDistanceField(t.main);
      foreach (var kv in field.distances) DrawHex(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
    }

    if (t.drawFlood) {
      foreach (var hex in grid.Flood(t.main)) DrawHex(hex, lightBlue);
    }
    if (t.drawArea) {
      var areas = grid.GetAreas();
      foreach (var area in areas) {
        var r = new System.Random(area.Value);
        var color = new Color(r.Next(64, 256) / 256f, r.Next(64, 256) / 256f, r.Next(64, 256) / 256f);
        DrawHex(area.Key, color);
      }
    }

    if (t.drawCheapestPath) {
      grid.CheapestPath(t.main, t.hover, out var path, out var field);
      foreach (var kv in field.scores) DrawHex(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
      foreach (var segment in path) DrawHex(segment, Color.blue);
    }
    if (t.drawShortestPath) {
      grid.ShortestPath(t.main, t.hover, out var path, out var field);
      foreach (var kv in field.scores) DrawHex(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
      foreach (var segment in path) DrawHex(segment, Color.blue);
    }
    if (t.drawFirstPath) {
      grid.FirstPath(t.main, t.hover, out var path, out var field);
      foreach (var kv in field.scores) DrawHex(kv.Key, Color.Lerp(Color.green, Color.red, kv.Value / 15f));
      foreach (var segment in path) DrawHex(segment, Color.blue);
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

  private void DrawHex(GameHex hex, Color color) {
    var wsCorners = hex.corners.Select(v => (Vector3)v).ToList();
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
    return new Deferred(() => {
      Handles.color = prevColor;
    });
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
      if (onDispose != null)
        onDispose();
    }
  }
}
#endif