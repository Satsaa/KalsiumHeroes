using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockShieldAbility : NoTargetAbility {

	new public RockShieldAbilityData data => (RockShieldAbilityData)_data;
	public override Type dataType => typeof(RockShieldAbilityData);

	public override EventHandler<GameEvents.Ability> CreateHandler(GameEvents.Ability msg) {
		return new InstantAbilityHandler(msg, this, (ability) => Create(unit, data.rockShieldModifier));
	}
}
