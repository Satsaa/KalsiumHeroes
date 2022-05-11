using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CordialInvitationAbility : UnitTargetAbility {

	public CordialInvitationStatus status;


	public override IEnumerable<Unit> GetTargets() {
		return base.GetTargets().Where(v => !v.modifiers.Get<CordialInvitationStatus>().Any());
	}

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets[0]].units[msg.targetIndexes[0]];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					var givenStatus = Create(unit, status);
					var gainedStatus = Create(this.unit, status);
					givenStatus.opponentStatus = gainedStatus;
					givenStatus.opponent = this.unit;
					gainedStatus.opponentStatus = givenStatus;
					gainedStatus.opponent = unit;
					gainedStatus.duelCaster = this;
				}
			}
		});
	}

	public override bool IsReady() {
		if (this.unit.modifiers.Get<CordialInvitationStatus>().Any()) return false;
		return base.IsReady();
	}
}
