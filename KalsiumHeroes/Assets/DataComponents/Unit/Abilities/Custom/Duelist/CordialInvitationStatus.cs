using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CordialInvitationStatus : Status, IOnMoveOver_Unit, IOnDamage_Unit, IOnDeath_Unit, IOnAbilityCastStart_Global
{
    Unit targetUnit;
    public new CordialInvitationStatusData data => (CordialInvitationStatusData)base.data;

    public override Type dataType => typeof(CordialInvitationStatusData);
    public void OnDeath() {
        data.opponent.data.health.value = data.opponent.data.health.other;
        Destroy(data.opponentStatus);
        Destroy(this);
    }
    public void OnAbilityCastStart(Ability ability) {
        if (ability.unit != this.unit && ability.unit != data.opponent) targetUnit = ability.unit;
    }
    public void OnDamage(ref float damage, ref DamageType type) {
        if (targetUnit != null && targetUnit != this.unit && targetUnit != data.opponent) targetUnit.gameObject.AddDataComponent(data.piercingGlare);
        return;
    }
    public void OnMoveOver(Tile from, Edge edge, Tile to) {
        var distance = Game.grid.Distance(this.unit.tile, data.opponent.tile);
        if (distance >= data.breakRange.value) {
            this.unit.gameObject.AddDataComponent(data.disgracefulBehaviour);
            Destroy(data.opponentStatus);
            Destroy(this);
        }
    }
}
