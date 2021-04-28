
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileModifier : Modifier {

	public new TileModifierData source => (TileModifierData)_source;
	public new TileModifierData data => (TileModifierData)_data;
	public override Type dataType => typeof(TileModifierData);
	public Tile tile => (Tile)master;


	protected new void OnCreate() {
		base.OnCreate();
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnTileModifierCreate_Tile>(scope, v => v.OnTileModifierCreate(this));
			Game.onEvents.ForEach<IOnTileModifierCreate_Global>(scope, v => v.OnTileModifierCreate(this));
		}
	}

	protected new void OnRemove() {
		using (var scope = new OnEvents.Scope()) {
			tile.onEvents.ForEach<IOnTileModifierRemove_Tile>(scope, v => v.OnTileModifierRemove(this));
			Game.onEvents.ForEach<IOnTileModifierRemove_Global>(scope, v => v.OnTileModifierRemove(this));
		}
		base.OnRemove();
	}

}

