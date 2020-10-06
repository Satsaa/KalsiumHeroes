
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Muc.Extensions;
using HexGrid;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameGrid : MonoBehaviour, ISerializationCallbackReceiver {

  [field: SerializeField]
  public Vector2Int size { get; private set; }

  public Dictionary<Vector3Int, GameHex> hexes = new Dictionary<Vector3Int, GameHex>();

  internal void ResetGrid() {
    hexes = new Dictionary<Vector3Int, GameHex>();
    for (int x = 0; x < size.y; x++) {
      int yOffset = Mathf.FloorToInt(x / 2);
      for (int y = -yOffset; y < size.x - yOffset; y++) {
        var hex = new Hex(y, x);
        hexes.Add(hex.pos, GameHex.Create(hex));
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

  /// <summary> Iterates in a radius around a GameHex. </summary>
  public IEnumerable<GameHex> Radius(GameHex hex, int radius) {
    for (int x = -radius; x <= radius; x++) {
      for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(+radius, -x + radius); y++) {
        var pos = hex.hex.pos + new Vector3Int(x, y, -x - y);
        if (hexes.TryGetValue(pos, out var res)) yield return res;
      }
    }
  }

  /// <summary> Iterates in a Ring around a GameHex. </summary>
  public IEnumerable<GameHex> Ring(GameHex hex, int radius) {
    if (radius <= 0) {
      if (radius == 0) {
        yield return hex;
        yield break;
      }
      throw new ArgumentOutOfRangeException(nameof(radius));
    }

    var pos = hex.hex.pos + new Vector3Int(0, radius, -radius);
    for (int i = 0; i < 6; i++) {
      for (int j = 0; j < radius; j++) {
        if (hexes.TryGetValue(pos, out var res)) yield return res;
        pos = new Hex(pos).GetNeighbor(i).pos;
      }
    }
  }

  /// <summary> Iterates in a direction until a null GameHex is reached </summary>
  public IEnumerable<GameHex> Direction(GameHex hex, GameHex.Dir direction) {
    var current = hex;
    while (current != null) {
      yield return current;
      current = current.GetNeighbor(direction);
    }
  }

  /// <summary> Finds the fastest path from a GameHex to another </summary>
  /*
  public IEnumerable<GameHex> Path(GameHex hex, GameHex hex) {
  }
  */


  public GameHex RaycastHex(Ray ray, float planeHeight = 0) {
    var plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0));
    if (plane.Raycast(ray, out float enter)) {
      var point = (ray.origin + ray.direction * enter).xz();
      var hex = Layout.PixelToHex(point).Round();
      hexes.TryGetValue(hex.pos, out var res);
      return res;
    }
    return default;
  }


  [SerializeField, HideInInspector] List<Vector3Int> keyList = new List<Vector3Int>();
  [SerializeField, HideInInspector] List<GameHex> valList = new List<GameHex>();

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


#if UNITY_EDITOR

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
#endif