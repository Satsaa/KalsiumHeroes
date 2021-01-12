using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class OutwardsImpassableEdgeModifier : EdgeModifier, IOnGetCanPass_Edge {

	public bool OnGetCanPass(Unit unit, Tile from, Tile to, bool current) {
		return context == from ? false : current;
	}

	public bool OnGetCanPass(Tile from, Tile to, bool current) {
		return context == from ? false : current;
	}
}
