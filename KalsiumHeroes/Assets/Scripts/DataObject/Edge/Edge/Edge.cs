
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;
using Muc.Numerics;

public class Edge : Master<EdgeModifier, EdgeModifierData, IEdgeHook> {

	public new EdgeData data => (EdgeData)_data;
	public override Type dataType => typeof(EdgeData);

	public Hex hex1;
	public Hex hex2;
	public Tile tile1 { get { Game.grid.tiles.TryGetValue(hex1.pos, out var res); return res; } }
	public Tile tile2 { get { Game.grid.tiles.TryGetValue(hex2.pos, out var res); return res; } }
	[field: SerializeField, HideInInspector]
	public TileDir direction { get; private set; }

	/// <summary> Creates an Edge based on the given source, Tile and direction. </summary>
	public static Edge Create(EdgeData source, Tile tile, TileDir direction) {
		return Create<Edge>(source, v => {
			if (tile.GetEdge(direction)) throw new InvalidOperationException("Edge already created at that position.");
			v.direction = direction;
			var dir = (int)direction;
			v.hex1 = tile;
			v.hex2 = tile.hex.GetNeighbor(dir);
			tile.SetEdge(dir, v);
		});
	}

	protected override void OnShow() {
		base.OnShow();
		var dir = (int)direction;
		var tile1 = this.tile1;
		var tile2 = this.tile2;
		actor.transform.position = (tile1.corners[dir] + tile1.corners[new CircularInt(dir + 1, 6)]) / 2;
		actor.transform.parent = Game.game.transform;
		actor.name = $"Edge ({tile1.hex.x}, {tile1.hex.y})" + (tile2 == null ? $" {direction:g}" : $" - ({tile2.hex.x}, {tile2.hex.y})");
	}

	/// <summary> Removes EdgeModifiers with matching context Tile </summary>
	public void RemoveModifiersByContext(Tile context) {
		foreach (var modifier in modifiers.Get<EdgeModifier>().Where(v => v.context == context).ToList()) {
			modifier.Remove();
		}
	}

	/// <summary> Is this Edge considered to be passable by Unit "unit" from Tile "from" to Tile "to". </summary>
	public bool CanPass(Unit unit, Tile from, Tile to) {
		if (unit is null) throw new ArgumentNullException(nameof(unit));
		if (from is null) throw new ArgumentNullException(nameof(from));
		if (to is null) throw new ArgumentNullException(nameof(to));
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.passable.current.value;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetCanPass_Edge>(scope, v => v.OnGetCanPass(unit, from, to, ref value));
			unit.hooks.ForEach<IOnGetCanPass_Unit>(scope, v => v.OnGetCanPass(from, this, to, ref value));
			Game.hooks.ForEach<IOnGetCanPass_Global>(scope, v => v.OnGetCanPass(unit, from, this, to, ref value));
		}
		return value;
	}

	/// <summary> Is this Edge considered to be passable from Tile "from" to Tile "to". </summary>
	public bool CanPass(Tile from, Tile to) {
		if (from is null) throw new ArgumentNullException(nameof(from));
		if (to is null) throw new ArgumentNullException(nameof(to));
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.passable.current.value;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetCanPass_Edge>(scope, v => v.OnGetCanPass(from, to, ref value));
			Game.hooks.ForEach<IOnGetCanPass_Global>(scope, v => v.OnGetCanPass(from, this, to, ref value));
		}
		return value;
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Unit unit, Tile from, Tile to) {
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.moveCost.current.value;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetMoveCost_Edge>(scope, v => v.OnGetMoveCost(unit, from, to, ref value));
			unit.hooks.ForEach<IOnGetMoveCost_Unit>(scope, v => v.OnGetMoveCost(from, this, to, ref value));
			Game.hooks.ForEach<IOnGetMoveCost_Global>(scope, v => v.OnGetMoveCost(unit, from, this, to, ref value));
		}
		return Mathf.Max(0, value);
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Tile from, Tile to) {
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.moveCost.current.value;
		using (var scope = new Hooks.Scope()) {
			this.hooks.ForEach<IOnGetMoveCost_Edge>(scope, v => v.OnGetMoveCost(from, to, ref value));
			Game.hooks.ForEach<IOnGetMoveCost_Global>(scope, v => v.OnGetMoveCost(from, this, to, ref value));
		}
		return Mathf.Max(0, value);
	}
}
