
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

	public Window window;

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

	protected new void Reset() {
		base.Reset();
		window = GetComponentInParent<Window>();
	}

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
			var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
			foreach (var child in children) {
				var pos = child.localPosition;
				rect.min = new Vector2(
					Mathf.Min(rect.min.x, pos.x + child.rect.min.x),
					Mathf.Min(rect.min.y, -pos.y + -child.rect.max.y)
				);
				rect.max = new Vector2(
					Mathf.Max(rect.max.x, pos.x + child.rect.max.x),
					Mathf.Max(rect.max.y, -pos.y + -child.rect.min.y)
				);
			}
			var neg = new Vector2(rect.min.x, rect.min.y);
			if (neg != Vector2.zero) {
				transform.Translate(neg.x, -neg.y, 0);
				foreach (var child in children) {
					child.Translate(-neg.x, neg.y, 0);
				}
			}
			width = rect.width;
			height = rect.height;

		}
	}

	void ILayoutElement.CalculateLayoutInputVertical() { }
	void ILayoutController.SetLayoutHorizontal() { }
	void ILayoutController.SetLayoutVertical() { }

}