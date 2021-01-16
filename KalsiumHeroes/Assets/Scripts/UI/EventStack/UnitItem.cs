
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitItem : RoundStackItem {

	public Image frame;
	public Image unitImage;
	public Text unitText;

	public const int highlightPriority = Targeting.hoverPriority * 2;

	[HideInInspector] public Highlighter highlighter;

	public void Start() {

		unitImage.sprite = unit.unitData.portrait;
		if (unitText) {
			unitText.color = unit.team switch {
				Team.Neutral => Color.yellow,
				Team.Team1 => Color.green,
				Team.Team2 => Color.red,
				_ => Color.magenta,
			};
			unitText.text = unit.unitData.displayName;
		}
	}

	public void OnDestroy() {
		if (highlighter) highlighter.Unhighlight(highlightPriority);
	}

	public void OnMouseEnter() {
		if (highlighter) highlighter.Unhighlight(highlightPriority);
		highlighter = unit.tile.highlighter;
		highlighter.Highlight(Color.cyan, highlightPriority);
	}

	public void OnMouseExit() {
		if (highlighter) highlighter.Unhighlight(highlightPriority);
	}

	/// <summary>
	/// When this item is first in the stack and reached target position
	/// </summary>
	public virtual void OnReachZero(RoundStack rs) { }
}