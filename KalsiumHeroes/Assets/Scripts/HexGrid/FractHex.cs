
namespace HexGrid {

	using System;
	using UnityEngine;

	[System.Serializable]
	public struct FractHex {

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
			int xi = Mathf.RoundToInt(x);
			int yi = Mathf.RoundToInt(y);
			int zi = Mathf.RoundToInt(z);
			float xd = Math.Abs(xi - x);
			float yd = Math.Abs(yi - y);
			float zd = Math.Abs(zi - z);
			if (xd > yd && xd > zd) {
				xi = -yi - zi;
			} else if (yd > zd) {
				yi = -xi - zi;
			} else {
				zi = -xi - yi;
			}
			return new Hex(xi, yi, zi);
		}


		public FractHex HexLerp(FractHex b, float t) {
			return new FractHex(x * (1f - t) + b.x * t, y * (1f - t) + b.y * t, z * (1f - t) + b.z * t);
		}

		public override bool Equals(object obj) {
			if (obj is FractHex fhex) {
				return x == fhex.x && y == fhex.y && z == fhex.z;
			}
			return false;
		}

		public override int GetHashCode() {
			return unchecked(x.GetHashCode() * 17 + y.GetHashCode() * 17 + z.GetHashCode());
		}

	}
}