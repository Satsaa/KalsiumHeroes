
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class UnitModifier : Modifier {

	public UnitModifierData unitModifierData => (UnitModifierData)data;
	public override Type dataType => typeof(UnitModifierData);

	public Unit unit;


	protected new void Awake() {
		unit = GetMasterComponent<Unit>();
		base.Awake();
		unit.modifiers.Add(this);
		OnAdd();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
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

	public virtual float OnHeal(float value) => value;
	public virtual float OnDamage(float value, DamageType type) => value;

	/// <summary> When the Unit's turn starts. </summary>
	public virtual void OnTurnStart() { }
	/// <summary> When the Unit's turn ends. </summary>
	public virtual void OnTurnEnd() { }

	/// <summary> When the Unit dies. </summary>
	public virtual void OnDeath() { }

	/// <summary> When the Unit casts an Ability that is not a base Ability. </summary>
	public virtual void OnAbilityCast(Ability ability) { }

	/// <summary> When the Unit casts a base Ability. </summary>
	public virtual void OnBaseAbilityCast(Ability ability) { }
}

