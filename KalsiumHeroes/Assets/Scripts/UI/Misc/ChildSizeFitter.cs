using System.Collections;
using System.Collections.Generic;
using Muc.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ContentSizeFitter))]
public class ChildSizeFitter : UIBehaviour, ILayoutElement, ILayoutGroup {

	[System.NonSerialized] private RectTransform _rectTransform;
	protected RectTransform rectTransform {
		get {
			if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}

	}
	[System.NonSerialized] private List<RectTransform> _rectChildren = new List<RectTransform>();
	protected List<RectTransform> rectChildren => _rectChildren;


	[SerializeField] public float minWidth { get; protected set; }
	[SerializeField] public float preferredWidth { get; protected set; }
	[SerializeField] public float flexibleWidth { get; protected set; }
	[SerializeField] public float minHeight { get; protected set; }
	[SerializeField] public float preferredHeight { get; protected set; }
	[SerializeField] public float flexibleHeight { get; protected set; }

	int ILayoutElement.layoutPriority => 0;

	protected DrivenRectTransformTracker trackers;

	void ILayoutElement.CalculateLayoutInputHorizontal() {
		_rectChildren.Clear();
		var toIgnoreList = ListPool<Component>.Get();
		for (int i = 0; i < rectTransform.childCount; i++) {
			var rect = rectTransform.GetChild(i) as RectTransform;
			if (rect == null || !rect.gameObject.activeInHierarchy)
				continue;

			rect.GetComponents(typeof(ILayoutIgnorer), toIgnoreList);

			if (toIgnoreList.Count == 0) {
				_rectChildren.Add(rect);
				continue;
			}

			for (int j = 0; j < toIgnoreList.Count; j++) {
				var ignorer = (ILayoutIgnorer)toIgnoreList[j];
				if (!ignorer.ignoreLayout) {
					_rectChildren.Add(rect);
					break;
				}
			}
		}
		ListPool<Component>.Release(toIgnoreList);
		trackers.Clear();
	}

	void ILayoutElement.CalculateLayoutInputVertical() { }

	void ILayoutController.SetLayoutHorizontal() {
		foreach (var rectChild in rectChildren) {
			trackers.Add(this, rectChild, DrivenTransformProperties.None);
		}
		var rect = GetChildrenRect();
		minWidth = rect.xMax;
		preferredWidth = flexibleWidth = rect.width;
		minHeight = rect.yMax;
		preferredHeight = flexibleHeight = rect.height;
	}

	void ILayoutController.SetLayoutVertical() { }

	public Rect GetChildrenRect() {

		if (_rectChildren.Count == 0) return Rect.zero;

		var rect = Rect.MinMaxRect(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity, float.NegativeInfinity);
		foreach (RectTransform child in _rectChildren) {
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

		return rect;

	}

}
