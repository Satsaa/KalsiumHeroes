

namespace GameGrid {

  using System.Collections.Generic;
  using System.Linq;
  using UnityEngine;

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
}


#if UNITY_EDITOR
namespace GameGrid {

  using System.Linq;
  using System;

  using UnityEngine;
  using UnityEditor;

  using Muc.Extensions;

  using HexGrid;

  [CustomEditor(typeof(GridTester))]
  public class GridTesterEditor : Editor {

    GridTester t => (GridTester)target;
    GameGrid grid => t.grid;


    protected virtual void OnSceneGUI() {

      int i = 0;
      int maxIters = 1000;

      // Draw selection
      if (grid) {
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
    }

    public override void OnInspectorGUI() {

      DrawDefaultInspector();

      t.highlights.Clear();

      using (new EditorGUILayout.HorizontalScope()) {
        using (new EditorGUI.DisabledGroupScope(t.main.down == null)) if (GUILayout.Button(nameof(t.main.down))) t.main = t.main.down ? t.main.down : t.main;
        using (new EditorGUI.DisabledGroupScope(t.main.downLeft == null)) if (GUILayout.Button(nameof(t.main.downLeft))) t.main = t.main.downLeft ? t.main.downLeft : t.main;
        using (new EditorGUI.DisabledGroupScope(t.main.downRight == null)) if (GUILayout.Button(nameof(t.main.downRight))) t.main = t.main.downRight ? t.main.downRight : t.main;
        using (new EditorGUI.DisabledGroupScope(t.main.up == null)) if (GUILayout.Button(nameof(t.main.up))) t.main = t.main.up ? t.main.up : t.main;
        using (new EditorGUI.DisabledGroupScope(t.main.upRight == null)) if (GUILayout.Button(nameof(t.main.upRight))) t.main = t.main.upRight ? t.main.upRight : t.main;
        using (new EditorGUI.DisabledGroupScope(t.main.upLeft == null)) if (GUILayout.Button(nameof(t.main.upLeft))) t.main = t.main.upLeft ? t.main.upLeft : t.main;
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
}