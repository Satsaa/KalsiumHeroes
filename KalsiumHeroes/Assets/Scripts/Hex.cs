using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

[Serializable]
public struct Hex {

  [field: SerializeField] public int x { get; }
  [field: SerializeField] public int y { get; }

  [field: SerializeField] public bool odd { get; }

  public Hex up => new Hex(x, y + 1);
  public Hex down => new Hex(x, y - 1);

  public Hex upRight => new Hex(x + 1, (odd ? y + 1 : y));
  public Hex downRight => new Hex(x + 1, (odd ? y : y - 1));
  public Hex upLeft => new Hex(x - 1, (odd ? y + 1 : y));
  public Hex downLeft => new Hex(x - 1, (odd ? y : y - 1));

  public Hex(int x, int y) {
    this.x = x;
    this.y = y;
    this.odd = x % 2 != 0;
  }
}