
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Unit))]
public abstract class UnitModifier : DataComponent {

	public UnitModifierData unitModifierData => (UnitModifierData)data;
	public override Type dataType => typeof(UnitModifierData);

	[HideInInspector] public Unit unit;


	protected new void Awake() {
		base.Awake();
		unit = GetComponent<Unit>();
		unit.modifiers.Add(this);
		OnAdd();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		OnConfigureNonpersistent(true);
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		unit.modifiers.Remove(this);
		OnConfigureNonpersistent(false);
		base.OnDestroy();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<UnitModifier>()) {
				mod.OnConfigureNonpersistent(true);
			}
		}
	}
#endif

	/// <summary>
	/// Modifier is instantiated or the scripts are reloaded.
	/// Also when the UnitModifier is removed but with add = false.
	/// Conditionally add or remove non-persistent things here.
	/// </summary>
	protected virtual void OnConfigureNonpersistent(bool add) { }

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

	/// <summary> When a round starts. </summary>
	public virtual void OnRoundStart() { }
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

