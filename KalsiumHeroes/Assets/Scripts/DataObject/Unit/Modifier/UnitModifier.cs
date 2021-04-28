
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitModifier : Modifier {

	public new UnitModifierData source => (UnitModifierData)_source;
	public new UnitModifierData data => (UnitModifierData)_data;
	public override Type dataType => typeof(UnitModifierData);
	public Unit unit => (Unit)master;

	protected override void OnCreate() {
		base.OnCreate();
		using (var scope = new OnEvents.Scope()) {
			unit.onEvents.ForEach<IOnUnitModifierCreate_Unit>(scope, v => v.OnUnitModifierCreate(this));
			unit.tile.onEvents.ForEach<IOnUnitModifierCreate_Tile>(scope, v => v.OnUnitModifierCreate(this));
			Game.onEvents.ForEach<IOnUnitModifierCreate_Global>(scope, v => v.OnUnitModifierCreate(this));
		}
	}

	protected override void OnRemove() {
		using (var scope = new OnEvents.Scope()) {
			unit.onEvents.ForEach<IOnUnitModifierRemove_Unit>(scope, v => v.OnUnitModifierRemove(this));
			unit.tile.onEvents.ForEach<IOnUnitModifierRemove_Tile>(scope, v => v.OnUnitModifierRemove(this));
			Game.onEvents.ForEach<IOnUnitModifierRemove_Global>(scope, v => v.OnUnitModifierRemove(this));
		}
		base.OnRemove();
	}

}

