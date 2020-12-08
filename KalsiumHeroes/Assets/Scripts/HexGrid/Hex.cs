

namespace HexGrid {

	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public struct Hex {

		public Hex(Vector2Int pos) : this(pos.x, pos.y, -pos.y - pos.x) { }
		public Hex(Vector3Int pos) : this(pos.x, pos.y, pos.z) { }
		public Hex(int x, int y) : this(x, y, -y - x) { }
		public Hex(int x, int y, int z) {
			this.x = x;
			this.y = y;
			this.z = z;
			if (x + y + z != 0) throw new ArgumentException($"The added total of {nameof(x)}, {nameof(y)} and {nameof(z)} must be zero");
		}

		[field: SerializeField] public int x { get; private set; }
		[field: SerializeField] public int y { get; private set; }
		[field: SerializeField] public int z { get; private set; }
		public Vector3Int pos => new Vector3Int(x, y, z);

		public Hex _up => new Hex(x, y + 1, z - 1);
		public Hex _down => new Hex(x, y - 1, z + 1);

		public Hex _upRight => new Hex(x - 1, y + 1, z);
		public Hex _downRight => new Hex(x - 1, y, z + 1);
		public Hex _upLeft => new Hex(x + 1, y, z - 1);
		public Hex _downLeft => new Hex(x + 1, y - 1, z);

		public Hex GetNeighbor(int index) {
			return Add(this, Hex.neighbors[index]);
		}

		#region Static

		/// <summary> Neighbor offsets from downRight to upRight </summary>
		public static Hex[] neighbors = new Hex[] {
						new Hex(1, -1, 0),
						new Hex(0, -1, 1),
						new Hex(-1, 0, 1),
						new Hex(-1, 1, 0),
						new Hex(0, 1, -1),
						new Hex(1, 0, -1)
				};


		public static Hex Add(Hex a, Hex b) {
			return new Hex(a.x + b.x, a.y + b.y, a.z + b.z);
		}

		public static int Distance(Hex a, Hex b) {
			return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z)) / 2;
		}

		public static IEnumerable<(Hex, FractHex)> GetLine(Hex a, Hex b) {
			int dist = Distance(a, b);
			FractHex aNudge = new FractHex(a.x + 1e-06f, a.y + 1e-06f, a.z + -2e-06f);
			FractHex bNudge = new FractHex(b.x + 1e-06f, b.y + 1e-06f, b.z + -2e-06f);
			for (int i = 0; i < dist; i++) {
				var res = Hex.Lerp(aNudge, bNudge, 1f / dist * i);
				yield return (res.Round(), res);
			}
			yield return (b, b.ToFract());
		}

		public static IEnumerable<Hex> Radius(Hex a, int dist) {
			for (int x = -dist; x < dist; x++) {
				for (int y = Mathf.Max(-dist, -x - dist); y < Mathf.Min(+dist, -x + dist); y++) {
					var z = -x - y;
					yield return Add(a, new Hex(x, y, z));
				}
			}

		}

		public static FractHex Lerp(FractHex a, FractHex b, float t) {
			return new FractHex(a.x * (1f - t) + b.x * t, a.y * (1f - t) + b.y * t, a.z * (1f - t) + b.z * t);
		}

		public FractHex ToFract() {
			return new FractHex(pos);
		}

		#endregion
	}
}
