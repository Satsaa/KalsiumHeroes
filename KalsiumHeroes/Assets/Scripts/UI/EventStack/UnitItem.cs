
using System.Collections.Generic;
using Muc.Systems.RenderImages;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitItem : RoundStackItem {

	public Image frame;
	public Image unitImage;
	public RenderImage unitRenderImage;
	public Text unitText;

	[HideInInspector] public Highlighter highlighter;

	new protected void Awake() {
		base.Awake();
	}

	protected void Start() {

		if (unit.data.portrait) {
			unitRenderImage.SetRenderObject(unit.data.portrait);
			unitImage.enabled = false;
		} else {
			unitImage.sprite = unit.data.sprite;
			unitRenderImage.enabled = false;
		}
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