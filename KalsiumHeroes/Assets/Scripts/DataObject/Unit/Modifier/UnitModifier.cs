
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitModifier : Modifier {

	[Tooltip("Displayed image")]
	public AssetReference<Sprite> sprite;


	public Unit unit => (Unit)master;

	protected override void OnCreate() {
		base.OnCreate();
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnUnitModifierCreate_Unit>(scope, v => v.OnUnitModifierCreate(this));
			unit.tile.hooks.ForEach<IOnUnitModifierCreate_Tile>(scope, v => v.OnUnitModifierCreate(this));
			Game.hooks.ForEach<IOnUnitModifierCreate_Game>(scope, v => v.OnUnitModifierCreate(this));
		}
	}

	protected override void OnRemove() {
		using (var scope = new Hooks.Scope()) {
			unit.hooks.ForEach<IOnUnitModifierRemove_Unit>(scope, v => v.OnUnitModifierRemove(this));
			unit.tile.hooks.ForEach<IOnUnitModifierRemove_Tile>(scope, v => v.OnUnitModifierRemove(this));
			Game.hooks.ForEach<IOnUnitModifierRemove_Game>(scope, v => v.OnUnitModifierRemove(this));
		}
		base.OnRemove();
	}

}

