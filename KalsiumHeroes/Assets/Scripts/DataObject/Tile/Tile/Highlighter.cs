
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

[Serializable]
public class Highlighter {

	public static readonly Color targetColor = new(0.25f, 0.75f, 0.25f);
	public static readonly Color selectionColor = new(0.1f, 0.7f, 1f);
	public static readonly Color hoverColor = new(0.25f, 0.25f, 1f);
	public static readonly Color invalidColor = new(0.80f, 0.25f, 0.25f);

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

	public int asd;

	[SerializeField] List<ColorItem> colors = new();
	[SerializeField] Renderer renderer;

	public void OnShow(GameObject gameObject) {
		renderer = gameObject.GetComponentInChildren<HighlighterComponent>().GetComponent<Renderer>();

		renderer.enabled = colors.Any();
		if (renderer.enabled) renderer.material.SetColor("_Color", colors.Last().color);
	}

	public void OnHide() {
		renderer = null;
	}

	public void Highlight(Color color, int priority) {
		if (renderer && !renderer.enabled) renderer.enabled = true;
		var item = new ColorItem(color, priority);
		for (int i = 0; i < colors.Count; i++) {
			var other = colors[i];
			if (other.priority == priority) {
				colors[i] = item;
				if (renderer) renderer.material.SetColor("_Color", colors.Last().color);
				return;
			} else if (other.priority > priority) {
				colors.Insert(i, item);
				if (renderer) renderer.material.SetColor("_Color", colors.Last().color);
				return;
			}
		}
		colors.Add(item);
		if (renderer) renderer.material.SetColor("_Color", color);
	}

	public void Unhighlight(int priority) {
		colors.RemoveAll(v => v.priority == priority);
		if (!renderer) return;
		if (colors.Count == 0) renderer.enabled = false;
		else renderer.material.SetColor("_Color", colors.Last().color);
	}

	public void ClearRange(int min, int max) {
		colors.RemoveAll(v => v.priority >= min && v.priority <= max);
		if (!renderer) return;
		if (colors.Count == 0) renderer.enabled = false;
		else renderer.material.SetColor("_Color", colors.Last().color);
	}

}
