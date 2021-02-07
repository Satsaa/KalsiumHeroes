

namespace Muc.Extensions {

	using UnityEngine;

	public static class TransformExtensions {

		/// <summary> Returns a Rect that represents the element in screen space </summary>
		public static Rect ScreenRect(this RectTransform transform) {
			Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
			return new Rect((Vector2)transform.position - (size * 0.5f), size);
		}
	}

}