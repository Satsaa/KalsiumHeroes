
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class WindowViewDragger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	public RectTransform rectTransform => (RectTransform)transform;

	public Window window;

	public RectTransform contentRect;
	public Texture2D cursorTexture;
	[Tooltip("The 'point' of the cursor")]
	public Vector2 cursorHotspot;

	Vector2 lastMousePos;
	bool dragging = false;
	public bool exited = false;

	void Reset() {
		window = GetComponentInParent<Window>();
	}

	void Start() {
		window.transform.SetParent(Windows.transform);
	}


	public void OnPointerEnter(PointerEventData eventData) {
		exited = false;
		if (!window.dragging) {
			Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
		}
	}

	public void OnPointerDown(PointerEventData eventData) {
		window.dragging = dragging = true;
		lastMousePos = eventData.position;
	}

	public void Update() {
		if (dragging) {
			var diff = Input.mousePosition.xy() - lastMousePos;
			contentRect.Translate(diff);
			lastMousePos = Input.mousePosition.xy();
		}
	}

	public void OnPointerUp(PointerEventData eventData) {
		window.dragging = dragging = false;
		if (exited) {
			Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
		}
	}

	public void OnPointerExit(PointerEventData eventData) {
		exited = true;
		if (!window.dragging) {
			Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
		}
	}

}