using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class InwardsImpassableEdgeModifier : EdgeModifier, IOnGetCanPass_Edge {

	public bool OnGetCanPass(Tile from, Tile to, bool current) {
		return context == from ? current : false;
	}

	public bool OnGetCanPass(Unit unit, Tile from, Tile to, bool current) {
		return context == from ? current : false;
	}
}
