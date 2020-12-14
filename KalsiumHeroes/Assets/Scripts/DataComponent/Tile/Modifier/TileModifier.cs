
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Tile))]
public class TileModifier : DataComponent {

	public TileModifierData tileModifierData => (TileModifierData)data;
	public override Type dataType => typeof(TileModifierData);

	[HideInInspector] public Tile tile;

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();


	protected void OnValidate() {
		if (source && !Application.isPlaying) data = Instantiate(source);
	}

	protected void Awake() {
		if (GetComponent<Tile>().initialized) Init();
	}

	public virtual void Init() {
		data = Instantiate(source);
		tile = GetComponent<Tile>();
		tile.modifiers.Add(this);
		Game.dataComponents.Add(this);
		OnAdd();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			OnRegisterAlterers();
		}
		OnLoadNonpersistent();
	}

	protected void OnDestroy() {
		OnRemove();
		foreach (var other in tile.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			AttributeBase.RemoveAlterers();
		}
		tile.modifiers.Remove(this);
		Game.dataComponents.Remove(this);
		OnUnloadNonpersistent();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<TileModifier>(true)) {
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

	/// <summary> When the TileModifier is instantiated or the scripts are reloaded. Place to add non-persistent event listeners for example. </summary>
	protected virtual void OnLoadNonpersistent() { }

	/// <summary> Same as OnRemove but exists for naming. </summary>
	protected virtual void OnUnloadNonpersistent() { }

	/// <summary> When this TileModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this TileModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other TileModifier is being added. </summary>
	public virtual void OnAdd(TileModifier tileModifier) { }
	/// <summary> When any other TileModifier is being removed. </summary>
	public virtual void OnRemove(TileModifier tileModifier) { }

}

