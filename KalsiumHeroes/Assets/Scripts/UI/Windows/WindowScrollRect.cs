
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowScrollRect : ScrollRect {

	public GameObject corner;
	int doCornerUpdate;

	public RectTransform rectTransform => (RectTransform)transform;

	public override void CalculateLayoutInputHorizontal() {
		base.CalculateLayoutInputHorizontal();
	}

	public override void CalculateLayoutInputVertical() {
		base.CalculateLayoutInputVertical();
	}
	public override void GraphicUpdateComplete() {
		base.GraphicUpdateComplete();
	}

	public override void LayoutComplete() {
		base.LayoutComplete();
	}

	public override void OnBeginDrag(PointerEventData eventData) { }
	public override void OnDrag(PointerEventData eventData) { }
	public override void OnEndDrag(PointerEventData eventData) { }

	public override void OnInitializePotentialDrag(PointerEventData eventData) {
		base.OnInitializePotentialDrag(eventData);
	}

	public override void OnScroll(PointerEventData data) {
		base.OnScroll(data);
	}

	public override void Rebuild(CanvasUpdate executing) {
		base.Rebuild(executing);
		doCornerUpdate = 2;
	}

	public override void SetLayoutHorizontal() {
		base.SetLayoutHorizontal();
	}

	public override void SetLayoutVertical() {
		base.SetLayoutVertical();
	}

	public override void StopMovement() {
		base.StopMovement();
	}

	protected override void LateUpdate() {
		base.LateUpdate();
		if (doCornerUpdate > 0) {
			UpdateCorner();
			doCornerUpdate--;
		}
	}

	protected override void OnBeforeTransformParentChanged() {
		base.OnBeforeTransformParentChanged();
	}

	protected override void OnCanvasGroupChanged() {
		base.OnCanvasGroupChanged();
	}

	protected override void OnCanvasHierarchyChanged() {
		base.OnCanvasHierarchyChanged();
	}

	protected override void OnDestroy() {
		base.OnDestroy();
	}

	protected override void OnDidApplyAnimationProperties() {
		base.OnDidApplyAnimationProperties();
	}

	protected override void OnDisable() {
		base.OnDisable();
	}

	protected override void OnEnable() {
		base.OnEnable();
	}

	protected override void OnRectTransformDimensionsChange() {
		base.OnRectTransformDimensionsChange();
		doCornerUpdate = 2;
		UpdateCorner();
	}

	protected override void OnTransformParentChanged() {
		base.OnTransformParentChanged();
	}

	protected override void SetContentAnchoredPosition(Vector2 position) {
		base.SetContentAnchoredPosition(position);
	}

	protected override void SetNormalizedPosition(float value, int axis) {
		base.SetNormalizedPosition(value, axis);
	}

	void UpdateCorner() {
		var enableCorner = verticalScrollbar && verticalScrollbar.isActiveAndEnabled && horizontalScrollbar && horizontalScrollbar.isActiveAndEnabled;
		if (corner) {
			if (corner.TryGetComponent<WindowResizer>(out var res)) {
				if (res.dragging) {
					doCornerUpdate++;
					return;
				}
			}
			corner.SetActive(enableCorner);
		}
	}
}