
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class WindowToolbar : UIBehaviour, IPointerDownHandler, IPointerUpHandler {

	public RectTransform rectTransform => (RectTransform)transform;

	[SerializeField, HideInInspector] Window window;
	Vector2 lastMousePos;
	bool dragging = false;
	float lastClick = float.NegativeInfinity;

	new protected void Awake() {
		base.Awake();
		window = GetComponentInParent<Window>();
		window.transform.SetParent(Windows.transform);
	}

	public void Update() {
		if (dragging) {
			var diff = App.input.pointer - lastMousePos;
			window.transform.Translate(diff);
			lastMousePos = App.input.pointer;
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
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

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		window.dragging = dragging = false;
	}

}