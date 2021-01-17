using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetWeakSpotAbility : Ability {
    public new TargetWeakSpotAbilityData data => (TargetWeakSpotAbilityData)base.data;

    public override Type dataType => typeof(TargetWeakSpotAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            var damage = data.damage.value;
            var target = Game.grid.tiles[msg.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                if (tile.unit) {
                    tile.unit.DealAbilityDamage(data.damage.value, this, data.damageType);
                    tile.unit.gameObject.AddDataComponent(data.defenseReductionModifier);
                }
            }
        });
    }
}
