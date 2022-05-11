using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class InwardsImpassableEdgeModifier : EdgeModifier, IOnGetCanPass_Edge {

	public void OnGetCanPass(Tile from, Tile to, ref bool current) {
		current = context == from && current;
	}

	public void OnGetCanPass(Unit unit, Tile from, Tile to, ref bool current) {
		current = context == from && current;
	}
}
