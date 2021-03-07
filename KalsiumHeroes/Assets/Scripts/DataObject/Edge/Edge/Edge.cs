
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;
using Muc.Numerics;

public class Edge : Master<EdgeModifier, EdgeModifierData, IEdgeOnEvent> {

	public new EdgeData data => (EdgeData)_data;
	public override Type dataType => typeof(EdgeData);

	public Hex hex1;
	public Hex hex2;
	public Tile tile1 { get { Game.grid.tiles.TryGetValue(hex1.pos, out var res); return res; } }
	public Tile tile2 { get { Game.grid.tiles.TryGetValue(hex2.pos, out var res); return res; } }

	/// <summary> Creates an Edge based on the given source, Tile and direction. </summary>
	public static Edge Create(EdgeData source, Tile tile, TileDir direction) {
		return Create<Edge>(source, v => {
			if (tile.GetEdge(direction)) throw new InvalidOperationException("Edge already created at that position.");
			var dir = (int)direction;
			var nbr = tile.GetNeighbor(dir);
			v.hex1 = tile;
			v.hex2 = tile.hex.GetNeighbor(dir);
			v.gameObject.transform.position = (tile.corners[dir] + tile.corners[new CircularInt(dir + 1, 6)]) / 2;
			v.gameObject.transform.parent = Game.game.transform;
			v.gameObject.name = $"Edge ({tile.hex.x}, {tile.hex.y})" + (nbr == null ? $" {(direction).ToString("g")}" : $" - ({nbr.hex.x}, {nbr.hex.y})");
			tile.SetEdge(dir, v);
		});
	}

	protected override void OnRemove() {
		base.OnRemove();
		ObjectUtil.Destroy(gameObject);
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
		var value = to.data.passable.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetCanPass_Edge>(scope, v => v.OnGetCanPass(unit, from, to, ref value));
			unit.onEvents.ForEach<IOnGetCanPass_Unit>(scope, v => v.OnGetCanPass(from, this, to, ref value));
			Game.onEvents.ForEach<IOnGetCanPass_Global>(scope, v => v.OnGetCanPass(unit, from, this, to, ref value));
		}
		return value;
	}

	/// <summary> Is this Edge considered to be passable from Tile "from" to Tile "to". </summary>
	public bool CanPass(Tile from, Tile to) {
		if (from is null) throw new ArgumentNullException(nameof(from));
		if (to is null) throw new ArgumentNullException(nameof(to));
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.passable.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetCanPass_Edge>(scope, v => v.OnGetCanPass(from, to, ref value));
			Game.onEvents.ForEach<IOnGetCanPass_Global>(scope, v => v.OnGetCanPass(from, this, to, ref value));
		}
		return value;
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Unit unit, Tile from, Tile to) {
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.moveCost.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetMoveCost_Edge>(scope, v => v.OnGetMoveCost(unit, from, to, ref value));
			unit.onEvents.ForEach<IOnGetMoveCost_Unit>(scope, v => v.OnGetMoveCost(from, this, to, ref value));
			Game.onEvents.ForEach<IOnGetMoveCost_Global>(scope, v => v.OnGetMoveCost(unit, from, this, to, ref value));
		}
		return Mathf.Max(0, value);
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Tile from, Tile to) {
		if (to != hex1 && to != hex2) throw new ArgumentException("To must be one of the Tiles connected to the Edge.", nameof(to));
		if (from != hex1 && from != hex2) throw new ArgumentException("From must be one of the Tiles connected to the Edge.", nameof(from));
		var value = to.data.moveCost.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetMoveCost_Edge>(scope, v => v.OnGetMoveCost(from, to, ref value));
			Game.onEvents.ForEach<IOnGetMoveCost_Global>(scope, v => v.OnGetMoveCost(from, this, to, ref value));
		}
		return Mathf.Max(0, value);
	}
}
