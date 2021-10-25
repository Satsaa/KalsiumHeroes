using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CordialInvitationStatus : Status, IOnMoveOver_Unit, IOnTakeDamage_Unit, IOnDeath_Unit {

	public Attribute<int> breakRange;
	public Modifier disgracefulBehaviour;
	public Modifier piercingGlare;

	[HideInInspector] public Unit opponent;
	[HideInInspector] public CordialInvitationStatus opponentStatus;
	[HideInInspector] public CordialInvitationAbility duelCaster;


	public void OnDeath() {
		opponent.health.current.value = opponent.health.max;
		opponentStatus.Remove();
		Remove();
	}

	public void OnTakeDamage(Modifier source, ref float damage, ref DamageType type) {
		if (source is Ability ability) {
			if (ability.unit != this.unit && ability.unit != opponent) {
				Modifier.Create(master, piercingGlare);
			}
		}
	}

	public void OnMoveOver(Modifier reason, Tile from, Edge edge, Tile to) {
		var distance = Game.grid.Distance(this.unit.tile, opponent.tile);
		if (distance >= breakRange.current) {
			Modifier.Create(master, disgracefulBehaviour);
			opponentStatus.Remove();
			this.Remove();
		}
	}
}
