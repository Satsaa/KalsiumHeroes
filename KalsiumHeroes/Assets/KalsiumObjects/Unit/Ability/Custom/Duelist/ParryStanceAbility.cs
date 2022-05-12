using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ParryStanceAbility), menuName = "KalsiumHeroes/Ability/" + nameof(ParryStanceAbility))]
public class ParryStanceAbility : NoTargetAbility, IOnAbilityCastEnd_Unit {

	[Tooltip("The UnitModifier given when casted")]
	public UnitModifier modifier;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => Create(unit, modifier));
	}

	public void OnAbilityCastEnd(Ability ability) {
		if (ability == this) {
			Game.rounds.NextTurn();
		}
	}
}
