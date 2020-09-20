using System;
using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;

public partial struct Hex {

  /// <summary> Neighbors from downRight to upRight </summary>
  public static Hex[] neighbors = new Hex[] {
    new Hex(1, -1, 0),
    new Hex(0, -1, 1),
    new Hex(-1, 0, 1),
    new Hex(-1, 1, 0),
    new Hex(0, 1, -1),
    new Hex(1, 0, -1)
  };

  public static Vector3 Lerp(Vector3 a, Vector3 b, float t) {
    return new Vector3(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t), Mathf.LerpUnclamped(a.z, b.z, t));
  }

}
