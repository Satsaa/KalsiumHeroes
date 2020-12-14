
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

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();


	protected void OnValidate() {
		if (source && !Application.isPlaying) data = Instantiate(source);
		if (!unit) unit = GetComponent<Unit>();
	}

	protected void Awake() {
		data = Instantiate(source);
		unit = GetComponent<Unit>();
		unit.modifiers.Add(this);
		Game.dataComponents.Add(this);
		OnAdd();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			OnRegisterAlterers();
		}
		OnLoadNonpersistent();
	}

	protected void OnDestroy() {
		OnRemove();
		foreach (var other in unit.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			AttributeBase.RemoveAlterers();
		}
		unit.modifiers.Remove(this);
		Game.dataComponents.Remove(this);
		OnUnloadNonpersistent();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<UnitModifier>(true)) {
				using (AttributeBase.ConfigurationScope(mod.altererKeys)) {
					mod.OnRegisterAlterers();
				}
				mod.OnLoadNonpersistent();
			}
		}
	}
#endif

	/// <summary> Register attribute alterers. This is the only place to do so. Alterers are automatically removed and added. </summary>
	protected virtual void OnRegisterAlterers() { }

	/// <summary> When the UnitModifier is instantiated or the scripts are reloaded. Place to add non-persistent event listeners for example. </summary>
	protected virtual void OnLoadNonpersistent() { }

	/// <summary> Same as OnRemove but exists for naming. </summary>
	protected virtual void OnUnloadNonpersistent() { }

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

