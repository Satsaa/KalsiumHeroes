
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

	/// <summary> Is this Edge considered to be passable from Tile "from". </summary>
	public bool CanPass(Unit unit, Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.");
		var value = to.data.passable.value;
		value = onEvents.Aggregate<IOnGetCanPass_Edge, bool>(value, (cur, v) => v.OnGetCanPass(unit, from, to, cur));
		value = unit.onEvents.Aggregate<IOnGetCanPass_Unit, bool>(value, (cur, v) => v.OnGetCanPass(from, this, to, cur));
		value = Game.onEvents.Aggregate<IOnGetCanPass_Global, bool>(value, (cur, v) => v.OnGetCanPass(unit, from, this, to, cur));
		return value;
	}

	/// <summary> Is this Edge considered to be passable from Tile "from". </summary>
	public bool CanPass(Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.");
		var value = to.data.passable.value;
		value = onEvents.Aggregate<IOnGetCanPass_Edge, bool>(value, (cur, v) => v.OnGetCanPass(from, to, cur));
		value = Game.onEvents.Aggregate<IOnGetCanPass_Global, bool>(value, (cur, v) => v.OnGetCanPass(from, this, to, cur));
		return value;
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Unit unit, Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.");
		var value = to.data.moveCost.value;
		value = onEvents.Aggregate<IOnGetMoveCost_Edge, float>(value, (cur, v) => v.OnGetMoveCost(unit, from, to, cur));
		value = unit.onEvents.Aggregate<IOnGetMoveCost_Unit, float>(value, (cur, v) => v.OnGetMoveCost(from, this, to, cur));
		value = Game.onEvents.Aggregate<IOnGetMoveCost_Global, float>(value, (cur, v) => v.OnGetMoveCost(unit, from, this, to, cur));
		return Mathf.Max(0, value);
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Tile from, Tile to) {
		if (to != tile1 && to != tile2) throw new ArgumentException("From must be one of the Tiles of the Edge.");
		var value = to.data.moveCost.value;
		value = onEvents.Aggregate<IOnGetMoveCost_Edge, float>(value, (cur, v) => v.OnGetMoveCost(from, to, cur));
		value = Game.onEvents.Aggregate<IOnGetMoveCost_Global, float>(value, (cur, v) => v.OnGetMoveCost(from, this, to, cur));
		return Mathf.Max(0, value);
	}
}
