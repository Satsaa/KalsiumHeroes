
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public abstract class TooltipProvider : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[SerializeField] bool hovered;

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => hovered = true;
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => hovered = false;

	void Update() {
		if (hovered) {
			OnHover();
		}
	}

	protected abstract void OnHover();

}