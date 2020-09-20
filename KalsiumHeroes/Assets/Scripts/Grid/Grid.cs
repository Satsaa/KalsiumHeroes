using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Grid : MonoBehaviour {

  [field: SerializeField]
  public Vector2Int size { get; private set; }

  public Dictionary<Vector2Int, Hex> hexes;

  void OnValidate() {
    CreateGrid();
  }

  void CreateGrid() {
    hexes = new Dictionary<Vector2Int, Hex>();
    for (int x = 0; x < size.y; x++) {
      int r_offset = Mathf.FloorToInt(x / 2);
      for (int y = -r_offset; y < size.x - r_offset; y++) {
        var hex = new Hex(y, x, -y - x);
        hexes.Add(new Vector2Int(x, y), hex);
      }
    }
  }

  void Start() {

  }

  void Update() {

  }

}

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

  Grid t => (Grid)target;

  protected virtual void OnSceneGUI() {
    foreach (var kv in t.hexes) {
      var corners = Layout.PolygonCorners(kv.Value);
      var wsCorners = corners.Select(v => (Vector3)v).ToList();
      wsCorners.Add(wsCorners[0]);
      Handles.DrawAAConvexPolygon(wsCorners.ToArray());
    }
  }
}