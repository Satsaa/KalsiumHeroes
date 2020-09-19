using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

[Serializable]
public struct Hex {

  [field: SerializeField] public int x { get; }
  [field: SerializeField] public int y { get; }
  [field: SerializeField] public int z { get; }

  public Hex up => new Hex(x, y, z);
  public Hex down => new Hex(x, y, z);

  public Hex upRight => new Hex(x, y, z);
  public Hex downRight => new Hex(x, y, z);
  public Hex upLeft => new Hex(x, y, z);
  public Hex downLeft => new Hex(x, y, z);

  /// <summary> Neighbors from downRight to upRight </summary>
  public IEnumerable<Hex> Neighbors() {
    yield return downRight;
    yield return down;
    yield return downLeft;
    yield return upLeft;
    yield return up;
    yield return upRight;
  }

  public Hex(int x, int y, int z) {
    this.x = x;
    this.y = y;
    this.z = z;
    if (x + y + z != 0) throw new ArgumentException($"The added total of {nameof(x)}, {nameof(y)} and {nameof(z)} must be zero");
  }

}