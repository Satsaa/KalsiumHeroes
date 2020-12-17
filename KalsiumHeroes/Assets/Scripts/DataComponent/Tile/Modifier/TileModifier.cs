
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Tile))]
public abstract class TileModifier : DataComponent {

	public TileModifierData tileModifierData => (TileModifierData)data;
	public override Type dataType => typeof(TileModifierData);

	[HideInInspector] public Tile tile;

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();


	protected new void Awake() {
		base.Awake();
		tile.modifiers.Add(this);
		Game.dataComponents.Add(this);
		OnAdd();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		OnConfigureNonpersistent(true);
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		tile.modifiers.Remove(this);
		OnConfigureNonpersistent(false);
		base.OnDestroy();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<TileModifier>()) {
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

	/// <summary> When this TileModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this TileModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other TileModifier is being added. </summary>
	public virtual void OnAdd(TileModifier modifier) { }
	/// <summary> When any other TileModifier is being removed. </summary>
	public virtual void OnRemove(TileModifier modifier) { }

}

