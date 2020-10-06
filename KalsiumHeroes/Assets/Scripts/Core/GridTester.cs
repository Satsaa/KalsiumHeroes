

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Numerics;
using Muc.Extensions;
using HexGrid;
using System;

[RequireComponent(typeof(GameGrid))]
public class GridTester : MonoBehaviour {

  [field: SerializeField]
  public GameGrid grid { get; private set; }

  // Editor
  public HashSet<GameHex> highlights = new HashSet<GameHex>();
  public GameHex main;
  public GameHex hover;
  public bool drawLine;
  public bool drawRadius;
  public bool drawRing;
  public bool drawPath;
  public bool painting;
  public PaintType paintType;
  public int max;

  public enum PaintType {
    None,
    Wall,
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

    // Draw blockages
    foreach (var hex in grid.hexes.Values) {
      if (hex.blocked) DrawHex(hex, Color.black);
    }


    // Draw highlights
    foreach (var hex in t.highlights) {
      DrawHex(hex, Color.green);
    }

    if (t.drawLine) {
      foreach (var pair in Hex.GetLine(t.main.hex, t.hover.hex)) {
        var hex = pair.Item1;
        var fractHex = pair.Item2;

        var gameHex = grid.hexes[hex.pos];
        DrawHex(gameHex, ChangeAlpha(Color.blue, 0.25f));
        var ws = Layout.HexToPixel(fractHex);
        var spherePos = new Vector3(ws.x, 0, ws.y);
        Handles.SphereHandleCap(0, spherePos, Quaternion.identity, 0.25f, EventType.Repaint);
      }
    }

    if (t.drawRadius) {
      foreach (var gameHex in grid.Radius(t.main, Hex.Distance(t.hover.hex, t.main.hex))) {
        DrawHex(gameHex, ChangeAlpha(Color.blue, 0.25f));
      }
    }

    if (t.drawRing) {
      foreach (var gameHex in grid.Ring(t.main, Hex.Distance(t.hover.hex, t.main.hex))) {
        DrawHex(gameHex, ChangeAlpha(Color.blue, 0.25f));
      }
    }

    if (t.painting) {
      switch (t.paintType) {
        case GridTester.PaintType.None: t.main.blocked = false; break;
        case GridTester.PaintType.Wall: t.main.blocked = true; break;
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
    }

    if (t.drawPath) {
      DrawPath();
    }
  }

  public override void OnInspectorGUI() {

    DrawDefaultInspector();

    t.highlights.Clear();

    using (new EditorGUILayout.HorizontalScope()) {
      using (new EditorGUI.DisabledGroupScope(t.main.upLeft == null)) if (GUILayout.Button(nameof(t.main.upLeft))) t.main = t.main.upLeft ? t.main.upLeft : t.main;
      using (new EditorGUI.DisabledGroupScope(t.main.upRight == null)) if (GUILayout.Button(nameof(t.main.upRight))) t.main = t.main.upRight ? t.main.upRight : t.main;
    }
    using (new EditorGUILayout.HorizontalScope()) {
      using (new EditorGUI.DisabledGroupScope(t.main.left == null)) if (GUILayout.Button(nameof(t.main.left))) t.main = t.main.left ? t.main.left : t.main;
      using (new EditorGUI.DisabledGroupScope(t.main.right == null)) if (GUILayout.Button(nameof(t.main.right))) t.main = t.main.right ? t.main.right : t.main;
    }
    using (new EditorGUILayout.HorizontalScope()) {
      using (new EditorGUI.DisabledGroupScope(t.main.downLeft == null)) if (GUILayout.Button(nameof(t.main.downLeft))) t.main = t.main.downLeft ? t.main.downLeft : t.main;
      using (new EditorGUI.DisabledGroupScope(t.main.downRight == null)) if (GUILayout.Button(nameof(t.main.downRight))) t.main = t.main.downRight ? t.main.downRight : t.main;
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

  private void DrawPath() {
    var total = 0;

    var costs = new Dictionary<GameHex, int>() { { t.main, 0 } };
    var sources = new Dictionary<GameHex, GameHex>() { { t.main, null } };

    var fringes = new Dictionary<int, List<GameHex>>() { { 0, new List<GameHex>() { t.main } } };
    for (var k = 0; fringes[k].Count > 0; k++) {
      fringes[k + 1] = new List<GameHex>();
      foreach (var hex in fringes[k]) {
        foreach (var neighbor in hex.Neighbors()) {
          var cost = costs[hex] + neighbor.moveCost;
          if (!neighbor.blocked && (!costs.TryGetValue(neighbor, out var otherCost) || otherCost > cost)) {
            if (total++ >= t.max) goto skip;
            costs[neighbor] = cost;
            sources[neighbor] = hex;
            fringes[k + 1].Add(neighbor);
          }
        }
      }
    }
  skip:
    foreach (var kv in costs) {
      DrawHex(kv.Key, Color.Lerp(ChangeAlpha(Color.green, 0.25f), Color.red, kv.Value / 15f));
    }
    var drawn = new HashSet<GameHex>() { t.hover };
    DrawHex(t.hover, ChangeAlpha(Color.blue, 0.25f));
    if (sources.TryGetValue(t.hover, out var current)) {
      while (current != null && !drawn.Contains(current)) {
        DrawHex(current, ChangeAlpha(Color.blue, 0.25f));
        drawn.Add(current);
        sources.TryGetValue(current, out current);
      }
    }
  }
}
#endif