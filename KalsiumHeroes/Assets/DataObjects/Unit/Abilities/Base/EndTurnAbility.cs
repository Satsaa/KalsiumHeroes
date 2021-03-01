using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnAbility : NoTargetAbility {

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, t =>
			App.client.PostEvent(new GameEvents.Turn())
		);
	}
}
