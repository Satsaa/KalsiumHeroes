using System.Collections.Generic;

using UnityEngine;
using System;
using System.Linq;

public class Targeting : MonoBehaviour {

	private RoundManager rm => Game.rounds;
	private Events e => Game.events;
	private bool finished => e.finished;

	private Tile prevHoverTile;

	Targeter targeter;
	[SerializeField] new Camera camera;

	public event Action onTargeterStart;
	public event Action onTargeterEnd;


	public bool TryStartTargeter(Targeter targeter) {
		if (this.targeter != null) return false;
		this.targeter = targeter;
		prevHoverTile = null;
		this.targeter.RefreshTargets();
		this.targeter.Hover(null);
		RefreshHighlights();
		onTargeterStart?.Invoke();
		TryComplete();
		return true;
	}


	void Start() {
		if (camera == null) camera = Camera.main;
	}

	void Update() {
		if (targeter != null) {
			if (!TryComplete()) {
				var tile = Game.grid.RaycastTile(camera.ScreenPointToRay(Input.mousePosition));
				if (Input.GetKeyDown(KeyCode.Mouse0)) {
					if (tile == null) {
						TryCancel();
						return;
					}
					if (targeter.Select(tile)) {
						if (!TryComplete()) {
							RefreshHighlights();
						}
					} else {
						TryCancel();
					}
				} else if (Input.GetKeyDown(KeyCode.Mouse1)) {
					TryCancel();
					return;
				} else {
					if (prevHoverTile != tile) {
						prevHoverTile = tile;
						targeter.Hover(tile);
						RefreshHighlights();
					}
				}
			}
		}
	}


	bool TryComplete() {
		if (targeter.IsCompleted()) {
			Complete();
			return true;
		}
		return false;
	}

	void Complete() {
		targeter.onComplete(targeter);
		End();
	}


	bool TryCancel() {
		if (targeter.Cancel()) {
			Cancel();
			return true;
		}
		return false;
	}

	void Cancel() {
		targeter.onCancel?.Invoke(targeter);
		End();
	}


	void End() {
		targeter = null;
		prevHoverTile = null;
		ClearHighlights();
		onTargeterEnd?.Invoke();
	}


	void RefreshHighlights() {
		targeter.RefreshHighlights();

		ClearHighlights();

		foreach (var kv in targeter.highlights) {
			var tile = kv.Key;
			var color = kv.Value;
			tile.highlighter.Highlight(color);
		}
	}

	void ClearHighlights() {
		foreach (var kv in Game.grid.tiles) {
			var tile = kv.Value;
			tile.highlighter.DisableHighlight();
		}
	}

}
