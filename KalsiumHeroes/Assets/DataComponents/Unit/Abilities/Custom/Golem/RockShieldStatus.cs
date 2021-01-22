using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockShieldStatus : Status, IOnTakeDamage_Unit
{
    public new RockShieldStatusData data => (RockShieldStatusData)base.data;
    public override Type dataType => typeof(RockShieldStatusData);

    Attribute<int> shield;

    public override void OnAdd() {
        shield = data.shieldHP;
        print("Shield created! Shield HP: " + shield.value);
        base.OnAdd();
    }

    public void OnTakeDamage(Modifier modifier, ref float damage, ref DamageType damageType) {
        damage = CalculateDamage(damage, damageType);
        if (damage < shield.value) {
            shield.value = shield.value - Mathf.RoundToInt(damage);
            print("Shield took " + damage + " damage! Shield HP: " + shield.value);
            damage = 0;
        }
        if (damage >= shield.value) {
            damage -= shield.value;
            print("Shield took " + damage + " damage! More damage than shield had HP! Damage taken: " + damage);
        }
        if (shield.value <= 0) {
            print("Shield was destroyed!");
            this.Destroy();
        }
    }

    public override void OnExpire() {
        print("Shield exploded on expiration!");
        var h = unit.tile;
        var radius = Game.grid.Ring(h, data.explosionRadius.value);
        foreach (var tile in radius) {
            if (tile.unit) tile.unit.gameObject.AddDataComponent(data.statusModifier);
        }
        base.OnExpire();
    }

    float CalculateDamage(float damage, DamageType damageType) {
        if (damageType == DamageType.Physical) {
            var oldValue = damage;
            damage *= (1 - unit.data.defense.value / 100f);
        }
        if (damageType == DamageType.Magical) {
            damage *= 1 - unit.data.resistance.value / 100;
        }
        return damage;
    }
}
