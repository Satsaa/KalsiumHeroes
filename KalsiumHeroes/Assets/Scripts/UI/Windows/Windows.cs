
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class Windows : Singleton<Windows> {

	public new static Transform transform => instance.gameObject.transform;

	public float doubleClickTime = 0.5f;

	public static void MoveToTop(Transform transform) {
		transform.SetParent(Windows.transform);
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