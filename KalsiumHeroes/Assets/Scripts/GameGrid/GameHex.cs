
namespace GameGrid {

  using System;
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using HexGrid;

  [Serializable]
  /// <summary>
  /// Use the static method Create to create GameHexes.
  /// </summary>
  public class GameHex : ScriptableObject {

    internal static GameHex Create(Hex hex) {
      var instance = ScriptableObject.CreateInstance<GameHex>();
      instance.hex = hex;
      var pix = Layout.HexToPixel(hex);
      instance.center = new Vector3(pix.x, 0, pix.y);
      instance.corners = Layout.PolygonCorners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
      return instance;
    }

    [field: SerializeField] public HexContent[] content { get; private set; }

    [field: SerializeField] internal Hex hex { get; private set; }

    [field: SerializeField] public Vector3 center { get; private set; }
    [field: SerializeField] public Vector3[] corners { get; private set; }

    [field: SerializeField] public GameHex downRight { get; internal set; }
    [field: SerializeField] public GameHex down { get; internal set; }
    [field: SerializeField] public GameHex downLeft { get; internal set; }
    [field: SerializeField] public GameHex upLeft { get; internal set; }
    [field: SerializeField] public GameHex up { get; internal set; }
    [field: SerializeField] public GameHex upRight { get; internal set; }

    public enum Dir {
      downRight,
      Down,
      DownLeft,
      UpLeft,
      Up,
      UpRight,
    }

    public GameHex GetNeighbor(Dir direction) {
      switch (direction) {
        case Dir.downRight: return downRight;
        case Dir.Down: return down;
        case Dir.DownLeft: return downLeft;
        case Dir.UpLeft: return upLeft;
        case Dir.Up: return up;
        case Dir.UpRight: return upRight;
        default: return null;
      }
    }

    internal void SetNeighbor(Dir direction, GameHex value) {
      switch (direction) {
        case Dir.downRight: downRight = value; break;
        case Dir.Down: down = value; break;
        case Dir.DownLeft: downLeft = value; break;
        case Dir.UpLeft: upLeft = value; break;
        case Dir.Up: up = value; break;
        case Dir.UpRight: upRight = value; break;
      }
    }

    public IEnumerable<GameHex> Neighbors() {
      if (downRight != null) yield return downRight;
      if (down != null) yield return down;
      if (downLeft != null) yield return downLeft;
      if (upLeft != null) yield return upLeft;
      if (up != null) yield return up;
      if (upRight != null) yield return upRight;
    }

    /// <summary> Iterates in a direction until a null GameHex is reached </summary>
    public IEnumerable<GameHex> Direction(Dir neighbor) {
      var current = this;
      while (current != null) {
        yield return current;
        current = current.GetNeighbor(neighbor);
      }

    }
  }
}
