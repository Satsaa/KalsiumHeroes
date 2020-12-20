
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using HexGrid;
using Muc.Extensions;

public class Edge : MasterComponent<EdgeModifier> {

	public EdgeData edgeData => (EdgeData)data;
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
	public bool IsPassable(Tile from, Tile to) {
		if (!((from == tile1 && to == tile2) || (from == tile2 && to == tile1))) throw new ArgumentException("Arguments from and to must be Tiles of the Edge");
		return modifiers.Get().Aggregate(to.tileData.passable.value, (cur, v) => v.IsPassable(from, to, cur));
	}

	/// <summary> The move cost over this Edge. </summary>
	public float MoveCost(Tile from, Tile to) {
		if (!((from == tile1 && to == tile2) || (from == tile2 && to == tile1))) throw new ArgumentException("Arguments from and to must be Tiles of the Edge");
		return Mathf.Max(0, modifiers.Get().Aggregate(to.tileData.moveCost.value, (cur, v) => v.MoveCost(from, to, cur)));
	}
}
