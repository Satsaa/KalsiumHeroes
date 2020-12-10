
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Targeter {

	public IEnumerable<Tile> targets = new List<Tile>(0);
	public List<Tile> selection = new List<Tile>(0);
	public HashSet<Tile> hovers = new HashSet<Tile>();
	public Dictionary<Tile, Color> highlights = new Dictionary<Tile, Color>();

	public Action<Targeter> onComplete;
	public Action<Targeter> onCancel;

	protected Color targetColor => new Color(0.25f, 0.75f, 0.25f);
	protected Color selectionColor => new Color(0.1f, 0.7f, 1f);
	protected Color hoverColor => new Color(0.25f, 0.25f, 1f);

	public abstract bool IsCompleted();
	public abstract void RefreshTargets();

	public virtual void RefreshHighlights() {
		highlights.Clear();
		foreach (var target in targets) highlights[target] = targetColor;
		foreach (var selected in selection) highlights[selected] = selectionColor;
		foreach (var hover in hovers) highlights[hover] = hoverColor;
	}

	/// <summary> Attempt to select a Tile. Return true if the selection is accepted. </summary>
	public virtual bool Select(Tile tile) {
		if (IsCompleted()) throw new InvalidOperationException($"Attempted to select after the {nameof(Targeter)} was completed.");
		if (targets.Contains(tile)) {
			selection.Add(tile);
			return true;
		}
		return false;
	}

	/// <summary> Hover over a Tile or nothing/null. Add it to hovers and additional Tiles around it for aoe. </summary>
	public virtual bool Hover(Tile tile) {
		hovers.Clear();
		if (tile == null) return false;
		if (targets.Contains(tile)) {
			hovers.Add(tile);
			return true;
		}
		return false;
	}

	/// <summary> Attempt canceling targeting sequence. Usually when clicking an invalid target. Return true to accept cancellation. </summary>
	public virtual bool Cancel() {
		return true;
	}
}
