
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;
using Muc.Time;
using UnityEngine.EventSystems;

public abstract class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

	bool dragging;

	protected void Update() {
		if (dragging) OnDrag();
	}

	public abstract void OnDragStart(PointerEventData eventData);
	public abstract void OnDrag();
	public abstract void OnDragEnd(PointerEventData eventData);

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		if (dragging != (dragging = true)) {
			OnDragStart(eventData);
		}
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		if (dragging != (dragging = false)) {
			OnDragEnd(eventData);
		}
	}

}