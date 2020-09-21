
namespace GameGrid {

  using System;
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using HexGrid;

  [System.Serializable]
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

    // Content property 
    // [field: SerializeField] public HexContent content { get; private set; }

    [field: SerializeField] internal Hex hex { get; private set; }

    [field: SerializeField] public Vector3 center { get; private set; }
    [field: SerializeField] public Vector3[] corners { get; private set; }

    [field: SerializeField] public GameHex downRight { get; internal set; }
    [field: SerializeField] public GameHex down { get; internal set; }
    [field: SerializeField] public GameHex downLeft { get; internal set; }
    [field: SerializeField] public GameHex upLeft { get; internal set; }
    [field: SerializeField] public GameHex up { get; internal set; }
    [field: SerializeField] public GameHex upRight { get; internal set; }

    public enum Neighbor {
      downRight,
      Down,
      DownLeft,
      UpLeft,
      Up,
      UpRight,
    }

    public readonly int asd = 3;

    public GameHex GetNeighbor(Neighbor neighbor) {
      switch (neighbor) {
        case Neighbor.downRight: return downRight;
        case Neighbor.Down: return down;
        case Neighbor.DownLeft: return downLeft;
        case Neighbor.UpLeft: return upLeft;
        case Neighbor.Up: return up;
        case Neighbor.UpRight: return upRight;
        default: return null;
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

    public IEnumerable<GameHex> Direction(Neighbor neighbor) {
      var current = this;
      while (current != null) {
        yield return current;
        current = current.GetNeighbor(neighbor);
      }

    }
  }
}
