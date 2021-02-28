using System.Collections.Generic;

using UnityEngine;
using System;
using System.Linq;

[DisallowMultipleComponent]
public class Targeting : MonoBehaviour {

	private Rounds rm => Game.rounds;
	private Events e => Game.events;
	private bool finished => e.finished;

	private Tile prevHoverTile;

	Targeter targeter;
	[SerializeField] new Camera camera;

	bool hoverIsValid;
	HashSet<Tile> targets = new HashSet<Tile>();
	HashSet<Tile> hovers = new HashSet<Tile>();
	Dictionary<Tile, (Color color, int priority)> customs = new Dictionary<Tile, (Color, int)>();

	public bool TryStartTargeter(Targeter targeter) {
		if (this.targeter != null) return false;
		this.targeter = targeter;
		prevHoverTile = null;
		RefreshTargets();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnTargeterStart>(scope, v => v.OnTargeterStart(targeter));
		TryComplete();
		return true;
	}


	void Start() {
		if (camera == null) camera = Camera.main;
	}

	void Update() {
		if (targeter != null) {
			if (!TryComplete() && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {

				var tile = Game.grid.GetHoveredTile(camera);

				if (Input.GetKeyDown(KeyCode.Mouse0)) {
					if (tile == null) {
						TryCancel();
						return;
					}
					UnhighlightSelections();
					var prevLength = targeter.selections.Count;
					if (hoverIsValid && targeter.TrySelect(tile)) {
						if (!TryComplete()) {
							HighlightSelections();
						}
					} else {
						Debug.Assert(
							prevLength == targeter.selections.Count,
							"Selection changed even though TrySelect returned false. Only modify selections after succesful selection."
						);
					}
				} else if (Input.GetKeyDown(KeyCode.Mouse1)) {
					TryCancel();
					return;
				} else {
					if (prevHoverTile != tile) {
						prevHoverTile = tile;
						hoverIsValid = targets.Contains(tile);
						RefreshHovers(tile);
					}
				}
			} else {
				prevHoverTile = null;
				RefreshHovers(null);
			}
		}
	}


	bool TryComplete() {
		if (targeter.IsCompleted()) {
			targeter.onComplete(targeter);
			End();
			return true;
		}
		return false;
	}

	bool TryCancel() {
		if (targeter.TryCancel()) {
			targeter.onCancel?.Invoke(targeter);
			End();
			return true;
		}
		return false;
	}

	void End() {
		targeter = null;
		prevHoverTile = null;
		foreach (var tile in Game.grid.tiles.Values) tile.highlighter.Clear();
		using (var scope = new OnEvents.Scope()) Game.onEvents.ForEach<IOnTargeterEnd>(scope, v => v.OnTargeterEnd());
	}


	void RefreshTargets() {
		foreach (var target in targets) target.highlighter.Unhighlight(Highlighter.targetPriority);
		targets = targeter.GetTargets();
		foreach (var target in targets) target.highlighter.Highlight(Highlighter.targetColor, Highlighter.targetPriority);
	}

	void UnhighlightSelections() {
		foreach (var selection in targeter.selections) selection.highlighter.Unhighlight(Highlighter.selectionPriority);
	}

	void HighlightSelections() {
		foreach (var selection in targeter.selections) selection.highlighter.Highlight(Highlighter.selectionColor, Highlighter.selectionPriority);
	}

	void RefreshHovers(Tile tile) {
		foreach (var hover in hovers) hover.highlighter.Unhighlight(Highlighter.hoverPriority);
		if (tile == null || tile.removed) hovers.Clear();
		else hovers = targeter.GetHover(tile);
		foreach (var hover in hovers) hover.highlighter.Highlight(hoverIsValid ? Highlighter.hoverColor : Highlighter.invalidColor, Highlighter.hoverPriority);
	}

	public void RefreshCustoms(Dictionary<Tile, (Color, int)> newCustoms) {
		foreach (var kv in customs) kv.Key.highlighter.Unhighlight(kv.Value.priority);
		customs = newCustoms;
		foreach (var kv in customs) kv.Key.highlighter.Highlight(kv.Value.color, kv.Value.priority);
	}

}
