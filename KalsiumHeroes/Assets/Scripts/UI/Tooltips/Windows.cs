
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class Windows : MonoBehaviour {

	public static Windows instance => _instance;
	public float doubleClickTime = 0.5f;
	private static Windows _instance;

	private void OnValidate() => Awake();
	private void Awake() {

		if (_instance != null && _instance != this) {
			Debug.LogError($"Multiple {nameof(Windows)} GameObjects. Exterminating.");
			ObjectUtil.Destroy(this);
			return;
		}

		_instance = this;

	}

	public void MoveToTop(Transform transform) {
		transform.SetParent(this.transform);
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
						Windows.instance.MoveToTop(window.transform);
					}
				}
			}
		}
	}
}