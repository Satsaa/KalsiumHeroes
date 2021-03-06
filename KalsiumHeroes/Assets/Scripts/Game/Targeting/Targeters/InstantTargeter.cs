
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class InstantTargeter : Targeter {

	public InstantTargeter(Action<Targeter> onComplete) {
		this.onComplete = onComplete;
	}

	public sealed override bool IsCompleted() => true;

	public sealed override HashSet<Tile> GetTargets() => new HashSet<Tile>();
	public sealed override Dictionary<Tile, Color> GetCustom() => new Dictionary<Tile, Color>();
	public sealed override HashSet<Tile> GetHover(Tile tile) => new HashSet<Tile>();
	public sealed override bool TrySelect(Tile tile) => false;
	public sealed override bool TryCancel() => true;
}
