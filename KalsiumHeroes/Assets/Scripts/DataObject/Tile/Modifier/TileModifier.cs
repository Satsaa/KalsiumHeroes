
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TileModifier : Tile.ActorModifier {

	public Tile tile => master;

	protected override void OnCreate() {
		base.OnCreate();
		using (var scope = new Hooks.Scope()) {
			tile.hooks.ForEach<IOnTileModifierCreate_Tile>(scope, v => v.OnTileModifierCreate(this));
			Game.hooks.ForEach<IOnTileModifierCreate_Game>(scope, v => v.OnTileModifierCreate(this));
		}
	}

	protected override void OnRemove() {
		using (var scope = new Hooks.Scope()) {
			tile.hooks.ForEach<IOnTileModifierRemove_Tile>(scope, v => v.OnTileModifierRemove(this));
			Game.hooks.ForEach<IOnTileModifierRemove_Game>(scope, v => v.OnTileModifierRemove(this));
		}
		base.OnRemove();
	}

}

