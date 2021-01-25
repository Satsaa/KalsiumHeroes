using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAbility : NoTargetAbility {

	public override EventHandler<Events.Ability> CreateHandler(Events.Ability msg) {
		return new InstantAbilityHandler(msg, this, t =>
			Game.client.PostEvent(new Events.Turn())
		);
	}
}
