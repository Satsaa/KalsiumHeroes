
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using UnityEngine.Serialization;

public class Highlighter : MonoBehaviour {

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
		colors.RemoveAll(v => v.priority == priority);
		if (colors.Count == 0) renderer.enabled = false;
		else renderer.material.SetColor("_Color", colors.Last().color);
	}

	public void Clear() {
		renderer.enabled = false;
		colors.Clear();
	}

}
