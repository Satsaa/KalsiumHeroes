
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using Muc.Components.Extended;

public class WindowToolbar : ExtendedUIBehaviour, IPointerDownHandler, IPointerUpHandler {

	[SerializeField, HideInInspector] Window window;
	Vector2 lastPointer;
	bool dragging = false;
	float lastClick = float.NegativeInfinity;
	protected Camera canvasCam;


	new protected void Awake() {
		base.Awake();
		window = GetComponentInParent<Window>();
		window.transform.SetParent(Windows.rectTransform);
	}

	public void Update() {
		if (dragging) {
			Debug.Assert(RectTransformUtility.ScreenPointToLocalPointInRectangle(Windows.rectTransform, App.input.pointer, canvasCam, out var pointer));
			var diff = pointer - lastPointer;
			diff *= window.rectTransform.lossyScale;
			window.transform.Translate(diff);
			lastPointer = pointer;
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
		canvasCam = eventData.pressEventCamera;

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
					lastClick = Time.time;
					Debug.Assert(RectTransformUtility.ScreenPointToLocalPointInRectangle(Windows.rectTransform, App.input.pointer, canvasCam, out lastPointer));
				}
				break;
		}
	}

	void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
		window.dragging = dragging = false;
	}

}