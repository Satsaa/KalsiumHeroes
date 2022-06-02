using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(RockShieldAbility), menuName = "KalsiumHeroes/Ability/" + nameof(RockShieldAbility))]
public class RockShieldAbility : NoTargetAbility {

	public UnitModifier modifier;


	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => Create(unit, modifier));
	}
}
