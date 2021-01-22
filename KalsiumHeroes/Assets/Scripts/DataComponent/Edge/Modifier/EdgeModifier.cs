
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using HexGrid;

public abstract class EdgeModifier : Modifier {

	public new EdgeModifierData data => (EdgeModifierData)_data;
	public override Type dataType => typeof(EdgeModifierData);
	public Edge edge => (Edge)master;

	/// <summary> Which hex to use as context. Either hex1 or hex2 of the Edge. </summary>
	[field: SerializeField] public Hex context { get; protected set; }

	/// <summary> Creates an EdgeModifier based on the given source and attaches it to the master. </summary>
	public static EdgeModifier Create(Edge edge, EdgeModifierData source, Hex context) {
		return Create<EdgeModifier>(edge, source, v => {
			Debug.Assert(context == v.edge.hex1 || context == v.edge.hex2, "Context was set to a Hex not connected to this Edge.");
			v.context = context;
		});
	}

	public void Init(Tile context) {
	}

	protected override void OnCreate() {
		base.OnCreate();
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) other.OnCreate(this);
	}

	protected override void OnRemove() {
		foreach (var other in edge.modifiers.Get().Where(mod => mod != this)) other.OnRemove(this);
		base.OnRemove();
	}

	/// <summary> When any other EdgeModifier is being added. </summary>
	public virtual void OnCreate(EdgeModifier modifier) { }
	/// <summary> When any other EdgeModifier is being removed. </summary>
	public virtual void OnRemove(EdgeModifier modifier) { }

}

