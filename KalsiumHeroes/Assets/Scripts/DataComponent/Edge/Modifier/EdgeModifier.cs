
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

[RequireComponent(typeof(Edge))]
public abstract class EdgeModifier : DataComponent {

	public EdgeModifierData edgeModifierData => (EdgeModifierData)data;
	public override Type dataType => typeof(EdgeModifierData);

	[HideInInspector] public Edge edge;

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();

	protected new void Awake() {
		base.Awake();
		edge = GetComponent<Edge>();
		edge.modifiers.Add(this);
		OnAdd();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			OnRegisterAlterers();
		}
		OnLoadNonpersistent();
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		using (AttributeBase.ConfigurationScope(altererKeys)) {
			AttributeBase.RemoveAlterers();
		}
		edge.modifiers.Remove(this);
		OnUnloadNonpersistent();
		base.OnDestroy();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<EdgeModifier>(true)) {
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

	/// <summary> When the EdgeModifier is instantiated or the scripts are reloaded. Place to add non-persistent event listeners for example. </summary>
	protected virtual void OnLoadNonpersistent() { }

	/// <summary> Same as OnRemove but exists for naming. </summary>
	protected virtual void OnUnloadNonpersistent() { }

	/// <summary> When this EdgeModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this EdgeModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other EdgeModifier is being added. </summary>
	public virtual void OnAdd(EdgeModifier modifier) { }
	/// <summary> When any other EdgeModifier is being removed. </summary>
	public virtual void OnRemove(EdgeModifier modifier) { }

	/// <summary> Is this Edge considered to be passable from Tile "from" to Tile "to". </summary>
	public virtual bool IsPassable(Tile from, Tile to, bool current) => current;

	/// <summary> The move cost over this Edge from Tile "from" to Tile "to". </summary>
	public virtual float MoveCost(Tile from, Tile to, float current) => current;
}

