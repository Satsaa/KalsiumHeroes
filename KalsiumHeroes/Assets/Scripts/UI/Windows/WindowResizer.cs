
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using Muc.Components.Extended;

public class WindowResizer : ExtendedUIBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler {

	[Tooltip("The resize direction.")]
	[SerializeField] Window.Edge directions;

	[Tooltip("Cursor texture when hovering or dragging.")]
	[SerializeField] Texture2D cursorTexture;

	[Tooltip("The 'point' of the cursor. (For the normal cursor it woudl be the tip of the arrow)")]
	[SerializeField] Vector2 cursorHotspot;

	[SerializeField, HideInInspector] Window window;

	[field: SerializeField, HideInInspector] public bool dragging { get; protected set; }
	[field: SerializeField, HideInInspector] public bool exited { get; protected set; }

	Vector2 lastPointer;
	Camera canvasCam;

	new protected void Awake() {
		base.Awake();
		window = GetComponentInParent<Window>();
	}

	public void LateUpdate() {
		if (dragging && window.allowResize) {
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Windows.rectTransform, App.input.pointer, canvasCam, out var pointer);
			var diff = pointer - lastPointer;
			lastPointer = pointer;
			var scrollRectRectRect = window.scrollRect.rectTransform.rect;
			if (directions.HasFlag(Window.Edge.Left)) {
				float value = Mathf.Min(diff.x, scrollRectRectRect.width - window.minWidth);
				window.rectTransform.offsetMin += new Vector2(value, 0);
			}
			if (directions.HasFlag(Window.Edge.Right)) {
				float value = Mathf.Max(diff.x, window.minWidth - scrollRectRectRect.width);
				window.rectTransform.offsetMax += new Vector2(value, 0);
			}
			if (directions.HasFlag(Window.Edge.Bottom)) {
				float value = Mathf.Min(diff.y, scrollRectRectRect.height - window.minHeight);
				window.rectTransform.offsetMin += new Vector2(0, value);
			}
			if (directions.HasFlag(Window.Edge.Top)) {
				float value = Mathf.Max(diff.y, window.minHeight - scrollRectRectRect.height);
				window.rectTransform.offsetMax += new Vector2(0, value);
			}
		}
	}

	new protected void OnDisable() {
		base.OnDisable();
		if (!window.dragging) {
			Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
		}
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
		if (window.allowResize) {
			exited = false;
			if (!window.dragging) {
				Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
			}
		}
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
		if (window.allowResize) {
			exited = true;
			if (!window.dragging) {
				Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
			}
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		if (window.allowResize) {
			window.dragging = dragging = true;
			canvasCam = eventData.pressEventCamera;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Windows.rectTransform, App.input.pointer, canvasCam, out lastPointer);
		}

	}
	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		if (window.allowResize) {
			window.dragging = dragging = false;
			if (exited) {
				Cursor.SetCursor(null, cursorHotspot, CursorMode.Auto);
			}
		}
	}

}