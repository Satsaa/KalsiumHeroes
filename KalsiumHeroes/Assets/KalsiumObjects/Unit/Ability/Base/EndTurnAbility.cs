using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(EndTurnAbility), menuName = "KalsiumHeroes/Ability/" + nameof(EndTurnAbility))]
public class EndTurnAbility : NoTargetAbility {

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, t => Game.rounds.NextTurn()); // 3
	}
}
