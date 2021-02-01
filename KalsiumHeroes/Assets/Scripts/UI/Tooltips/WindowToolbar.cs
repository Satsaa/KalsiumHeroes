
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class WindowToolbar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public Window window;

	Vector2 lastMousePos;
	bool dragging = false;

	void Reset() {
		window = GetComponentInParent<Window>();
	}

	public void OnPointerDown(PointerEventData eventData) {
		window.dragging = dragging = true;
		lastMousePos = eventData.position;
	}

	public void Update() {
		if (dragging) {
			var diff = Input.mousePosition.xy() - lastMousePos;
			window.transform.Translate(diff);
			lastMousePos = Input.mousePosition.xy();
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		window.dragging = dragging = false;
	}

}