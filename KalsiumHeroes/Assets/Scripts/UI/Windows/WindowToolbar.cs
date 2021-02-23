
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class WindowToolbar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	public Window window;

	public RectTransform rectTransform => (RectTransform)transform;

	Vector2 lastMousePos;
	bool dragging = false;
	float lastClick = float.NegativeInfinity;

	void Reset() {
		window = GetComponentInParent<Window>();
	}

	void Start() {
		window.transform.SetParent(Windows.transform);
	}

	public void OnPointerDown(PointerEventData eventData) {
		switch (eventData.button) {

			case PointerEventData.InputButton.Middle:
				window.Destroy();
				break;

			case PointerEventData.InputButton.Left:
				if (eventData.button == PointerEventData.InputButton.Left) {
					if (Time.time < lastClick + Windows.instance.doubleClickTime) {
						window.FitSize(true);
					}
					window.dragging = dragging = true;
					lastMousePos = eventData.position;
					lastClick = Time.time;
				}
				break;
		}
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