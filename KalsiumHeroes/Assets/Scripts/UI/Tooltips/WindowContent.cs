
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Rendering;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class WindowContent : UIBehaviour, ILayoutElement, ILayoutGroup, ILayoutController {

	private RectTransform _rect;
	protected RectTransform rectTransform => _rect == null ? _rect = GetComponent<RectTransform>() : _rect;

	protected DrivenRectTransformTracker drivens;
	private List<RectTransform> children = new List<RectTransform>();

	[SerializeField] float width = 100;
	float ILayoutElement.minWidth => width;
	float ILayoutElement.preferredWidth => width;
	float ILayoutElement.flexibleWidth => width;
	[SerializeField] float height = 100;
	float ILayoutElement.minHeight => height;
	float ILayoutElement.preferredHeight => height;
	float ILayoutElement.flexibleHeight => height;
	int ILayoutElement.layoutPriority => 1;

	void ILayoutElement.CalculateLayoutInputHorizontal() {

		children.Clear();
		var ignorers = ListPool<ILayoutIgnorer>.Get();
		for (int i = 0; i < rectTransform.childCount; i++) {
			var childRect = rectTransform.GetChild(i) as RectTransform;
			if (childRect == null || !childRect.gameObject.activeInHierarchy)
				continue;

			childRect.GetComponents<ILayoutIgnorer>(ignorers);

			if (ignorers.Count == 0) {
				children.Add(childRect);
				continue;
			}

			for (int j = 0; j < ignorers.Count; j++) {
				var ignorer = ignorers[j];
				if (!ignorer.ignoreLayout) {
					children.Add(childRect);
					break;
				}
			}
		}
		ListPool<ILayoutIgnorer>.Release(ignorers);

		drivens.Clear();
		foreach (var child in children) {
			drivens.Add(this, child, DrivenTransformProperties.None);
		}

		if (children.Any()) {
			width = 0;
			height = 0;
			foreach (var child in children) {
				width = Mathf.Max(width, child.localPosition.x + child.rect.xMax);
				height = Mathf.Max(height, -child.localPosition.y - child.rect.yMin);
			}
		}
	}

	void ILayoutElement.CalculateLayoutInputVertical() { }
	void ILayoutController.SetLayoutHorizontal() { }
	void ILayoutController.SetLayoutVertical() { }

}