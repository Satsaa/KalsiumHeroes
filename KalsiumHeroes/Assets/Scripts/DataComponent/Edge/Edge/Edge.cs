
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;

public class Edge : MasterComponent<EdgeModifier, IEdgeOnEvent> {

	public new EdgeData data => (EdgeData)base.data;
	public override Type dataType => typeof(EdgeData);

	public Tile tile1;
	public Tile tile2;
	public TileDir direction;

	protected new void Awake() {
		base.Awake();
	}

	/// <summary> Removes EdgeModifiers with matching context Tile </summary>
	public void RemoveModifiersByContext(Tile context) {
		var modifiers = GetComponentsInChildren<EdgeModifier>();
		foreach (var modifier in modifiers) {
			if (modifier.context == context) {
				if (modifier.source && (modifier.source as ModifierData).container) ObjectUtil.Destroy(modifier.gameObject);
				else ObjectUtil.Destroy(modifier);
			}
		}
	}

	/// <summary> Is this Edge considered to be passable from Tile "from" to Tile "to". </summary>
	public bool CanPass(Unit unit, Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("To must be one of the Tiles of the Edge.", nameof(to));
		if (from != tile1 && from != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.", nameof(from));
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
		if (to != tile1 && to != tile2) throw new ArgumentException("To must be one of the Tiles of the Edge.", nameof(to));
		if (from != tile1 && from != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.", nameof(from));
		var value = to.data.passable.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetCanPass_Edge>(scope, v => v.OnGetCanPass(from, to, ref value));
			Game.onEvents.ForEach<IOnGetCanPass_Global>(scope, v => v.OnGetCanPass(from, this, to, ref value));
		}
		return value;
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Unit unit, Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("To must be one of the Tiles of the Edge.", nameof(to));
		if (from != tile1 && from != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.", nameof(from));
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
		if (to != tile1 && to != tile2) throw new ArgumentException("To must be one of the Tiles of the Edge.", nameof(to));
		if (from != tile1 && from != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.", nameof(from));
		var value = to.data.moveCost.value;
		using (var scope = new OnEvents.Scope()) {
			this.onEvents.ForEach<IOnGetMoveCost_Edge>(scope, v => v.OnGetMoveCost(from, to, ref value));
			Game.onEvents.ForEach<IOnGetMoveCost_Global>(scope, v => v.OnGetMoveCost(from, this, to, ref value));
		}
		return Mathf.Max(0, value);
	}
}
