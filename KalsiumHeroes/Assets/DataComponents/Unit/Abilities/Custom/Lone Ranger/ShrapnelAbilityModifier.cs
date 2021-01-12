using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelAbilityModifier : UnitModifier, IOnTurnStart_Unit {

	public ShrapnelAbilityData casterData;

	[HideInInspector] public List<Tile> aoe;
	[HideInInspector] public Tile target;

	public virtual void OnTurnStart() {
		foreach (var tile in aoe) {
			if (tile.unit) tile.unit.Damage(casterData.damage.value, casterData.damageType);
		}
		Debug.Log("Shrapnel?");
		Destroy(this);
	}

}
