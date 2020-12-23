
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitStackItem : EventStackItem {

	[field: SerializeField]
	public Unit unit { get; private set; }
	public Image frame;
	public Image unitImage;
	public Text unitText;
	public Highlighter highlighter;

	public void Init(Unit unit) {
		this.unit = unit;

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
		if (highlighter) highlighter.Unhighlight(5);
	}

	public void OnMouseEnter() {
		if (highlighter) highlighter.Unhighlight(5);
		highlighter = unit.tile.highlighter;
		highlighter.Highlight(Color.cyan, 5);
	}

	public void OnMouseExit() {
		if (highlighter) highlighter.Unhighlight(5);
	}

	/// <summary>
	/// When this item is first in the stack and reached target position
	/// </summary>
	public virtual void OnReachZero(RoundStack rs) { }
}