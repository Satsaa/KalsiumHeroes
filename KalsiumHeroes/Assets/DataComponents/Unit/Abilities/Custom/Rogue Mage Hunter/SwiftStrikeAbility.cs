using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiftStrikeAbility : Ability
{
    public new SwiftStrikeAbilityData data => (SwiftStrikeAbilityData)base.data;

    public override Type dataType => typeof(SwiftStrikeAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            var damage = data.damage.value;
            var target = Game.grid.tiles[msg.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                if (tile.unit) DealDamage(tile.unit, data.damage.value, data.damageType);
            }
        });
    }
}
