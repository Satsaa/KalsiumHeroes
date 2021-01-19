using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CordialInvitationStatus : Status, IOnMoveOver_Unit, IOnTakeDamage_Unit, IOnDeath_Unit/*, IOnAbilityCastStart_Global*/ {

	public new CordialInvitationStatusData data => (CordialInvitationStatusData)base.data;
	public override Type dataType => typeof(CordialInvitationStatusData);

    //Unit targetUnit;

	public void OnDeath() {
		data.opponent.data.health.value = data.opponent.data.health.other;
		data.opponentStatus.Destroy();
        this.Destroy();
	}

    //DECREPIT SHIT
	//public void OnAbilityCastStart(Ability ability) {
	//	if (ability.unit != this.unit && ability.unit != data.opponent) targetUnit = ability.unit;
	//}

	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
        //OLD SHIT WAY
		//if (targetUnit != null && targetUnit != this.unit && targetUnit != data.opponent) targetUnit.gameObject.AddDataComponent(data.piercingGlare);
        if (source is Ability ability) {
            if (ability.unit != this.unit && ability.unit != data.opponent) ability.unit.gameObject.AddDataComponent(data.piercingGlare);
        }
	}

	public void OnMoveOver(Tile from, Edge edge, Tile to) {
		var distance = Game.grid.Distance(this.unit.tile, data.opponent.tile);
		if (distance >= data.breakRange.value) {
			this.unit.gameObject.AddDataComponent(data.disgracefulBehaviour);
			data.opponentStatus.Destroy();
			this.Destroy();
		}
	}
}
