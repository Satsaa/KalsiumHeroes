using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CordialInvitationAbility : Ability
{
    public new CordialInvitationAbilityData data => (CordialInvitationAbilityData)base.data;

    public override Type dataType => typeof(CordialInvitationAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            var target = Game.grid.tiles[msg.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                if (tile.unit) {
                    var givenStatus = tile.unit.gameObject.AddDataComponent<CordialInvitationStatus>(data.statusModifier);
                    var gainedStatus = this.unit.gameObject.AddDataComponent<CordialInvitationStatus>(data.statusModifier);
                    givenStatus.data.opponentStatus = gainedStatus;
                    givenStatus.data.opponent = this.unit;
                    gainedStatus.data.opponentStatus = givenStatus;
                    gainedStatus.data.opponent = tile.unit;
                }
            }
        });
    }
}
