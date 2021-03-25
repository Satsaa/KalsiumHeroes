
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Collections;
using UnityEngine.EventSystems;
using Muc.Extensions;
using UnityEngine.UI;

public class WindowizableTooltip : Tooltip {

	[SerializeField] Window windowPrefab;
	[SerializeField] bool disallowWindowization;
	[SerializeField] List<Object> destroyOnWindowize;
	[SerializeField, HideInInspector] bool windowized;

	public override void OnCreatorClicked(Rect creatorRect) {
		if (!disallowWindowization) Windowize(windowPrefab);
	}

	protected virtual void Windowize(Window windowPrefab) {
		if (gameObject.GetComponentInParent<Window>()) return;
		var animator = GetComponentInParent<TooltipAnimator>();
		if (animator) animator.FinishAnims();
		var window = Instantiate(windowPrefab, gameObject.transform.position, gameObject.transform.rotation, Windows.transform);
		Windows.MoveToTop(window.gameObject.transform);
		transform.SetParent(window.content.contentParent);
		root = window.gameObject;
		foreach (var destroy in destroyOnWindowize) {
			Destroy(destroy);
		}
		window.gameObject.transform.Translate(0, window.toolbar.rectTransform.sizeDelta.y * window.toolbar.rectTransform.lossyScale.y / 2, 0);
		if (animator) Destroy(animator.gameObject);
	}

	public override void OnHide() {
		if (windowized) return;
		base.OnHide();
	}

}
