

namespace HexGrid {

	using System.Collections.Generic;
	using UnityEngine;

	internal static class Layout {

		public static Vector2 size = Vector2.one;
		public static Vector2 origin = Vector2.zero;

		public static Vector2 HexToPixel(Hex h) {
			float x = (Orientation.f0 * h.x + Orientation.f1 * h.y) * size.x;
			float y = (Orientation.f2 * h.x + Orientation.f3 * h.y) * size.y;
			return new Vector2(x + origin.x, y + origin.y);
		}

		public static Vector2 HexToPixel(FractHex h) {
			float x = (Orientation.f0 * h.x + Orientation.f1 * h.y) * size.x;
			float y = (Orientation.f2 * h.x + Orientation.f3 * h.y) * size.y;
			return new Vector2(x + origin.x, y + origin.y);
		}


		public static FractHex PixelToHex(Vector2 p) {
			Vector2 pt = new Vector2((p.x - origin.x) / size.x, (p.y - origin.y) / size.y);
			float x = Orientation.b0 * pt.x + Orientation.b1 * pt.y;
			float y = Orientation.b2 * pt.x + Orientation.b3 * pt.y;
			return new FractHex(x, y, -x - y);
		}


		public static Vector2 HexCornerOffset(int corner) {
			float angle = 2f * Mathf.PI * (Orientation.start_angle - corner) / 6f;
			return new Vector2(size.x * Mathf.Cos(angle), size.y * Mathf.Sin(angle));
		}


		public static List<Vector2> PolygonCorners(Hex h) {
			List<Vector2> corners = new List<Vector2>();
			Vector2 center = HexToPixel(h);
			for (int i = 0; i < 6; i++) {
				Vector2 offset = HexCornerOffset(i);
				corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
			}
			return corners;
		}

	}
}