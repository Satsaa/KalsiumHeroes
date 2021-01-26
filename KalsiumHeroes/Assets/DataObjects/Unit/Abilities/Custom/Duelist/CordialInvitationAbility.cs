using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CordialInvitationAbility : UnitTargetAbility {

	public new CordialInvitationAbilityData data => (CordialInvitationAbilityData)_data;
	public override Type dataType => typeof(CordialInvitationAbilityData);

	public override IEnumerable<Unit> GetTargets() {
		return base.GetTargets().Where(v => !v.modifiers.Get<CordialInvitationStatus>().Any());
	}

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			var target = Game.grid.tiles[msg.targets.First()].units[msg.targetIndexes.First()];
			var aoe = GetAffectedArea(target);
			foreach (var tile in aoe) {
				foreach (var unit in tile.units) {
					var givenStatus = Modifier.Create<CordialInvitationStatus>(unit, data.statusModifier);
					var gainedStatus = Modifier.Create<CordialInvitationStatus>(this.unit, data.statusModifier);
					givenStatus.data.opponentStatus = gainedStatus;
					givenStatus.data.opponent = this.unit;
					gainedStatus.data.opponentStatus = givenStatus;
					gainedStatus.data.opponent = unit;
					gainedStatus.data.duelCaster = this;
				}
			}
		});
	}

	public override bool IsReady() {
		if (this.unit.modifiers.Get<CordialInvitationStatus>().Any()) return false;
		return base.IsReady();
	}
}
