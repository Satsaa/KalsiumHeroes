
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;

public class Window : MonoBehaviour {

	[HideInInspector] public bool dragging = false;
	public WindowContent content;
	public List<WindowResizer> resizers;
	public WindowToolbar toolbar;

	[Flags]
	public enum Edge {
		Left = 1,
		Right = 2,
		Bottom = 4,
		Top = 8,
	}

	public RectTransform rectTransform => (RectTransform)transform;

	void Reset() {
		content = GetComponentInChildren<WindowContent>(true);
		GetComponentsInChildren<WindowResizer>(true, resizers);
		toolbar = GetComponentInChildren<WindowToolbar>(true);
	}

	void OnDestroy() {
		Destroy(gameObject);
	}

}