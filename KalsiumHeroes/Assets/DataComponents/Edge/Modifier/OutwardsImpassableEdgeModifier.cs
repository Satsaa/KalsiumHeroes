using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

public class OutwardsImpassableEdgeModifier : EdgeModifier {

	public override bool IsPassable(Tile from, Tile to, bool current) {
		return context == from ? false : current;
	}
}
