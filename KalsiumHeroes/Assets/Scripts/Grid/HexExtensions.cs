using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

[Serializable]
public static class HexExtensions {

  public static Hex GetNeighbor(this Hex a, int index) {
    return Add(a, Hex.neighbors[index]);
  }

  public static Hex Add(this Hex a, Hex b) {
    return new Hex(a.x + b.x, a.y + b.y, a.z + b.z);
  }

  public static int Distance(this Hex a, Hex b) {
    return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z) / 2;
  }

  public static IEnumerable<Hex> Linedraw(this Hex a, Hex b) {
    int fist = a.Distance(b);
    var a_nudge = new FractHex(a.x + 1e-06f, a.y + 1e-06f, a.z - 2e-06f);
    var b_nudge = new FractHex(b.x + 1e-06f, b.y + 1e-06f, b.z - 2e-06f);
    float step = 1f / Math.Max(fist, 1);
    for (int i = 0; i <= fist; i++) {
      yield return a_nudge.HexLerp(b_nudge, step * i).HexRound();
    }
  }


  public static IEnumerable<Hex> GetRange(this Hex a, int dist) {
    for (int x = -dist; x < dist; x++) {
      for (int y = Mathf.Max(-dist, -x - dist); y < Mathf.Min(+dist, -x + dist); y++) {
        var z = -x - y;
        yield return (a.Add(new Hex(x, y, z)));
      }
    }

  }
}
