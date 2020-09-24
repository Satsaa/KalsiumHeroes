
namespace HexGrid {

  using System;
  using UnityEngine;

  [System.Serializable]
  internal struct FractHex {

    public FractHex(Hex hex) : this(hex.x, hex.y, hex.z) { }
    public FractHex(Vector2 pos) : this(pos.x, pos.y, -pos.y - pos.x) { }
    public FractHex(Vector3 pos) : this(pos.x, pos.y, pos.z) { }
    public FractHex(float x, float y) : this(x, y, -y - x) { }
    public FractHex(float x, float y, float z) {
      this.x = x;
      this.y = y;
      this.z = z;
      if (Mathf.Round(x + y + z) != 0) throw new ArgumentException($"The added total of {nameof(x)}, {nameof(y)} and {nameof(z)} must round to zero");
    }

    [field: SerializeField] public float x { get; private set; }
    [field: SerializeField] public float y { get; private set; }
    [field: SerializeField] public float z { get; private set; }
    public Vector3 pos => new Vector3(x, y, z);

    public Hex Round() {
      int qi = Mathf.RoundToInt(x);
      int ri = Mathf.RoundToInt(y);
      int si = Mathf.RoundToInt(z);
      float q_diff = Math.Abs(qi - x);
      float r_diff = Math.Abs(ri - y);
      float s_diff = Math.Abs(si - z);
      if (q_diff > r_diff && q_diff > s_diff) {
        qi = -ri - si;
      } else
          if (r_diff > s_diff) {
        ri = -qi - si;
      } else {
        si = -qi - ri;
      }
      return new Hex(qi, ri, si);
    }


    public FractHex HexLerp(FractHex b, float t) {
      return new FractHex(x * (1f - t) + b.x * t, y * (1f - t) + b.y * t, z * (1f - t) + b.z * t);
    }
  }
}