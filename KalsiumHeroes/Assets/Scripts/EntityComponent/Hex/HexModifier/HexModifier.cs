
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Unit))]
public class HexModifier : EntityComponent {

	public HexModifierData hexModifierData => (HexModifierData)data;
	public override Type dataType => typeof(HexModifierData);

	[HideInInspector] public GameHex hex;

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();


	protected void OnValidate() {
		if (source && !Application.isPlaying) data = Instantiate(source);
	}

	protected void Awake() {
		data = Instantiate(source);
		hex = GetComponent<GameHex>();
		// hex.RegisterModifier(this);
		Game.ecCache.Cache(this);
		OnAdd();
		// foreach (var other in hex.modifiers.Where(mod => mod != this)) {
		// 	other.OnAdd(this);
		// }
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			OnRegisterAlterers();
		}
		OnLoadNonpersistent();
	}

	protected void OnDestroy() {
		OnRemove();
		// foreach (var other in hex.modifiers.Where(mod => mod != this)) {
		// 	other.OnRemove(this);
		// }
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			AttributeBase.RemoveAlterers();
		}
		// hex.UnregisterModifier(this);
		Game.ecCache.Uncache(this);
		OnUnloadNonpersistent();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.ecCache.Enumerate<HexModifier>(true)) {
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

	/// <summary> When the HexModifier is instantiated or the scripts are reloaded. Place to add non-persistent event listeners for example. </summary>
	protected virtual void OnLoadNonpersistent() { }

	/// <summary> Same as OnRemove but exists for naming. </summary>
	protected virtual void OnUnloadNonpersistent() { }

	/// <summary> When this HexModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this HexModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other HexModifier is being added. </summary>
	public virtual void OnAdd(HexModifier hexModifier) { }
	/// <summary> When any other HexModifier is being removed. </summary>
	public virtual void OnRemove(HexModifier hexModifier) { }

}

