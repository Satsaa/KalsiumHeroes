
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Components.Extended;

public class Windows : UISingleton<Windows> {

	public float doubleClickTime = 0.5f;

	public static void MoveToTop(Transform transform) {
		transform.SetParent(Windows.rectTransform);
		transform.SetAsLastSibling();
	}

	public void ShowWindowUnderMouse() {
		if (CustomInputModule.IsPointerOverUI()) {
			var hovered = CustomInputModule.GetHoveredGameObject();
			if (hovered) {
				var window = hovered.GetComponentInParent<Window>();
				if (window) {
					Windows.MoveToTop(window.transform);
				}
			}
		}
	}
}