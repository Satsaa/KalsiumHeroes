
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
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) other.OnCreate(this);
	}

	protected override void OnRemove() {
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		base.OnRemove();
	}

	/// <summary> When any other UnitModifier is being added. </summary>
	public virtual void OnCreate(UnitModifier modifier) { }
	/// <summary> When any other UnitModifier is being removed. </summary>
	public virtual void OnRemove(UnitModifier modifier) { }

}

