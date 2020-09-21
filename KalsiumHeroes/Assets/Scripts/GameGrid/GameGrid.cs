
namespace GameGrid {

  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using Muc.Extensions;
  using HexGrid;

  public class GameGrid : MonoBehaviour, ISerializationCallbackReceiver {

    [field: SerializeField]
    public Vector2Int size { get; private set; }

    public Dictionary<Vector3Int, GameHex> hexes = new Dictionary<Vector3Int, GameHex>();

    [SerializeField, HideInInspector] List<Vector3Int> keyList = new List<Vector3Int>();
    [SerializeField, HideInInspector] List<GameHex> valList = new List<GameHex>();

    internal void ResetGrid() {
      hexes = new Dictionary<Vector3Int, GameHex>();
      for (int x = 0; x < size.x; x++) {
        int yOffset = Mathf.FloorToInt(x / 2);
        for (int y = -yOffset; y < size.y - yOffset; y++) {
          var hex = new Hex(x, y);
          hexes.Add(hex.pos, GameHex.Create(hex));
          hex = new Hex(x, y);
        }
      }

      foreach (var kv in hexes) {
        var gameHex = kv.Value;
        var hex = gameHex.hex;
        for (int i = 0; i < Hex.neighbors.Length; i++) {
          var offset = Hex.neighbors[i];
          var offsetHex = Hex.Add(hex, offset);
          if (hexes.TryGetValue(offsetHex.pos, out var neighbor))
            gameHex.SetNeighbor((GameHex.Dir)i, neighbor);
        }
      }
    }

    public GameHex RaycastHex(Ray ray) {
      var plane = new Plane(Vector3.up, Vector3.zero);
      if (plane.Raycast(ray, out float enter)) {
        var point = (ray.origin + ray.direction * enter).xz();
        var hex = Layout.PixelToHex(point).Round();
        hexes.TryGetValue(hex.pos, out var res);
        return res;
      }
      return default;
    }


    void ISerializationCallbackReceiver.OnBeforeSerialize() {
      keyList = hexes.Select(v => v.Key).ToList();
      valList = hexes.Select(v => v.Value).ToList();
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize() {
      hexes = new Dictionary<Vector3Int, GameHex>();
      for (int i = 0; i < keyList.Count; i++) {
        var key = keyList[i];
        var val = valList[i];
        hexes.Add(key, val);
      }
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
      if (GUILayout.Button(nameof(t.ResetGrid))) t.ResetGrid();
    }
  }
}
#endif