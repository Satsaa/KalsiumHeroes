using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetWeakSpotAbility : Ability {
    public new TargetWeakSpotAbilityData abilityData => (TargetWeakSpotAbilityData)base.data;

    public override Type dataType => typeof(TargetWeakSpotAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability data) {
        return new InstantAbilityHandler(data, this, (ability) => {
            var damage = abilityData.damage.value;
            var target = Game.grid.tiles[data.targets.First()];
            var aoe = GetAffectedArea(target);
            foreach (var tile in aoe) {
                if (tile.unit) {
                    tile.unit.DealAbilityDamage(abilityData.damage.value, this, abilityData.damageType);
                    tile.unit.gameObject.AddDataComponent(abilityData.defenseReductionModifier);
                }
            }
        });
    }
}
