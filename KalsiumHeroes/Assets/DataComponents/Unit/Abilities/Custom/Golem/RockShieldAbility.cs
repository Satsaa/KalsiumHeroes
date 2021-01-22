using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockShieldAbility : Ability
{
    public new RockShieldAbilityData data => (RockShieldAbilityData)base.data;
    public override Type dataType => typeof(RockShieldAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            var target = Game.grid.tiles[msg.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                if (tile.unit) tile.unit.gameObject.AddDataComponent(data.rockShieldModifier);
            }
        });
    }
}
