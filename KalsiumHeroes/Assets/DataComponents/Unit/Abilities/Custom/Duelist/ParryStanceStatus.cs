using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceStatus : Status, IOnAbilityCastStart_Global, IOnDamage_Unit, IOnTurnStart_Unit
{
    [SerializeField, HideInInspector]
    Ability lastAbility;
    [SerializeField, HideInInspector]
    Unit targetUnit;

    public ParryStanceStatusData parryStanceStatusData => (ParryStanceStatusData)data;

    public override Type dataType => typeof(ParryStanceStatusData);

    protected override void OnConfigureNonpersistent(bool add) {
        base.OnConfigureNonpersistent(add);
        unit.data.defense.ConfigureAlterer(add, v => v + parryStanceStatusData.defenseIncrease.value);
    }

    public void OnAbilityCastStart(Ability ability) {
        lastAbility = ability;
        targetUnit = ability.unit;
    }

    public (float damage, DamageType type) OnDamage(float damage, DamageType type) {
        if (lastAbility.data.range.value <= 1 && lastAbility.data.abilityType == AbilityType.WeaponSkill) {
            targetUnit.DealStatusDamage(parryStanceStatusData.damage.value, this, parryStanceStatusData.damageType);
            Destroy(this);
        }
        return (damage, type);
    }

    public void OnTurnStart() {
        Destroy(this);
    }
}
