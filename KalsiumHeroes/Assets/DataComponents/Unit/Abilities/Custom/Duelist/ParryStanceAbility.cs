using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ParryStanceAbility : Ability, IOnAbilityCastEnd_Unit {

	public new ParryStanceAbilityData data => (ParryStanceAbilityData)_data;
	public override Type dataType => typeof(ParryStanceAbilityData);

	public override EventHandler<Events.Ability> CreateEventHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => {
			Modifier.Create(master, data.statusModifier);
		});
	}

	public void OnAbilityCastEnd(Ability ability) {
		if (ability == this) {
			Game.client.PostEvent(new Events.Turn());
		}
	}
}
