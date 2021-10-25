using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrapnelModifier : UnitModifier, IOnTurnStart_Unit, IOnDeath_Unit {

	[HideInInspector] public List<Tile> aoe;
	[HideInInspector] public Tile target;
	[HideInInspector] public ShrapnelAbility creator;
	[HideInInspector] public float calculatedDamage;
	[HideInInspector] public DamageType damageType;

	public virtual void OnTurnStart() {
		foreach (var tile in aoe) {
			foreach (var unit in tile.units) {
				unit.DealCalculatedDamage(creator, calculatedDamage, damageType);
			}
		}
		Debug.Log("Shrapnel?");
		Remove();
	}

	void IOnDeath_Unit.OnDeath() {
		Debug.Log("Shrapnel DEATH!");
	}
}
