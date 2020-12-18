
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
	/// <summary> Which tile to use as  context. Either tile1 or tile2 of the Edge. </summary>
	[field: SerializeField]
	public Tile context { get; protected set; }

	private Dictionary<object, AttributeBase> altererKeys = new Dictionary<object, AttributeBase>();

	protected new void Awake() {
		base.Awake();
		Debug.Assert(context, $"No context set! Use initializer parameter of AddDataComponent and call {nameof(Init)}");
		edge = GetComponent<Edge>();
		edge.modifiers.Add(this);
		OnAdd();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) {
			other.OnAdd(this);
		}
		OnConfigureNonpersistent(true);
	}

	public void Init(Tile context) {
		edge = GetComponent<Edge>();
		Debug.Assert(context == edge.tile1 || context == edge.tile2, "Context was set to a Tile that doesn't have this Edge.");
		this.context = context;
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) {
			other.OnRemove(this);
		}
		edge.modifiers.Remove(this);
		OnConfigureNonpersistent(false);
		base.OnDestroy();
	}

#if UNITY_EDITOR
	[UnityEditor.Callbacks.DidReloadScripts]
	private static void OnReloadScripts() {
		if (Application.isPlaying) {
			foreach (var mod in Game.dataComponents.Get<EdgeModifier>()) {
				if (mod) mod.OnConfigureNonpersistent(true);
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

