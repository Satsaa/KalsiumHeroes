
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;

public class WindowResizer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	public Window window;
	public Window.Edge directions;
	public Texture2D cursorTexture;
	[Tooltip("The 'point' of the cursor")]
	public Vector2 cursorHotspot;

	Vector2 lastMousePos;
	public bool dragging = false;
	public bool exited = false;

	void Reset() {
		window = GetComponentInParent<Window>();
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

	public void LateUpdate() {
		if (dragging) {
			var diff = Input.mousePosition.xy() - lastMousePos;
			if (directions.HasFlag(Window.Edge.Left)) {
				var xDiff = diff.x;
				window.rectTransform.offsetMin += new Vector2(xDiff, 0);
			}
			if (directions.HasFlag(Window.Edge.Right)) {
				var xDiff = diff.x;
				window.rectTransform.offsetMax += new Vector2(xDiff, 0);
			}
			if (directions.HasFlag(Window.Edge.Bottom)) {
				var yDiff = diff.y;
				window.rectTransform.offsetMin += new Vector2(0, yDiff);
			}
			if (directions.HasFlag(Window.Edge.Top)) {
				var yDiff = diff.y;
				window.rectTransform.offsetMax += new Vector2(0, yDiff);
			}
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

	void OnDisable() {
		if (!window.dragging) {
			Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
		}
	}

}