using System.Collections.Generic;

using UnityEngine;
using System;
using System.Linq;

public class Targeting : MonoBehaviour {

	public const int targetPriority = 0;
	public const int selectionPriority = 2;
	public const int hoverPriority = 4;

	public readonly Color targetColor = new Color(0.25f, 0.75f, 0.25f);
	public readonly Color selectionColor = new Color(0.1f, 0.7f, 1f);
	public readonly Color hoverColor = new Color(0.25f, 0.25f, 1f);
	public readonly Color invalidColor = new Color(0.80f, 0.25f, 0.25f);


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
			if (!TryComplete()) {

				var ray = camera.ScreenPointToRay(Input.mousePosition);
				var hex = Game.grid.RaycastHex(ray);
				var main = Game.grid.GetTile(hex);
				var area = Game.grid.Radius(hex, 1);
				List<(Tile tile, Collider col)> candidates = area.Select(v => (v, v.GetComponent<Collider>())).ToList();

				var tile = main;
				var minDist = float.PositiveInfinity;
				foreach (var candidate in candidates) {
					if (candidate.col && candidate.col.Raycast(ray, out var hit, minDist) && hit.point.y >= (main == null ? -0.25f : main.transform.position.y - 0.25f)) {
						minDist = hit.distance;
						tile = candidate.tile;
					}
				}

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
		foreach (var target in targets) target.highlighter.Unhighlight(targetPriority);
		targets = targeter.GetTargets();
		foreach (var target in targets) target.highlighter.Highlight(targetColor, targetPriority);
	}

	void UnhighlightSelections() {
		foreach (var selection in targeter.selections) selection.highlighter.Unhighlight(selectionPriority);
	}

	void HighlightSelections() {
		foreach (var selection in targeter.selections) selection.highlighter.Highlight(selectionColor, selectionPriority);
	}

	void RefreshHovers(Tile tile) {
		foreach (var hover in hovers) hover.highlighter.Unhighlight(hoverPriority);
		if (tile == null) hovers.Clear();
		else hovers = targeter.GetHover(tile);
		foreach (var hover in hovers) hover.highlighter.Highlight(hoverIsValid ? hoverColor : invalidColor, hoverPriority);
	}

	public void RefreshCustoms(Dictionary<Tile, (Color, int)> newCustoms) {
		foreach (var kv in customs) kv.Key.highlighter.Unhighlight(kv.Value.priority);
		customs = newCustoms;
		foreach (var kv in customs) kv.Key.highlighter.Highlight(kv.Value.color, kv.Value.priority);
	}

}
