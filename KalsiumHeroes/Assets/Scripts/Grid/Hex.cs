using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

[Serializable]
public partial struct Hex {

  public Hex(int x, int y, int z) {
    this.x = x;
    this.y = y;
    this.z = z;
    if (x + y + z != 0) throw new ArgumentException($"The added total of {nameof(x)}, {nameof(y)} and {nameof(z)} must be zero");
  }

  [field: SerializeField] public int x { get; }
  [field: SerializeField] public int y { get; }
  [field: SerializeField] public int z { get; }

  public Hex up => new Hex(x, y + 1, z - 1);
  public Hex down => new Hex(x, y - 1, z + 1);

  public Hex upRight => new Hex(x + 1, y, z - 1);
  public Hex downRight => new Hex(x + 1, y - 1, z);
  public Hex upLeft => new Hex(x - 1, y + 1, z);
  public Hex downLeft => new Hex(x - 1, y, z + 1);

}
