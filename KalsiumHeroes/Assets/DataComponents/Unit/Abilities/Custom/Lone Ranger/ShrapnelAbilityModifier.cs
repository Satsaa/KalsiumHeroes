using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelAbilityModifier : UnitModifier {

	public ShrapnelAbilityData casterData;

	[HideInInspector] public List<Tile> aoe;
	[HideInInspector] public Tile target;

	public override void OnTurnStart() {
		base.OnTurnStart();

		foreach (var tile in aoe) {
			if (tile.unit) tile.unit.Damage(casterData.damage.value, casterData.damageType);
		}

		Debug.Log("We DID DAMGE?");
		Destroy(this);
	}

}
