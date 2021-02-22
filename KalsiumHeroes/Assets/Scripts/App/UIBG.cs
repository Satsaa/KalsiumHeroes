using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Image))]
public class UIBG : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {

	/// <summary> True when this RectTransform is hovered. This happends when no other UI elements are hovered. </summary>
	public bool hovered { get; private set; }

	new protected void Reset() {
		base.Reset();
		var canvas = GetComponent<Canvas>();
		canvas.overrideSorting = true;
		canvas.sortingOrder = System.Int16.MinValue + 1;
		var image = GetComponent<Image>();
		image.color = new Color(0, 0, 0, 0);
	}

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
		hovered = true;
	}

	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
		hovered = false;
	}
}