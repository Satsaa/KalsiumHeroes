
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitModifier : Modifier {

	public UnitModifierData unitModifierData => (UnitModifierData)data;
	public override Type dataType => typeof(UnitModifierData);

	[HideInInspector] public Unit unit;


	protected new void Awake() {
		unit = GetMasterComponent<Unit>();
		base.Awake();
		unit.onEvents.Add(this);
		unit.modifiers.Add(this);
		OnAdd();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) other.OnAdd(this);
		Game.InvokeOnAfterEvent();
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		Game.InvokeOnAfterEvent();
		unit.onEvents.Remove(this);
		unit.modifiers.Remove(this);
		base.OnDestroy();
	}

	/// <summary> When this UnitModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this UnitModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other UnitModifier is being added. </summary>
	public virtual void OnAdd(UnitModifier modifier) { }
	/// <summary> When any other UnitModifier is being removed. </summary>
	public virtual void OnRemove(UnitModifier modifier) { }

}

