
namespace GameGrid {

  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using Muc.Extensions;
  using HexGrid;

  public class GameGrid : MonoBehaviour {

    [field: SerializeField]
    public Vector2Int size { get; private set; }

    public Dictionary<Vector3Int, GameHex> hexes;

    void OnValidate() {
      ResetGrid();
    }

    void ResetGrid() {
      hexes = new Dictionary<Vector3Int, GameHex>();
      for (int x = 0; x < size.x; x++) {
        int yOffset = Mathf.FloorToInt(x / 2);
        for (int y = -yOffset; y < size.y - yOffset; y++) {
          var hex = new Hex(x, y);
          hexes.Add(hex.pos, GameHex.Create(hex));
        }
      }
    }

    public GameHex RaycastHex(Ray ray) {
      var plane = new Plane(Vector3.up, Vector3.zero);
      if (plane.Raycast(ray, out float enter)) {
        var point = (ray.origin + ray.direction * enter).xy();
        var hex = Layout.PixelToHex(point).Round();
        hexes.TryGetValue(hex.pos, out var res);
        return res;
      }
      return default;
    }

  }
}

#if UNITY_EDITOR
namespace GameGrid {

  using System.Linq;
  using UnityEngine;
  using UnityEditor;

  [CustomEditor(typeof(GameGrid))]
  public class GameGridEditor : Editor {

    GameGrid t => (GameGrid)target;

    protected virtual void OnSceneGUI() {
      foreach (var kv in t.hexes) {
        var hex = kv.Value;
        Handles.DrawAAConvexPolygon(hex.corners);
        var prevColor = Handles.color;
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(hex.corners);
        Handles.color = prevColor;
      }
    }

    public override void OnInspectorGUI() {

      DrawDefaultInspector();

    }
  }
}
#endif