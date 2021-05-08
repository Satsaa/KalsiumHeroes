
using System.Collections.Generic;
using Muc.Systems.RenderImages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitItem : RoundStackItem {

	[HideInInspector] public Highlighter highlighter;

	new protected void Awake() {
		base.Awake();
	}

	protected void Start() {
		ValueReceiver.SendValue(gameObject, unit);
	}

	public void OnDestroy() {
		if (highlighter) highlighter.Unhighlight(Highlighter.highlightPriority);
	}

	public void OnMouseEnter() {
		if (highlighter) highlighter.Unhighlight(Highlighter.highlightPriority);
		highlighter = unit.tile.highlighter;
		highlighter.Highlight(Color.cyan, Highlighter.highlightPriority);
	}

	public void OnMouseExit() {
		if (highlighter) highlighter.Unhighlight(Highlighter.highlightPriority);
	}

}