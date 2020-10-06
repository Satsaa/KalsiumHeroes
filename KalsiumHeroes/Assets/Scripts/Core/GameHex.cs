
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

  public bool blocked;
  public int moveCost;

  public static GameHex Create(Hex hex) {
    var instance = ScriptableObject.CreateInstance<GameHex>();
    instance.hex = hex;
    var pix = Layout.HexToPixel(hex);
    instance.center = new Vector3(pix.x, 0, pix.y);
    instance.corners = Layout.PolygonCorners(hex).Select(v => new Vector3(v.x, 0, v.y)).ToArray();
    return instance;
  }

  [field: SerializeField] public Unit unit { get; set; }

  [field: SerializeField] public Hex hex { get; private set; }

  [field: SerializeField] public Vector3 center { get; private set; }
  [field: SerializeField] public Vector3[] corners { get; private set; }

  [field: SerializeField] public GameHex downRight { get; internal set; }
  [field: SerializeField] public GameHex downLeft { get; internal set; }
  [field: SerializeField] public GameHex left { get; internal set; }
  [field: SerializeField] public GameHex upLeft { get; internal set; }
  [field: SerializeField] public GameHex upRight { get; internal set; }
  [field: SerializeField] public GameHex right { get; internal set; }

  public enum Dir {
    DownRight,
    DownLeft,
    Left,
    UpLeft,
    UpRight,
    Right,
  }

  public GameHex GetNeighbor(Dir direction) {
    switch (direction) {
      case Dir.DownRight: return downRight;
      case Dir.DownLeft: return downLeft;
      case Dir.Left: return left;
      case Dir.UpLeft: return upLeft;
      case Dir.UpRight: return upRight;
      case Dir.Right: return right;
      default: return null;
    }
  }

  public void SetNeighbor(Dir direction, GameHex value) {
    switch (direction) {
      case Dir.DownRight: downRight = value; break;
      case Dir.DownLeft: downLeft = value; break;
      case Dir.Left: left = value; break;
      case Dir.UpLeft: upLeft = value; break;
      case Dir.UpRight: upRight = value; break;
      case Dir.Right: right = value; break;
    }
  }

  public IEnumerable<GameHex> Neighbors() {
    if (downRight != null) yield return downRight;
    if (downLeft != null) yield return downLeft;
    if (left != null) yield return left;
    if (upLeft != null) yield return upLeft;
    if (upRight != null) yield return upRight;
    if (right != null) yield return right;
  }
}
