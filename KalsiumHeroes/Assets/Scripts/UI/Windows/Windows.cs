
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class Windows :  Singleton<Windows> {

	public new static Transform transform => _instance.gameObject.transform;

	public float doubleClickTime = 0.5f;

	public static void MoveToTop(Transform transform) {
		transform.SetParent(Windows.transform);
		transform.SetAsLastSibling();
	}

	private List<RaycastResult> raycasts = new List<RaycastResult>();
	public void Update() {
		if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse2)) {

			var cur = EventSystem.current;
			if (cur.IsPointerOverGameObject()) {
				var ptrData = new PointerEventData(cur);
				ptrData.position = Input.mousePosition;
				cur.RaycastAll(ptrData, raycasts);
				if (raycasts.Any()) {
					var window = raycasts.First().gameObject.GetComponentInParent<Window>();
					if (window) {
						Windows.MoveToTop(window.transform);
					}
				}
			}
		}
	}
}