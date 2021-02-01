
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Muc.Extensions;

public class Window : MonoBehaviour {

	public bool dragging = false;

	[Flags]
	public enum Edge {
		Left = 1,
		Right = 2,
		Bottom = 4,
		Top = 8,
	}

	public RectTransform rectTransform => (RectTransform)transform;

	public void Destroy() {
		Destroy(gameObject);
	}

}