using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockShieldStatus : Status, IOnTakeDamage_Unit {

	public new RockShieldStatusData data => (RockShieldStatusData)_data;
	public override Type dataType => typeof(RockShieldStatusData);

	Attribute<int> shield;

	protected override void OnCreate() {
		base.OnCreate();
		shield = data.shieldHP;
		Debug.Log("Shield created! Shield HP: " + shield.current);
	}

	public void OnTakeDamage(Modifier modifier, ref float damage, ref DamageType damageType) {
		damage = CalculateDamage(damage, damageType);
		if (damage < shield.current) {
			shield.current.value = shield.current - Mathf.RoundToInt(damage);
			Debug.Log("Shield took " + damage + " damage! Shield HP: " + shield.current);
			damage = 0;
		}
		if (damage >= shield.current) {
			damage -= shield.current;
			Debug.Log("Shield took " + damage + " damage! More damage than shield had HP! Damage taken: " + damage);
		}
		if (shield.current <= 0) {
			Debug.Log("Shield was removed!");
			Remove();
		}
	}

	public override void OnExpire() {
		Debug.Log("Shield exploded on expiration!");
		var h = unit.tile;
		var radius = Game.grid.Ring(h, data.explosionRadius.current);
		foreach (var tile in radius) {
			foreach (var unit in tile.units) {
				Modifier.Create(unit, data.modifier);
			}
		}
		base.OnExpire();
	}

	float CalculateDamage(float damage, DamageType damageType) {
		if (damageType == DamageType.Physical) {
			var oldValue = damage;
			damage *= 1 - unit.data.defense.current / 100f;
		}
		if (damageType == DamageType.Magical) {
			damage *= 1 - unit.data.resistance.current / 100;
		}
		return damage;
	}
}
