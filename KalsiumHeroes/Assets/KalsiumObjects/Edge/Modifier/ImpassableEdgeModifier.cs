using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Muc.Extensions;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ImpassableEdgeModifier), menuName = "KalsiumHeroes/EdgeModifier/" + nameof(ImpassableEdgeModifier))]
public class ImpassableEdgeModifier : EdgeModifier, IOnGetCanPass_Edge {

	public void OnGetCanPass(Tile from, Tile to, ref bool current) {
		current = false;
	}
	public void OnGetCanPass(Unit unit, Tile from, Tile to, ref bool current) {
		current = false;
	}
}
