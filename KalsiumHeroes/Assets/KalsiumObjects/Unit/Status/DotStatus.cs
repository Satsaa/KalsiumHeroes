using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(DotStatus), menuName = "KalsiumHeroes/Status/" + nameof(DotStatus))]
public class DotStatus : Status, IOnTurnEnd_Unit {

	public Attribute<float> damage;
	public DamageType damageType;


	public override void OnTurnEnd() {
		DealDamage(unit, damage.current, damageType);
		base.OnTurnEnd();
	}
}
