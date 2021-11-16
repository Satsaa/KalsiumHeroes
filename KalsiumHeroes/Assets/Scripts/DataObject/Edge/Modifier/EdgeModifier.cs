
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Muc.Editor;
using HexGrid;

public abstract class EdgeModifier : Edge.ActorModifier {

	/// <summary> Which hex to use as context. Either hex1 or hex2 of the Edge. </summary>
	[field: SerializeField] public Hex context { get; protected set; }


	public Edge edge => master;

	/// <summary> Creates an EdgeModifier based on the given source and attaches it to the master. </summary>
	public static EdgeModifier Create(Edge edge, EdgeModifier source, Hex context) {
		return Create(edge, source, v => {
			Debug.Assert(context == v.master.hex1 || context == v.master.hex2, "Context was set to a Hex not connected to this Edge.");
			v.context = context;
		});
	}

	protected override void OnCreate() {
		base.OnCreate();
		using (var scope = new Hooks.Scope()) {
			edge.hooks.ForEach<IOnEdgeModifierCreate_Edge>(scope, v => v.OnEdgeModifierCreate(this));
			Game.hooks.ForEach<IOnEdgeModifierCreate_Game>(scope, v => v.OnEdgeModifierCreate(this));
		}
	}

	protected override void OnRemove() {
		using (var scope = new Hooks.Scope()) {
			edge.hooks.ForEach<IOnEdgeModifierRemove_Edge>(scope, v => v.OnEdgeModifierRemove(this));
			Game.hooks.ForEach<IOnEdgeModifierRemove_Game>(scope, v => v.OnEdgeModifierRemove(this));
		}
		base.OnRemove();
	}

}

