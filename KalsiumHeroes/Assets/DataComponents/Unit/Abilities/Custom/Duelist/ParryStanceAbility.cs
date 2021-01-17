using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceAbility : Ability, IOnAbilityCastEnd_Unit
{
    public new ParryStanceAbilityData data => (ParryStanceAbilityData)base.data;

    public override Type dataType => typeof(ParryStanceAbilityData);

    public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
        return new InstantAbilityHandler(msg, this, (ability) => {
            this.unit.AddDataComponent(data.statusModifier);
        });
    }

    public void OnAbilityCastEnd(Ability ability) {
        if (ability == this) {
            Game.client.PostEvent(new Events.Turn());
        }
    }
}
