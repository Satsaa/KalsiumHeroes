
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;

public abstract class EdgeModifier : Modifier {

	public new EdgeModifierData data => (EdgeModifierData)base.data;
	public override Type dataType => typeof(EdgeModifierData);

	[HideInInspector] public Edge edge;
	/// <summary> Which tile to use as  context. Either tile1 or tile2 of the Edge. </summary>
	[field: SerializeField] public Tile context { get; protected set; }


	protected new void Awake() {
		edge = GetMasterComponent<Edge>();
		base.Awake();
		Debug.Assert(context, $"No context set! Use an initializer with AddDataComponent and call {nameof(Init)}");
		edge.modifiers.Add(this);
		edge.onEvents.Add(this);
		OnAdd();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) other.OnAdd(this);
	}

	public void Init(Tile context) {
		edge = GetMasterComponent<Edge>();
		Debug.Assert(context == edge.tile1 || context == edge.tile2, "Context was set to a Tile that doesn't have this Edge.");
		this.context = context;
	}

	protected new void OnDestroy() {
		OnRemove();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		edge.onEvents.Remove(this);
		edge.modifiers.Remove(this);
		base.OnDestroy();
	}

	/// <summary> When this EdgeModifier is being added (instantiated). </summary>
	public virtual void OnAdd() { }
	/// <summary> When this EdgeModifier is being removed (destroyed). </summary>
	public virtual void OnRemove() { }

	/// <summary> When any other EdgeModifier is being added. </summary>
	public virtual void OnAdd(EdgeModifier modifier) { }
	/// <summary> When any other EdgeModifier is being removed. </summary>
	public virtual void OnRemove(EdgeModifier modifier) { }

}

