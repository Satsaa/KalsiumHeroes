
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.EventSystems;
using Muc.Extensions;
using Muc.Components.Extended;

public class DataObjectTooltipProvider : ExtendedUIBehaviour, IValueReceiver, IPointerEnterHandler, IPointerExitHandler {

	[field: SerializeField, HideInInspector]
	public bool hovered { get; protected set; }

	[field: SerializeField, HideInInspector]
	public bool allowClick { get; protected set; }

	[SerializeField, HideInInspector]
	DataObjectData data;

	[SerializeField, HideInInspector]
	DataObject dataObject;

	protected Camera canvasCam;

	void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) { hovered = true; canvasCam = eventData.enterEventCamera; }
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData) => hovered = false;

	protected void Update() {
		if (hovered) {
			OnHover();
		}
	}

	bool IValueReceiver.TryHandleValue(object value) {
		if (value.Equals(this.data) || value.Equals(this.dataObject)) return true;
		if (HasValue()) Tooltips.instance.Hide(GetTooltip(), rectTransform);
		this.data = null;
		this.dataObject = null;
		if (value is DataObjectData data) return this.data = data;
		if (value is DataObject dataObject) return this.dataObject = dataObject;
		return false;
	}

	private void OnHover() {
		if (!HasValue()) return;
		if (!App.input.primary || App.input.primaryDown) {
			Tooltips.instance.Show(GetTooltip(), rectTransform, rectTransform.rect, canvasCam, InitializeTooltip);
			if (App.input.primaryDown) {
				if (allowClick) {
					Tooltips.instance.InvokeOnCreatorClicked(rectTransform.rect);
				} else {
					Tooltips.instance.Hide(GetTooltip(), rectTransform);
				}
			}
		} else {
			Tooltips.instance.Ping(GetTooltip(), rectTransform, rectTransform.rect);
		}
	}

	private void InitializeTooltip(Tooltip tooltip) {
		ValueReceiver.SendValue(tooltip.gameObject, GetValue());
	}

	private string GetTooltip() {
		if (data) return data.tooltip;
		if (dataObject) return dataObject.data.tooltip;
		return default;
	}

	private bool HasValue() {
		return GetValue();
	}

	private Object GetValue() {
		return data
			? data
			: dataObject;
	}

}