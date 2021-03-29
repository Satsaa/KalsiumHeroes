
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Components.Extended;

public class TooltipProvider : ExtendedUIBehaviour, IPointerEnterHandler, IPointerExitHandler {

	[field: SerializeField, HideInInspector]
	public bool hovered { get; protected set; }

	[field: SerializeField]
	public string id { get; protected set; } = "modifier_list";

	protected Camera canvasCam;

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) { hovered = true; canvasCam = eventData.enterEventCamera; }
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => hovered = false;

	protected void Update() {
		if (hovered) {
			OnHover();
		}
	}

	protected virtual void OnHover() {
		if (!App.input.primary || App.input.primaryDown) {
			Tooltips.instance.Show(id, rectTransform, rectTransform.rect, canvasCam, InitializeTooltip);
			if (App.input.primaryDown) {
				Tooltips.instance.InvokeOnCreatorClicked(rectTransform.rect);
			}
		} else {
			Tooltips.instance.Ping(id, rectTransform, rectTransform.rect);
		}
	}

	protected virtual void InitializeTooltip(Tooltip tooltip) {

	}
}