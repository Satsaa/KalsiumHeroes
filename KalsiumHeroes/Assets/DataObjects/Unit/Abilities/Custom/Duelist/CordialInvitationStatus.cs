using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CordialInvitationStatus : Status, IOnMoveOver_Unit, IOnTakeDamage_Unit, IOnDeath_Unit {

	public new CordialInvitationStatusData data => (CordialInvitationStatusData)_data;
	public override Type dataType => typeof(CordialInvitationStatusData);

	public void OnDeath() {
		data.opponent.data.health.current.value = data.opponent.data.health.max;
		data.opponentStatus.Remove();
		Remove();
	}

	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.unit != this.unit && ability.unit != data.opponent) {
				Modifier.Create(master, data.piercingGlare);
			}
		}
	}

	public void OnMoveOver(Modifier reason, Tile from, Edge edge, Tile to) {
		var distance = Game.grid.Distance(this.unit.tile, data.opponent.tile);
		if (distance >= data.breakRange.current) {
			Modifier.Create(master, data.disgracefulBehaviour);
			data.opponentStatus.Remove();
			this.Remove();
		}
	}
}
