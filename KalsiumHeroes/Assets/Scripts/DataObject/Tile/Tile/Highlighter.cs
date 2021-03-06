
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

public class Highlighter : MonoBehaviour {

	public static readonly Color targetColor = new Color(0.25f, 0.75f, 0.25f);
	public static readonly Color selectionColor = new Color(0.1f, 0.7f, 1f);
	public static readonly Color hoverColor = new Color(0.25f, 0.25f, 1f);
	public static readonly Color invalidColor = new Color(0.80f, 0.25f, 0.25f);

	public const int currentUnitPriority = 100;

	public const int targetingMin = 1000;
	public const int targetingMax = 1999;

	public const int targetPriority = 1250;
	public const int selectionPriority = 1500;
	public const int hoverPriority = 1750;

	public const int highlightPriority = 2100;

	[Serializable]
	class ColorItem {
		public Color color;
		public int priority;
		public ColorItem(Color color, int priority) { this.color = color; this.priority = priority; }
	}

	[SerializeField] new Renderer renderer;
	[SerializeField] List<ColorItem> colors;

	void Awake() {
		renderer = GetComponentInChildren<Renderer>();
	}

	public void Highlight(Color color, int priority) {
		if (!renderer.enabled) renderer.enabled = true;
		var item = new ColorItem(color, priority);
		for (int i = 0; i < colors.Count; i++) {
			var other = colors[i];
			if (other.priority == priority) {
				colors[i] = item;
				renderer.material.SetColor("_Color", colors.Last().color);
				return;
			} else if (other.priority > priority) {
				colors.Insert(i, item);
				renderer.material.SetColor("_Color", colors.Last().color);
				return;
			}
		}
		colors.Add(item);
		renderer.material.SetColor("_Color", color);
	}

	public void Unhighlight(int priority) {
		if (!renderer) {
			colors.Clear();
			return;
		}
		colors.RemoveAll(v => v.priority == priority);
		if (colors.Count == 0) renderer.enabled = false;
		else renderer.material.SetColor("_Color", colors.Last().color);
	}

	public void ClearRange(int min, int max) {
		colors.RemoveAll(v => v.priority >= min && v.priority <= max);
		if (colors.Count == 0) renderer.enabled = false;
		else renderer.material.SetColor("_Color", colors.Last().color);
	}

}
