
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;

public class TooltipProvider : UIBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public RectTransform rectTransform => transform as RectTransform;

	[field: SerializeField, HideInInspector]
	public bool hovered { get; protected set; }

	[field: SerializeField]
	public string id { get; protected set; } = "modifier_list";

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) => hovered = true;
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => hovered = false;

	void Update() {
		if (hovered) {
			OnHover();
		}
	}

	protected virtual void OnHover() {
		if (!Input.GetKey(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse0)) {
			Tooltips.instance.Show(id, gameObject, rectTransform.ScreenRect(), InitializeTooltip);
			if (Input.GetKeyDown(KeyCode.Mouse0)) {
				Tooltips.instance.Windowize();
			}
		} else {
			Tooltips.instance.Ping(id, gameObject, rectTransform.rect);
		}
	}

	protected virtual void InitializeTooltip(Tooltip tooltip) {

	}
}