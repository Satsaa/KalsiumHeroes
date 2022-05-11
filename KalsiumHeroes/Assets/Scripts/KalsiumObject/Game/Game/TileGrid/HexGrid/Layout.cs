

namespace HexGrid {

	using System.Collections.Generic;
	using UnityEngine;

	internal static class Layout {

		public static Vector2 size = Vector2.one;
		public static Vector2 origin = Vector2.zero;

		public static Vector2 HexToPoint(Hex h) {
			var x = (Orientation.f0 * h.x + Orientation.f1 * h.y) * size.x;
			var y = (Orientation.f2 * h.x + Orientation.f3 * h.y) * size.y;
			return new Vector2(x + origin.x, y + origin.y);
		}

		public static Vector2 HexToPoint(FractHex h) {
			var x = (Orientation.f0 * h.x + Orientation.f1 * h.y) * size.x;
			var y = (Orientation.f2 * h.x + Orientation.f3 * h.y) * size.y;
			return new Vector2(x + origin.x, y + origin.y);
		}


		public static FractHex PointToHex(Vector2 p) {
			var pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
			var x = Orientation.b0 * pt.x + Orientation.b1 * pt.y;
			var y = Orientation.b2 * pt.x + Orientation.b3 * pt.y;
			return new FractHex(x, y, -x - y);
		}


		public static Vector2 CornerOffset(int corner) {
			var angle = 2f * Mathf.PI * (Orientation.start_angle - corner) / 6f;
			return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
		}


		public static IEnumerable<Vector2> Corners(Hex h) {
			var center = HexToPoint(h);
			for (int i = 0; i < 6; i++) {
				var offset = CornerOffset(i);
				yield return new Vector2(center.x + offset.x, center.y + offset.y);
			}
		}

	}
}