
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class Targeter {

	public List<Tile> selections = new(0);
	public Dictionary<Tile, Color> highlights = new();

	public Action<Targeter> onComplete;
	public Action<Targeter> onCancel;

	public abstract bool IsCompleted();

	public abstract HashSet<Tile> GetTargets();
	public virtual HashSet<Tile> GetHover(Tile tile) => new() { tile };
	public virtual Dictionary<Tile, Color> GetCustom() => new();

	/// <summary> Attempt to select a Tile. Return true to accept the selection. </summary>
	public virtual bool TrySelect(Tile tile) {
		selections.Add(tile);
		return true;
	}

	/// <summary> Attempt canceling targeting sequence. Usually when clicking an invalid target. Return true to accept cancellation. </summary>
	public virtual bool TryCancel() {
		return true;
	}
}
