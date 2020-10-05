

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System;
using UnityEditor;
using Muc.Extensions;
using HexGrid;
#endif

[RequireComponent(typeof(GameGrid))]
public class GridTester : MonoBehaviour {

  [field: SerializeField]
  public GameGrid grid { get; private set; }

  // Editor
  public HashSet<GameHex> highlights = new HashSet<GameHex>();
  public GameHex main;
  public GameHex hover;
  public bool drawLine;

  void OnValidate() {
    grid = GetComponent<GameGrid>();
    main = grid.hexes.First().Value;
    hover = grid.hexes.First().Value;
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

    int i = 0;
    int maxIters = 1000;

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
      if ((e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0) {
        t.main = t.hover;
        e.Use();
      }
    }


    // Draw highlights
    foreach (var hex in t.highlights) {
      if (i++ > maxIters) break;
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
}
#endif