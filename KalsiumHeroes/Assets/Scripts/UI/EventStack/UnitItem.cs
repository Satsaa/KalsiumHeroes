
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitItem : RoundStackItem {

	public Image frame;
	public Image unitImage;
	public Text unitText;

	[HideInInspector] public Highlighter highlighter;

	public void Start() {

		unitImage.sprite = unit.data.portrait;
		if (unitText) {
			unitText.color = unit.team switch {
				Team.Team1 => Color.green,
				Team.Team2 => Color.red,
				_ => Color.magenta,
			};
			unitText.text = unit.data.displayName;
		}
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

	/// <summary>
	/// When this item is first in the stack and reached target position
	/// </summary>
	public virtual void OnReachZero(RoundStack rs) { }
}